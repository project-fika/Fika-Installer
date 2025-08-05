using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fika_Installer.Utils
{
    public static class JsonUtils
    {
        public static JObject ReadJson(string jsonPath)
        {
            string launcherConfig = File.ReadAllText(jsonPath);
            JObject jObject = JObject.Parse(launcherConfig);

            return jObject;
        }

        public static bool WriteJson(JObject jObject, string jsonPath)
        {
            bool result = false;

            try
            {
                using (var streamWriter = new StreamWriter(jsonPath))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.IndentChar = '\t';
                    jsonWriter.Indentation = 1;

                    jObject.WriteTo(jsonWriter);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ConUtils.WriteError($"An error occurred while writing to: {jsonPath}");
            }

            return result;
        }
    }
}
