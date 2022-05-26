using FluentAssertions;
using RulesEngine.Extensions;
using RulesEngine.Models;

namespace MSRulesEngineSamples
{
    [TestClass]
    public class SimpleSample
    {
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
        }

        [DataRow(25.17, 50.30, 99.99, 145.78, true, true, true, true)]
        [DataRow(1.99, 12.45, 25.15, 99.99, false, false, false, true)]
        [DataTestMethod]
        public async Task SimpleRule_MultipleInputs(double price1, double price2, double price3, double price4
                                    ,bool expectedResult1, bool expectedResult2, bool expectedResult3, bool expectedResult4)
        {
            //Arrange

            var (workflowName, engine) = GenerateSimpleRuleEngine();

            var inputs = new[]
            {
                new { Price = price1 },
                new { Price = price2 },
                new { Price = price3 },
                new { Price = price4 }
            };

            //Act
            var results = await engine.ExecuteAllRulesAsync(workflowName, inputs);

            //Assert
            results.Count.Should().Be(1);

            var theResult = results.Single();
            theResult.Inputs.Count.Should().Be(4);
            theResult.IsSuccess.Should().Be(expectedResult1 && expectedResult2 && expectedResult3 && expectedResult4);

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
