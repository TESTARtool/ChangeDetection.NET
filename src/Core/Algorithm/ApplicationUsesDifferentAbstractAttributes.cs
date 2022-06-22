namespace Testar.ChangeDetection.Core.Algorithm;

public class ApplicationUsesDifferentAbstractAttributes : ComparisonException
{
    public ApplicationUsesDifferentAbstractAttributes(Model oldModel, Model newModel)
    : base($"Application '{oldModel.Name}','{oldModel.Version}' is using different abstract attibutes than applcation '{oldModel.Name}'.'{oldModel.Version}'")
    {
    }
}