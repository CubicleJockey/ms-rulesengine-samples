using FluentAssertions;
using Newtonsoft.Json;

namespace MSRulesEngineSamples
{
    [TestClass]
    public class JsonWorkflows
    {
        [TestMethod]
        public async Task UseJsonWorkflow()
        {
            var jsonWorkflow = await LoadJsonWorkFlows("tax-bracket-workflow.json");
            jsonWorkflow.Should().NotBeNullOrWhiteSpace();

            string[] jsonConfig = { jsonWorkflow };
            var engine = new RulesEngine.RulesEngine(jsonConfig);

            var inputs = new[]
            {
                new { Income = 160500 }
            };

            var workflowNames = engine.GetAllRegisteredWorkflowNames();
            workflowNames.Should().NotBeEmpty();
            workflowNames.Count.Should().Be(1);
            workflowNames.Single().Should().Be("Tax Brackets");

            var results = await engine.ExecuteAllRulesAsync("Tax Brackets", default);
        }

        #region Helper Methods

        private static async Task<string> LoadJsonWorkFlows(string fileName)
        {
            var content = await File.ReadAllTextAsync(Path.Combine("Workflows", fileName));
            if (string.IsNullOrWhiteSpace(content)) { throw new Exception($"Failed to load file: {fileName}"); }
            var asObject = JsonConvert.DeserializeObject(content);
            return JsonConvert.SerializeObject(asObject);
        }

        #endregion Helper Methods
    }
}
