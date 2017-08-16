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

        // Get the default form options so that we can use them to set the default limits for
        // request body data
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        [HttpPost("api/parcels/{pid}/documents_x")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddDocument(string pid)
        {
            return Ok();
        }

        #region snippet1

        // 1. Disable the form value model binding here to take control of handling 
        //    potentially large files.
        // 2. Typically antiforgery tokens are sent in request body, but since we 
        //    do not want to read the request body early, the tokens are made to be 
        //    sent via headers. The antiforgery token filter first looks for tokens
        //    in the request header and then falls back to reading the body.
        [HttpPost("api/parcels/{pid}/documents")]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string pid)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }


            // Used to accumulate all the form url encoded key value pairs in the 
            // request.
            var formAccumulator = new KeyValueAccumulator();

            string targetFilePath = null;


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
            var user = new User();
            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            var bindingSuccessful = await TryUpdateModelAsync(user, prefix: "",
                valueProvider: formValueProvider);

            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            var uploadedData = new UploadedData()
            {
                Name = user.Name,
                Age = user.Age,
                Zipcode = user.Zipcode,
                FilePath = targetFilePath
            };

            return Json(uploadedData);
        }

        #endregion



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
    }


    public class User
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(5, ErrorMessage = "Name of the user must be at least 5 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 45, ErrorMessage = "Age must be between 18 and 45 (inclusive)")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Zipcode is required")]
        public int Zipcode { get; set; }
    }

    public class UploadedData
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public int Zipcode { get; set; }

        public string FilePath { get; set; }
    }
}
