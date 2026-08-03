using System.Net;
using Fika_Installer.Models.Enums;

namespace Fika_Installer.Models.Fika;

public sealed record FikaPingResponse(FikaPingResult pingResult, HttpStatusCode httpStatusCode)
{
    public FikaPingResult PingResult { get; set; } = pingResult;
    public HttpStatusCode HttpStatusCode { get; set; } = httpStatusCode;
}
