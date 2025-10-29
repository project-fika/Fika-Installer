using Fika_Installer.Models.Enums;
using System.Net;

namespace Fika_Installer.Models.Fika
{
    public class FikaPingResponse(FikaPingResult pingResult, HttpStatusCode httpStatusCode)
    {
        public FikaPingResult PingResult { get; set; } = pingResult;
        public HttpStatusCode HttpStatusCode { get; set; } = httpStatusCode;
    }
}
