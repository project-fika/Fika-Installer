using System.Text.Json.Serialization;

namespace Fika_Installer.Models.Fika.Network
{
    public class CreateHeadlessProfileResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
    }
}
