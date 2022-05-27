using Newtonsoft.Json;

namespace MSRulesEngineSamples
{
    public abstract class BaseHelpers
    {
        protected static async Task<string> LoadJsonWorkFlows(string fileName)
        {
            var content = await File.ReadAllTextAsync(Path.Combine("Workflows", fileName));
            if (string.IsNullOrWhiteSpace(content)) { throw new Exception($"Failed to load file: {fileName}"); }
            var asObject = JsonConvert.DeserializeObject(content);
            return JsonConvert.SerializeObject(asObject);
        }
    }
}
