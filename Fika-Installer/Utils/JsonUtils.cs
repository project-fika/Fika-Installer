using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fika_Installer.Utils
{
    public static class JsonUtils
    {
        public static JObject ReadJson(string jsonPath)
        {
            JObject jObject = new();
            try
            {
                string launcherConfig = File.ReadAllText(jsonPath);
                jObject = JObject.Parse(launcherConfig);
            }
            catch 
            {
                ConUtils.WriteError($"An error occurred while reading: {jsonPath}");
            }

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
