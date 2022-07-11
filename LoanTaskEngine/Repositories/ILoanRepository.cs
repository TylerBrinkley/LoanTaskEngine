using LoanTaskEngine.Entities;

namespace LoanTaskEngine.Repositories;

public interface ILoanRepository
{
    Borrower CreateBorrower(string loanId, string borrowerId);
    Loan CreateLoan(string loanId);
    Borrower? GetBorrower(string borrowerId);
    Loan? GetLoan(string loanId);
    Loan SetLoanField(string loanId, string field, object? value);
    Borrower SetBorrowerField(string borrowerId, string field, object? value);
}