using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PricesWatcher.Json
{
    public class RequestSender
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<string> Send(string jsonSerializedObject, Uri requestUri)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = requestUri,
                Method = HttpMethod.Post,
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes(jsonSerializedObject)),
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var hotelResponse = await _client.SendAsync(request).ConfigureAwait(false);
            hotelResponse.EnsureSuccessStatusCode();

            return await hotelResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
