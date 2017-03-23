using System.Net.Http;
using System.Threading.Tasks;

namespace HttpService
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, bool passToken);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content,  bool passToken);
    }
}
