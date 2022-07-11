using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;

namespace LoanTaskEngine.Actions;

[JsonConverter(typeof(DefaultConverter))]
public sealed class CreateLoanAction : LoanAction
{
    public CreateLoanAction(string loanIdentifier)
        : base(loanIdentifier)
    {
    }

    public override Loan Execute(ILoanRepository loanRepository) => loanRepository.CreateLoan(LoanIdentifier);
}