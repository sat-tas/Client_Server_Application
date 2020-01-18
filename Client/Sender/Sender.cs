using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Sender
{
    public class Sender
    {
        private static HttpClient client = new HttpClient();

        public static async Task<HttpResponseMessage> Send(Dictionary<string, string> values)
        {
            try
            {
                var content = new FormUrlEncodedContent(values);
                HttpResponseMessage response = await client.PostAsync("http://127.0.0.1/", content);
                return response;
            }
            catch (Exception)
            {

                HttpResponseMessage response =new HttpResponseMessage();
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }
        }

    }
}
