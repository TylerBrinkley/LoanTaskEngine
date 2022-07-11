using LoanTaskEngine.Utilities;

namespace LoanTaskEngine.Actions;

public abstract class LoanAction : EntityAction
{
    public string LoanIdentifier { get; }

    protected LoanAction(string loanIdentifier)
    {
        LoanIdentifier = Preconditions.NotNullOrEmpty(loanIdentifier);
    }
}