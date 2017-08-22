using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    public class DocumentController : Controller
    {
        static readonly string _APP_NAME = "ROWM";

        #region ctor
        OwnerRepository _repo;

        public DocumentController(OwnerRepository r)
        {
            _repo = r;
        }
        #endregion

        [HttpGet("api/documents/{docId:Guid}/info")]
        public DocumentInfo GetDocument(Guid docId) => new DocumentInfo( _repo.GetDocument(docId));

        [HttpPut("api/documents/{docId:Guid}/info", Name="UpdateDocuMeta")]
        [ProducesResponseType(typeof(DocumentInfo), 202)]
        public async Task<IActionResult> UpdateDocument(Guid docId, [FromBody] DocumentInfo info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var d = _repo.GetDocument(docId);
            d.ApprovedDate = info.ApprovedDate;
            d.ClientSignatureDate = info.ClientSignatureDate;
            d.ClientTrackingNumber = info.ClientTrackingNumber;
            d.DeliveredDate = info.DeliveredDate;
            d.QCDate = info.QCDate;
            d.ReceivedDate = info.ReceivedDate;
            d.ReceivedFromClientDate = info.ReceivedFromClientDate;
            d.ReceivedFromOwnerDate = info.ReceivedFromOwnerDate;
            d.RowmTrackingNumber = info.RowmTrackingNumber;
            d.SentDate = info.SentDate;
            d.SignedDate = info.SignedDate;

            d.LastModified = DateTimeOffset.Now;

            return CreatedAtRoute("UpdateDocuMeta", new DocumentInfo(await _repo.UpdateDocument(d)));
        }

        [HttpGet("api/documents/{docId:Guid}")]
        public IActionResult GetFile(Guid docId)
        {
            var v = _repo.GetDocument(docId);
            return File(v.Content, v.ContentType ?? "application/pdf");
        }

        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly FormOptions _defaultFormOptions = new FormOptions();


        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        [Route("api/parcels/{pid}/documents"), HttpPost]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string pid)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var myParcel = await _repo.GetParcel(pid);

            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            var formAccumulator = new KeyValueAccumulator();

            string targetFilePath = null;
            string sourceFilename = null;
            string sourceContentType = null;

            // files

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                            // _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }
                        sourceContentType = section.ContentType;
                        sourceFilename = contentDisposition.FileName;
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }


            // Bind form data to a model
            var header = new DocumentHeader();
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            var bindingSuccessful = await TryUpdateModelAsync(header, prefix: "",
                valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var bb = System.IO.File.ReadAllBytes(targetFilePath);
            var agent = await _repo.GetAgent(header.AgentName);

            var d = await _repo.Store(header.DocumentTitle, header.DocumentType, sourceContentType, sourceFilename, agent.AgentId, bb);
            d.Agents.Add(agent);

            myParcel.Documents.Add(d);
            await _repo.UpdateParcel(myParcel);

            header.DocumentId = d.DocumentId;

            return Json(header);
        }


        #region helpers
        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;

            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);

            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }
        #endregion
    }

    #region doc header
    public class DocumentHeader
    {
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string AgentName { get; set; }
        public Guid DocumentId { get; set; }

        /// <summary>
        /// default ctor
        /// </summary>
        public DocumentHeader() { }

        internal DocumentHeader(Document d)
        {
            DocumentId = d.DocumentId;
            DocumentType = d.DocumentType;
            DocumentTitle = d.Title;
        }
    }

    public class DocumentInfo
    {
        public Guid DocumentId { get; set; }

        public string DocumentType { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; } 

        // denormalized tracking
        public bool TitleInFile { get; set; }
        public DateTimeOffset? ReceivedDate { get; set; }
        public DateTimeOffset? QCDate { get; set; }     // ready for QC
        public DateTimeOffset? ApprovedDate { get; set; }   // QC approved
        public DateTimeOffset? SentDate { get; set; } // sent to owner
        public string RowmTrackingNumber { get; set; }
        public DateTimeOffset? DeliveredDate { get; set; } // deliver to owner
        public DateTimeOffset? SignedDate { get; set; } // owner signed
        public DateTimeOffset? ReceivedFromOwnerDate { get; set; }
        public string ClientTrackingNumber { get; set; }
        public DateTimeOffset? ClientSignatureDate { get; set; } // signed by client
        public DateTimeOffset? ReceivedFromClientDate { get; set; } // received from client

        public string SharePointUrl { get; set; }
        public string BlobId { get; set; }

        /// <summary>
        /// default ctor
        /// </summary>
        public DocumentInfo() { }

        internal DocumentInfo( Document d)
        {
            DocumentId = d.DocumentId;
            DocumentType = d.DocumentType;
            Title = d.Title;
            ContentType = d.ContentType;

            ReceivedDate = d.ReceivedDate;
            QCDate = d.QCDate;
            ApprovedDate = d.ApprovedDate;
            SentDate = d.SentDate;
            DeliveredDate = d.DeliveredDate;
            SignedDate = d.SignedDate;
            ReceivedFromOwnerDate = d.ReceivedFromOwnerDate;
            ClientSignatureDate = d.ClientSignatureDate;
            ReceivedFromClientDate = d.ReceivedFromClientDate;

            RowmTrackingNumber = d.RowmTrackingNumber;
            ClientTrackingNumber = d.ClientTrackingNumber;

            SharePointUrl = d.SharePointUrl;
            BlobId = d.AzureBlobId;
        }
    }
    #endregion
}
