using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace api.utils
{
    public class httpRequest
    {
        public static async Task<string> MakePostRequest(string url, object data)
        {
            using (HttpClient client = new HttpClient())
            {
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}