using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fika_Installer.Models
{
    public class SptProfile
    {
        public string ProfileId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; } 

        public SptProfile() 
        {
            ProfileId = "";
            Name = "";
            Password = "";
        }

        public SptProfile(string sessionID, string name, string password)
        {
            ProfileId = sessionID;
            Name = name;
            Password = password;
        }
    }
}
