using System.Text.Json;

namespace Fika_Installer.Utils
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public static T? DeserializeFromFile<T>(string jsonPath)
        {
            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    return JsonSerializer.Deserialize<T>(jsonContent);
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occurred while reading: {jsonPath}. {ex.Message}");
                    return default;
                }
            }

            return default;
        }

        public static bool SerializeToFile<T>(string jsonPath, T obj)
        {
            try
            {
                string json = JsonSerializer.Serialize<T>(obj, _jsonSerializerOptions);
                File.WriteAllText(jsonPath, json);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while writing to: {jsonPath}. {ex.Message}");
            }

            return false;
        }
    }
}
