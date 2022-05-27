using FluentAssertions;
using Newtonsoft.Json;

namespace MSRulesEngineSamples
{
    [TestClass]
    public class JsonWorkflows : BaseHelpers
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

        [DataRow(8750, false, "TaxRate10Percent")]
        [DataRow(37812, false, "TaxRate12Percent")]
        [DataRow(85000, false, "TaxRate22Percent")]
        [DataRow(130999, false, "TaxRate24Percent")]
        [DataRow(203567, false, "TaxRate32Percent")]
        [DataRow(499345, false, "TaxRate35Percent")]
        [DataRow(int.MaxValue, false, "TaxRate37Percent")]
        [DataRow(19900, true, "TaxRate10Percent")]
        [DataRow(80000, true, "TaxRate12Percent")]
        [DataRow(171569, true, "TaxRate22Percent")]
        [DataRow(329850, true, "TaxRate24Percent")]
        [DataRow(415896, true, "TaxRate32Percent")]
        [DataRow(628300, true, "TaxRate35Percent")]
        [DataRow(int.MaxValue, true, "TaxRate37Percent")]
        [DataTestMethod]
        public async Task UseComplexJsonWorkflow(double income, bool isMarried, string successfulRule)
        {
            //Arrange
            var jsonWorkflow = await LoadJsonWorkFlows("tax-brackets-complex-workflow.json");
            string[] jsonConfig = { jsonWorkflow };
            var engine = new RulesEngine.RulesEngine(jsonConfig);

            //Act
            var workflowNames = engine.GetAllRegisteredWorkflowNames();
            var input1 = new { IsMarried = isMarried, Income = income };
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
    }
}
