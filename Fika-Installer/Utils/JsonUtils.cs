using System.Text.Json;
using System.Text.Json.Nodes;

namespace Fika_Installer.Utils
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public static JsonObject? DeserializeFromFile(string jsonPath)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonPath);
                return JsonSerializer.Deserialize<JsonObject>(jsonContent);
            }
            catch(Exception ex)
            {
                ConUtils.WriteError($"An error occurred while reading: {jsonPath}. {ex.Message}");
                return null;
            }
        }

        public static bool SerializeToFile(string jsonPath, JsonObject jsonObject)
        {
            try
            {
                string json = JsonSerializer.Serialize<JsonObject>(jsonObject, _jsonSerializerOptions);
                File.WriteAllText(jsonPath, json);

                return true;
            }
            catch (Exception ex)
            {
                ConUtils.WriteError($"An error occurred while writing to: {jsonPath}. {ex.Message}");
            }

            return false;
        }
    }
}
