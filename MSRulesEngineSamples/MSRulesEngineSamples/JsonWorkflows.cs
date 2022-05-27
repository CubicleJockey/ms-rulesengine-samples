using FluentAssertions;
using Newtonsoft.Json;

using static System.Console;

namespace MSRulesEngineSamples
{
    [TestClass]
    public class JsonWorkflows
    {
        [DataRow(8750, "TaxRate10Percent")]
        [DataRow(37812, "TaxRate12Percent")]
        [DataRow(85000, "TaxRate22Percent")]
        [DataRow(130999, "TaxRate24Percent")]
        [DataRow(203567, "TaxRate32Percent")]
        [DataRow(499345, "TaxRate35Percent")]
        [DataRow(int.MaxValue, "TaxRate37Percent")]
        [DataTestMethod]
        [Description("This work flow is using the default parementer names. i.e: input1, input2, ...etc")]
        public async Task UseJsonWorkflow(double income, string successfulRule)
        {
            //Arrange
            var jsonWorkflow = await LoadJsonWorkFlows("tax-brackets-single-workflow.json");
            jsonWorkflow.Should().NotBeNullOrWhiteSpace();

            string[] jsonConfig = { jsonWorkflow };
            var engine = new RulesEngine.RulesEngine(jsonConfig);

            //Act
            var workflowNames = engine.GetAllRegisteredWorkflowNames();
            var input1 = new { Income = income };
            var results = await engine.ExecuteAllRulesAsync("Tax Brackets", input1);

            //Assert
            workflowNames.Should().NotBeEmpty();
            workflowNames.Count.Should().Be(1);
            workflowNames.Single().Should().Be("Tax Brackets");

            var passed = results.Single(result => result.Rule.RuleName == successfulRule);
            passed.IsSuccess.Should().BeTrue();

            var failed = results.Where(result => result.Rule.RuleName != successfulRule);
            failed.All(result => result.IsSuccess == false).Should().BeTrue();
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
