using LoanTaskEngine.Entities;
using LoanTaskEngine.Repositories;
using LoanTaskEngine.Utilities;
using Newtonsoft.Json;

namespace LoanTaskEngine.Actions;

[JsonConverter(typeof(DefaultConverter))]
public sealed class CreateBorrowerAction : LoanAction
{
    public string BorrowerIdentifier { get; }

    public CreateBorrowerAction(string loanIdentifier, string borrowerIdentifier)
        : base(loanIdentifier)
    {
        BorrowerIdentifier = Preconditions.NotNullOrEmpty(borrowerIdentifier);
    }

    public override Borrower Execute(ILoanRepository loanRepository) => loanRepository.CreateBorrower(LoanIdentifier, BorrowerIdentifier);
}