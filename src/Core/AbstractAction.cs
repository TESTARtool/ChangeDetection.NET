namespace Testar.ChangeDetection.Core;

public class AbstractAction
{
    public AbstractActionId AbstractActionId { get; set; }
    public ConcreteActionId ConcreteActionId { get; set; }
    public string Description { get; set; }
}