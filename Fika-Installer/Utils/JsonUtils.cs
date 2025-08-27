using System.Text.Json;

namespace Fika_Installer.Utils
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public static T? ReadJson<T>(string jsonPath)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonPath);
                return JsonSerializer.Deserialize<T>(jsonContent);
            }
            catch
            {
                ConUtils.WriteError($"An error occurred while reading: {jsonPath}");
                return default;
            }
        }

        public static bool WriteJson<T>(string jsonPath, T obj)
        {
            try
            {
                string json = JsonSerializer.Serialize<T>(obj, _jsonSerializerOptions);
                File.WriteAllText(jsonPath, json);
                
                return true;
            }
            catch (Exception ex)
            {
                ConUtils.WriteError($"An error occurred while writing to: {jsonPath}");
            }

            return false;
        }
    }
}
