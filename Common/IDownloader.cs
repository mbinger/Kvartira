using System.Threading.Tasks;

namespace Common
{
    public interface IDownloader
    {
        public Task<Response> GetAsync(string url, string description);
        public Task Delay();
    }

    public class Response
    {
        public string Url { get; set; }
        public string Exception { get; set; }
        public int HttpStatusCode { get; set; }
        public string Content { get; set; }
    }
}