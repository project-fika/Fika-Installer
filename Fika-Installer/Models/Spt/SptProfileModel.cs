using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fika_Installer.Models.Spt
{
    public class SptProfileModel
    {
        [JsonPropertyName("info")]
        public ProfileInfo Info { get; set; } = new();
    }

    public class ProfileInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "";
    }
}
