using EnumsNET;
using LoanTaskEngine.Actions;
using LoanTaskEngine.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LoanTaskEngine.Tests;

public class EntityActionTests
{
    [Test]
    public void EnsureAllActionNamesHaveCorrespondingActionTypes()
    {
        var camelCaseNamingStrategy = new CamelCaseNamingStrategy();
        foreach (var value in Enums.GetNames<ActionName>())
        {
            var actionName = camelCaseNamingStrategy.GetPropertyName(value, hasSpecifiedName: false);
            var actionType = typeof(EntityAction).Assembly.GetType($"LoanTaskEngine.Actions.{actionName}Action", throwOnError: false, ignoreCase: true);
            Assert.That(actionType, Is.Not.Null, $"Failed to find corresponding action type for {actionName}");
        }
    }

    [Test]
    public void EnsureAllActionTypesHaveCorrespondingActionNames()
    {
        var camelCaseNamingStrategy = new CamelCaseNamingStrategy();
        var actionTypes = typeof(EntityAction).Assembly.GetTypes().Where(t => string.Equals("LoanTaskEngine.Actions", t.Namespace) && t.IsAssignableTo(typeof(EntityAction)) && !t.IsAbstract).ToList();
        Assert.That(actionTypes, Has.Count.EqualTo(Enums.GetMemberCount<ActionName>()));
        foreach (var actionType in actionTypes)
        {
            Assert.That(actionType.Name, Does.EndWith("Action"));
            var actionName = actionType.Name[..^"Action".Length];
            Assert.That(Enums.TryParse<ActionName>(actionName, out _));
        }
    }

    [Test]
    public void SerializeAndDeserializeCreateLoanAction()
    {
        var action = new CreateLoanAction("loan1");
        var json = JsonConvert.SerializeObject(action);
        var expectedJson = $@"{{""loanIdentifier"":""loan1"",""action"":""createLoan""}}";
        Assert.That(json, Is.EqualTo(expectedJson));
        var entityAction = JsonConvert.DeserializeObject<EntityAction>(json);
        Assert.That(entityAction, Is.TypeOf<CreateLoanAction>());
        json = JsonConvert.SerializeObject(entityAction);
        Assert.That(json, Is.EqualTo(expectedJson));
    }

    [Test]
    public void SerializeAndDeserializeCreateBorrowerAction()
    {
        var action = new CreateBorrowerAction("loan1", "borr1");
        var json = JsonConvert.SerializeObject(action);
        var expectedJson = $@"{{""borrowerIdentifier"":""borr1"",""loanIdentifier"":""loan1"",""action"":""createBorrower""}}";
        Assert.That(json, Is.EqualTo(expectedJson));
        var entityAction = JsonConvert.DeserializeObject<EntityAction>(json);
        Assert.That(entityAction, Is.TypeOf<CreateBorrowerAction>());
        json = JsonConvert.SerializeObject(entityAction);
        Assert.That(json, Is.EqualTo(expectedJson));
    }

    [Test]
    public void SerializeAndDeserializeSetLoanFieldAction()
    {
        var action = new SetLoanFieldAction("loan1", "loanAmount", 100000);
        var json = JsonConvert.SerializeObject(action);
        var expectedJson = $@"{{""field"":""loanAmount"",""value"":100000,""loanIdentifier"":""loan1"",""action"":""setLoanField""}}";
        Assert.That(json, Is.EqualTo(expectedJson));
        var entityAction = JsonConvert.DeserializeObject<LoanAction>(json);
        Assert.That(entityAction, Is.TypeOf<SetLoanFieldAction>());
        json = JsonConvert.SerializeObject(entityAction);
        Assert.That(json, Is.EqualTo(expectedJson));
    }

    [Test]
    public void SerializeAndDeserializeSetBorrowerFieldAction()
    {
        var action = new SetBorrowerFieldAction("borr1", "lastName", "Smith");
        var json = JsonConvert.SerializeObject(action);
        var expectedJson = $@"{{""field"":""lastName"",""value"":""Smith"",""borrowerIdentifier"":""borr1"",""action"":""setBorrowerField""}}";
        Assert.That(json, Is.EqualTo(expectedJson));
        var entityAction = JsonConvert.DeserializeObject<EntityAction>(json);
        Assert.That(entityAction, Is.TypeOf<SetBorrowerFieldAction>());
        json = JsonConvert.SerializeObject(entityAction);
        Assert.That(json, Is.EqualTo(expectedJson));
    }
}