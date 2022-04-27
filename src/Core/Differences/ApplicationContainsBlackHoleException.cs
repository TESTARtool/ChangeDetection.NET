namespace Testar.ChangeDetection.Core.Differences;

public class ApplicationContainsBlackHoleException : ComparisonException
{
    public ApplicationContainsBlackHoleException(Model model)
        : base($"Application '{model.Name}','{model.Version}' contains a black hole")
    {
    }
}