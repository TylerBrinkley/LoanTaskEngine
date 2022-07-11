using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Tasks;

namespace LoanTaskEngine.Tests;

public class TaskRepositoryTests
{
    [Test]
    public void Test()
    {
        ITaskRepository repo = new TaskRepository();
        Assert.That(repo.GetTasks(), Is.Empty);

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
        Assert.That(task.Id, Is.Null);
        repo.AddTask(task);
        Assert.That(task.Id, Is.Not.Null);

        var allTasks = repo.GetTasks().ToList();
        var loanTasks = repo.GetTasks(EntityType.Loan).ToList();
        var borrowerTasks = repo.GetTasks(EntityType.Borrower).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(allTasks, Has.Count.EqualTo(1));
            Assert.That(loanTasks, Has.Count.EqualTo(1));
            Assert.That(borrowerTasks, Has.Count.EqualTo(0));
            Assert.That(allTasks[0], Is.SameAs(task));
            Assert.That(loanTasks[0], Is.SameAs(task));
        });

        var foundTask = repo.GetTask(task.Id!);
        Assert.That(foundTask, Is.SameAs(task));
    }
}