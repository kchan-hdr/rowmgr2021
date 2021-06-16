using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace geographia.ags
{
    internal class TokenHelper
    {
        readonly static string _USERID = "svc_in_ROW";
        readonly static string _PASSWD = @"FfxxgUz<V7r9\V";

        private readonly string _service;
        private string _ags;
        private readonly HttpClient _client;

        private string _token;
        private DateTimeOffset _expd;

        internal TokenHelper(string service, HttpClient c = null)
        {
            if (string.IsNullOrWhiteSpace(service))
                throw new ArgumentNullException(nameof(service));

            _client = c ?? new HttpClient();
            _service = service;
        }

        internal async Task<bool> Validate()
        {
            var resp = await _client.GetFromJsonAsync<LayerDto>($"{_service}?f=json");
            var cap = resp.Capabilities;
            Trace.WriteLine(cap);
            return !string.IsNullOrWhiteSpace(cap);
        }

        internal async Task<string> GetToken()
        {
            if (IsTokeValid())
                return _token;

            var q = $"{GetAgsBase()}info?f=json";
            var resp = await _client.GetFromJsonAsync<AuthDto>(q);
            var tokenUrl = resp.AuthInfo.TokenServicesUrl;

            q = $"{tokenUrl}generateToken";
            var c = new StringContent($"username={_USERID}&password={_PASSWD}&f=json");
            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var respM = await _client.PostAsync(q,c);
            respM.EnsureSuccessStatusCode();
            var token = await respM.Content.ReadFromJsonAsync<TokenDto>();
            _token = token.Token;
            _expd = token.Expiration;
            return _token;
        }

        internal async Task<( string token, DateTimeOffset expiration )> Echo()
        {
            await GetToken();
            return (_token, _expd);
        }

        bool IsTokeValid()
        {
            if (string.IsNullOrWhiteSpace(_token))
                return false;

            Trace.WriteLine($"{_token}, {_expd} time is {DateTimeOffset.Now}");
            return DateTimeOffset.Now.AddMinutes(1) < _expd;
        }
        #region private
        T Parse<T>(string response)
        {
            var json = JsonDocument.Parse(response);
            if (!IsOk(json))
                throw new InvalidOperationException(response);
            return JsonSerializer.Deserialize<T>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        bool IsOk(JsonDocument json) => json != null && !json.RootElement.TryGetProperty("error", out _);
        
        string GetAgsBase()
        {
            const string key = "services/";

            if (!string.IsNullOrWhiteSpace(_ags))
                return _ags;

            var url = new Uri(_service);

            var scheme = url.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped);
            var host = url.GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped);
            var parts = url.Segments.Select( sx=> sx.ToLowerInvariant());

            if (!parts.Contains(key))
                throw new KeyNotFoundException();

            var good = parts.Skip(1).TakeWhile(sx => key != sx);
            var gurl = new Uri($"{scheme}://{host}");
            var g2url = new Uri(gurl, string.Join("", good));
            
            _ags = g2url.AbsoluteUri;
            return _ags;
        }
        #endregion
        #region dto
        public class AuthDto
        {
            public AuthInfoDto AuthInfo { get; set; }
            public class AuthInfoDto
            {
                public bool IsTokenBasedSecurity { get; set; }
                public string TokenServicesUrl { get; set; }
                public int ShortLivedTokenValidity { get; set; }
            }
        }
        public class TokenDto
        {
            public string Token { get; set; }
            public long Expires { get; set; }

            public DateTimeOffset Expiration { get => DateTimeOffset.FromUnixTimeMilliseconds(this.Expires); }
        }
        public class LayerDto
        {
            public string Capabilities { get; set; }
        }
        #endregion
    }
}
