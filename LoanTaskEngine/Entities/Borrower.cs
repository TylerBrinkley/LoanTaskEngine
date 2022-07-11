using LoanTaskEngine.Utilities;
using Newtonsoft.Json;

namespace LoanTaskEngine.Entities;

public sealed class Borrower : Entity
{
    [JsonIgnore]
    public string LoanId { get; }

    public string? FirstName { get; internal set; }

    public string? LastName { get; internal set; }

    public string? Address { get; internal set; }

    public int? BirthYear { get; internal set; }

    public Borrower(string loanId, string id)
        : base(id)
    {
        LoanId = Preconditions.NotNullOrEmpty(loanId);
    }
}