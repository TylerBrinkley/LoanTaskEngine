using LoanTaskEngine.Entities;
using LoanTaskEngine.Tasks;
using Newtonsoft.Json;

namespace LoanTaskEngine.Tests;

public class EntityTaskTests
{
    [Test]
    public void SerializeAndDeserializeEntityTask()
    {
        var task = new EntityTask("Require purchase price for purchase loans", EntityType.Loan)
        {
            TriggerConditions = new List<Condition>
            {
                new Condition("loanAmount", Comparator.Exists),
                new Condition("loanType", Comparator.Equals, "Purchase")
            },
            CompletionConditions = new List<Condition>
            {
                new Condition("purchasePrice", Comparator.Exists)
            }
        };
        var json = JsonConvert.SerializeObject(task);
        var expectedJson = $@"{{""name"":""Require purchase price for purchase loans"",""entity"":""Loan"",""triggerConditions"":[{{""field"":""loanAmount"",""comparator"":""exists""}},{{""field"":""loanType"",""comparator"":""equals"",""value"":""Purchase""}}],""completionConditions"":[{{""field"":""purchasePrice"",""comparator"":""exists""}}]}}";
        Assert.That(json, Is.EqualTo(expectedJson));
        var entityTask = JsonConvert.DeserializeObject<EntityTask>(json);
        Assert.That(entityTask, Is.Not.Null);
        json = JsonConvert.SerializeObject(entityTask);
        Assert.That(json, Is.EqualTo(expectedJson));
    }
}