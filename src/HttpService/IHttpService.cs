using System.Net.Http;
using System.Threading.Tasks;

namespace HttpService
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, bool passToken);
    }
}
