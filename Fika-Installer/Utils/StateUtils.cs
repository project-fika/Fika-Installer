using System.Text.Json;
using System.Text.Json.Nodes;

namespace Fika_Installer.Utils
{
    /// <summary>
    /// Uses a json file to store information across multiple executions.
    /// </summary>
    public static class StateUtils
    {
        private static readonly string _stateFile = "fika-installer.state.json";

        /// <summary>
        /// Reads the state file and returns the value for the given key.
        /// If the file or key does not exist, returns default(T).
        /// For defaults, see https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/default-values
        /// </summary>
        public static T? GetValue<T>(string key)
        {
            if (!File.Exists(_stateFile))
            {
                return default;
            }

            var state = JsonUtils.DeserializeFromFile<JsonObject>(_stateFile);

            if (state == null)
            {
                Logger.Warning($"Key {key} not found in state file, using default");
                return default;
            }

            if (state.ContainsKey(key) && state[key] is JsonNode node)
            {
                return node.Deserialize<T>();
            }
            return default;
        }

        public static void SetValue<T>(string key, T value)
        {
            JsonObject state;

            if (File.Exists(_stateFile))
            {
                state = JsonUtils.DeserializeFromFile<JsonObject>(_stateFile) ?? new JsonObject();
            }
            else
            {
                state = new JsonObject();
            }

            state[key] = JsonSerializer.SerializeToNode(value);

            JsonUtils.SerializeToFile(_stateFile, state);
        }
    }
}
