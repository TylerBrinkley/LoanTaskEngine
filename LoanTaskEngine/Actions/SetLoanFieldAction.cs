using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;

namespace LoanTaskEngine.Actions;

[JsonConverter(typeof(DefaultConverter))]
public sealed class SetLoanFieldAction : LoanAction
{
    public string Field { get; }

    public object Value { get; }

    public SetLoanFieldAction(string loanIdentifier, string field, object value)
        : base(loanIdentifier)
    {
        Field = Preconditions.NotNullOrEmpty(field);
        Value = value;
    }

    public override Loan Execute(ILoanRepository loanRepository) => loanRepository.SetLoanField(LoanIdentifier, Field, Value);
}