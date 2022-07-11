using System.Collections.ObjectModel;

namespace LoanTaskEngine.Entities;

public sealed class Loan : Entity
{
    internal readonly List<Borrower> _borrowers = new();

    // Made properties internally set so changes must go through the LoanRepository to ensure tasks are evaluated
    public decimal? LoanAmount { get; internal set; }

    public string? LoanType { get; internal set; }

    public decimal? PurchasePrice { get; internal set; }

    public string? PropertyAddress { get; internal set; }

    public IReadOnlyList<Borrower> Borrowers { get; }

    public Loan(string id)
        : base(id)
    {
        Borrowers = new ReadOnlyCollection<Borrower>(_borrowers);
    }
}