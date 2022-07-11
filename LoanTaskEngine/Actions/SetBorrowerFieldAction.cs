using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;

namespace LoanTaskEngine.Actions;

[JsonConverter(typeof(DefaultConverter))]
public sealed class SetBorrowerFieldAction : BorrowerAction
{
    public string Field { get; }

    public object Value { get; }

    public SetBorrowerFieldAction(string borrowerIdentifier, string field, object value)
        : base(borrowerIdentifier)
    {
        Field = Preconditions.NotNullOrEmpty(field);
        Value = value;
    }

    public override Borrower Execute(ILoanRepository loanRepository) => loanRepository.SetBorrowerField(BorrowerIdentifier, Field, Value);
}