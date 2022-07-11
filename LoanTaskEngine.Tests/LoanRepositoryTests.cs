using LoanTaskEngine.Actions;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Tasks;

namespace LoanTaskEngine.Tests;

public class LoanRepositoryTests
{
    [Test]
    public void EntityActionExecutions()
    {
        ITaskRepository taskRepository = new TaskRepository();
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
        taskRepository.AddTask(task);
        ILoanRepository loanRepository = new LoanRepository(taskRepository);

        var createLoanAction = new CreateLoanAction("loan1");
        var loan = createLoanAction.Execute(loanRepository);
        Assert.That(loan.Id, Is.EqualTo(createLoanAction.LoanIdentifier));
        var foundLoan = loanRepository.GetLoan(createLoanAction.LoanIdentifier);
        Assert.That(foundLoan, Is.SameAs(loan));
        Assert.That(loan.TaskStatuses, Is.Empty);

        Assert.That(loan.Borrowers, Is.Empty);
        var createBorrowerAction = new CreateBorrowerAction(createLoanAction.LoanIdentifier, "borr1");
        var borrower = createBorrowerAction.Execute(loanRepository);
        Assert.That(borrower.Id, Is.EqualTo(createBorrowerAction.BorrowerIdentifier));
        Assert.That(borrower.LoanId, Is.EqualTo(createBorrowerAction.LoanIdentifier));
        Assert.That(loan.Borrowers, Has.Count.EqualTo(1));
        Assert.That(loan.Borrowers[0], Is.SameAs(borrower));
        var foundBorrower = loanRepository.GetBorrower(createBorrowerAction.BorrowerIdentifier);
        Assert.That(foundBorrower, Is.SameAs(borrower));

        Assert.That(loan.LoanAmount, Is.Null);
        var setLoanFieldAction = new SetLoanFieldAction(createLoanAction.LoanIdentifier, "loanAmount", 100_000);
        setLoanFieldAction.Execute(loanRepository);
        Assert.That(loan.LoanAmount, Is.EqualTo(setLoanFieldAction.Value));
        Assert.That(loan.TaskStatuses, Is.Empty);

        // Task is opened when trigger conditions are met
        Assert.That(loan.LoanType, Is.Null);
        setLoanFieldAction = new SetLoanFieldAction(createLoanAction.LoanIdentifier, "loanType", "Purchase");
        setLoanFieldAction.Execute(loanRepository);
        Assert.That(loan.LoanType, Is.EqualTo(setLoanFieldAction.Value));
        Assert.That(loan.TaskStatuses, Has.Count.EqualTo(1));
        var taskStatusPair = loan.TaskStatuses.First();
        Assert.That(taskStatusPair.Key, Is.EqualTo(task.Id));
        Assert.That(taskStatusPair.Value, Is.EqualTo(EntityTaskStatus.Open));

        // Open task gets cancelled when trigger conditions are invalidated
        setLoanFieldAction = new SetLoanFieldAction(createLoanAction.LoanIdentifier, "loanType", "Refi");
        setLoanFieldAction.Execute(loanRepository);
        Assert.That(loan.LoanType, Is.EqualTo(setLoanFieldAction.Value));
        Assert.That(loan.TaskStatuses, Has.Count.EqualTo(1));
        taskStatusPair = loan.TaskStatuses.First();
        Assert.That(taskStatusPair.Key, Is.EqualTo(task.Id));
        Assert.That(taskStatusPair.Value, Is.EqualTo(EntityTaskStatus.Cancelled));

        // Task is reopened when trigger conditions are met
        setLoanFieldAction = new SetLoanFieldAction(createLoanAction.LoanIdentifier, "loanType", "Purchase");
        setLoanFieldAction.Execute(loanRepository);
        Assert.That(loan.LoanType, Is.EqualTo(setLoanFieldAction.Value));
        Assert.That(loan.TaskStatuses, Has.Count.EqualTo(1));
        taskStatusPair = loan.TaskStatuses.First();
        Assert.That(taskStatusPair.Key, Is.EqualTo(task.Id));
        Assert.That(taskStatusPair.Value, Is.EqualTo(EntityTaskStatus.Open));

        // Task is completed when completion conditions are met
        Assert.That(loan.PurchasePrice, Is.Null);
        setLoanFieldAction = new SetLoanFieldAction(createLoanAction.LoanIdentifier, "purchasePrice", 125_000);
        setLoanFieldAction.Execute(loanRepository);
        Assert.That(loan.PurchasePrice, Is.EqualTo(setLoanFieldAction.Value));
        Assert.That(loan.TaskStatuses, Has.Count.EqualTo(1));
        taskStatusPair = loan.TaskStatuses.First();
        Assert.That(taskStatusPair.Key, Is.EqualTo(task.Id));
        Assert.That(taskStatusPair.Value, Is.EqualTo(EntityTaskStatus.Completed));

        // Completed task doesn't reset to cancelled when trigger conditions are invalidated
        setLoanFieldAction = new SetLoanFieldAction(createLoanAction.LoanIdentifier, "loanType", "Refi");
        setLoanFieldAction.Execute(loanRepository);
        Assert.That(loan.LoanType, Is.EqualTo(setLoanFieldAction.Value));
        Assert.That(loan.TaskStatuses, Has.Count.EqualTo(1));
        taskStatusPair = loan.TaskStatuses.First();
        Assert.That(taskStatusPair.Key, Is.EqualTo(task.Id));
        Assert.That(taskStatusPair.Value, Is.EqualTo(EntityTaskStatus.Completed));

        Assert.That(borrower.FirstName, Is.Null);
        var setBorrowerFieldAction = new SetBorrowerFieldAction(createBorrowerAction.BorrowerIdentifier, "firstName", "Jane");
        setBorrowerFieldAction.Execute(loanRepository);
        Assert.That(borrower.FirstName, Is.EqualTo(setBorrowerFieldAction.Value));
    }
}