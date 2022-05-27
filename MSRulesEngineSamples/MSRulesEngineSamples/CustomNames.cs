using Bogus;
using FluentAssertions;
using RulesEngine.Models;
using Person = MSRulesEngineSamples.Models.Person;

namespace MSRulesEngineSamples
{
    [TestClass]
    public class CustomNames : BaseHelpers
    {
        private readonly Faker faker;
        public CustomNames()
        {
            faker = new Faker();
        }

        [DataRow("1/1/2000", "Karen", false)]
        [DataRow("11/11/1982", "André", true)]
        [DataRow("05/26/2010", "Wade", false)]
        [DataTestMethod]
        public async Task CustomNameParameters(string birthDay, string name, bool expectedSuccess)
        {
            var json = await LoadJsonWorkFlows("custom-expression-types-workflow.json");
            string[] jsonConfigs = { json };

            var engine = new RulesEngine.RulesEngine(jsonConfigs);
            engine.GetAllRegisteredWorkflowNames().Single().Should().Be("Bar Entrance Workflow");

            var person = new Person(faker.Random.Int(1), name, DateTime.Parse(birthDay));

            var input = new RuleParameter(nameof(person), person);

            var results = await engine.ExecuteAllRulesAsync("Bar Entrance Workflow", input);
            results.Should().HaveCount(1);

            var daResult = results.Single();
            daResult.IsSuccess.Should().Be(expectedSuccess);
            if (!daResult.IsSuccess)
            {
                daResult.ExceptionMessage.Should().Be("You are either not 21 or you're a Karen!");
            }
        }
    }
}
