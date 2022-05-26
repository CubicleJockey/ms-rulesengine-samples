using FluentAssertions;
using RulesEngine.Extensions;
using RulesEngine.Models;

namespace MSRulesEngineSamples
{
    [TestClass]
    public class SimpleSample
    {
        [TestMethod]
        public void GetRegisteredWorkFlows()
        {
            //Arrange
            var (workflowName, engine) = GenerateSimpleRuleEngine();

            //Act
            var workflows = engine.GetAllRegisteredWorkflowNames();

            workflows.Should().NotBeEmpty();
            workflows.Count.Should().Be(1);

            var workflow = workflows.Single();
            workflow.Should().NotBeNullOrWhiteSpace();
            workflow.Should().Be(workflowName);
        }

        [DataRow(1.99, false)]
        [DataRow(12.56, false)]
        [DataRow(25.16, false)]
        [DataRow(25.17, true)]
        [DataRow(30.22, true)]
        [DataRow(56.17, true)]
        [DataTestMethod]
        public async Task SimpleRule_SingleInputs(double price, bool expectedResult)
        {
            //Arrange
            var (workflowName, engine) = GenerateSimpleRuleEngine();

            var inputs = new[] { new { Price = price } };

            //Act
            var results = await engine.ExecuteAllRulesAsync(workflowName, inputs);

            //Assert
            results.Count.Should().Be(1);
            
            var theResult = results.Single();
            theResult.Inputs.Count.Should().Be(1);
            theResult.IsSuccess.Should().Be(expectedResult);

            //Some ways to show overall success or failure
            results.OnSuccess(eventName => Console.WriteLine($"'{eventName}' was successful."));
            results.OnFail(() => Console.WriteLine("Failure"));
        }

        #region Helper Methods

        private static (string WorkflowName, RulesEngine.RulesEngine Engine) GenerateSimpleRuleEngine()
        {
            const string WORKFLOW_NAME = "Super simple prices workflow #1";

            //Create a simple workflow
            var workFlows = new Workflow[]
            {
                new()
                {
                    WorkflowName = WORKFLOW_NAME,
                    Rules = new List<Rule>
                    {
                        new()
                        {
                            RuleName = "1st rule",
                            SuccessEvent = "The price is higher than $25.16",
                            ErrorMessage = "Price was did not exceed $25.16",
                            Expression = "Price > 25.16",
                            RuleExpressionType = RuleExpressionType.LambdaExpression
                        }
                    }
                }
            };

            var engine = new RulesEngine.RulesEngine(workFlows);
            return (WORKFLOW_NAME, engine);
        }

        #endregion Helper Methods
    }
}
