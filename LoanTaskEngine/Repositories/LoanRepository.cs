using System.Collections.Concurrent;
using LoanTaskEngine.Entities;
using LoanTaskEngine.Tasks;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json.Serialization;

namespace LoanTaskEngine.Repositories;

public sealed class LoanRepository : ILoanRepository
{
    private readonly ConcurrentDictionary<string, Loan> _loans = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Borrower> _borrowers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ITaskRepository _taskRepository;

    public LoanRepository(ITaskRepository taskRepository)
    {
        _taskRepository = Preconditions.NotNull(taskRepository);
    }

    public Loan? GetLoan(string loanId)
    {
        Preconditions.NotNullOrEmpty(loanId);

        _loans.TryGetValue(loanId, out var loan);
        return loan;
    }

    public Loan CreateLoan(string loanId)
    {
        Preconditions.NotNullOrEmpty(loanId);

        var loan = new Loan(loanId);
        if (!_loans.TryAdd(loanId, loan))
        {
            throw new ArgumentException($"Loan already exists with loanId of '{loanId}'", nameof(loanId));
        }
        return loan;
    }

    public Borrower? GetBorrower(string borrowerId)
    {
        Preconditions.NotNullOrEmpty(borrowerId);

        _borrowers.TryGetValue(borrowerId, out var borrower);
        return borrower;
    }

    public Borrower CreateBorrower(string loanId, string borrowerId)
    {
        Preconditions.NotNullOrEmpty(loanId);
        Preconditions.NotNullOrEmpty(borrowerId);

        var loan = GetLoan(loanId);
        if (loan is null)
        {
            throw new ArgumentException($"Could not find loan with loanId of '{loanId}'", nameof(loanId));
        }

        var borrower = new Borrower(loanId, borrowerId);
        if (!_borrowers.TryAdd(borrowerId, borrower))
        {
            throw new ArgumentException($"Borrower already exists with borrowerId of '{borrowerId}'", nameof(borrowerId));
        }

        loan._borrowers.Add(borrower);
        return borrower;
    }

    public Loan SetLoanField(string loanId, string field, object? value)
    {
        Preconditions.NotNullOrEmpty(loanId);
        Preconditions.NotNullOrEmpty(field);

        var loan = GetLoan(loanId);
        if (loan is null)
        {
            throw new ArgumentException($"Could not find loan with loanId of '{loanId}'", nameof(loanId));
        }

        // Make loan update
        var property = (JsonHelper.DefaultSerializer.ContractResolver.ResolveContract(typeof(Loan)) as JsonObjectContract)!.Properties.GetClosestMatchProperty(field);
        if (property is null)
        {
            throw new ArgumentException($"Could not find field '{field}' for Loan entity", nameof(field));
        }
        property.ValueProvider!.SetValue(loan, ConvertValue(value, property.PropertyType!));

        // Evaluate tasks
        EvaluateEntityTasks(loan, EntityType.Loan);

        return loan;
    }

    public Borrower SetBorrowerField(string borrowerId, string field, object? value)
    {
        Preconditions.NotNullOrEmpty(borrowerId);
        Preconditions.NotNullOrEmpty(field);

        var borrower = GetBorrower(borrowerId);
        if (borrower is null)
        {
            throw new ArgumentException($"Could not find loan with borrowerId of '{borrowerId}'", nameof(borrowerId));
        }

        // Make borrower update
        var property = (JsonHelper.DefaultSerializer.ContractResolver.ResolveContract(typeof(Borrower)) as JsonObjectContract)!.Properties.GetClosestMatchProperty(field);
        if (property is null)
        {
            throw new ArgumentException($"Could not find field '{field}' for Borrower entity", nameof(field));
        }
        property.ValueProvider!.SetValue(borrower, ConvertValue(value, property.PropertyType!));

        // Evaluate tasks
        EvaluateEntityTasks(borrower, EntityType.Borrower);

        return borrower;
    }

    private void EvaluateEntityTasks(Entity entity, EntityType entityType)
    {
        foreach (var loanTask in _taskRepository.GetTasks(entityType))
        {
            if (!entity.TaskStatuses.TryGetValue(loanTask.Id!, out var taskStatus) || taskStatus != EntityTaskStatus.Completed && loanTask.TriggerConditions.Count > 0)
            {
                if (loanTask.TriggerConditions.All(c => c.Evaluate(entity)))
                {
                    if (loanTask.CompletionConditions.Count > 0 && loanTask.CompletionConditions.All(c => c.Evaluate(entity)))
                    {
                        entity._taskStatuses[loanTask.Id!] = EntityTaskStatus.Completed;
                    }
                    else
                    {
                        entity._taskStatuses[loanTask.Id!] = EntityTaskStatus.Open;
                    }
                }
                else if (taskStatus == EntityTaskStatus.Open)
                {
                    entity._taskStatuses[loanTask.Id!] = EntityTaskStatus.Cancelled;
                }
            }
        }
    }

    private static object? ConvertValue(object? value, Type propertyType)
    {
        if (value is null)
        {
            return value;
        }
        var targetType = propertyType;
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = propertyType.GenericTypeArguments[0];
        }
        return Convert.ChangeType(value, targetType);
    }
}