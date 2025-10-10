using System.Net;

namespace Fika_Installer.Models.Fika
{
    public enum PingResult
    {
        Success,
        Failed
    }
    
    public class FikaPingResponse(PingResult pingResult, HttpStatusCode httpStatusCode)
    {
        public PingResult PingResult {  get; set; } = pingResult;
        public HttpStatusCode HttpStatusCode { get; set; } = httpStatusCode;
    }
}
