using System.Net;
using Fika_Installer.Models.Enums;

namespace Fika_Installer.Models.Fika;

public sealed record FikaPingResponse(
    FikaPingResult PingResult,
    HttpStatusCode HttpStatusCode
);