using LoanTaskEngine.Utilities;

namespace LoanTaskEngine.Actions;

public abstract class BorrowerAction : EntityAction
{
    public string BorrowerIdentifier { get; }

    public BorrowerAction(string borrowerIdentifier)
    {
        BorrowerIdentifier = Preconditions.NotNullOrEmpty(borrowerIdentifier);
    }
}