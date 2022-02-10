using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Strategy;

namespace BlazorApp.ViewModels;

public interface IStrategyBuilder
{
    string[] Names();

    public IChangeDetectionStrategy GetStrategyByName(string name);
}

public class AllStrats : IStrategyBuilder
{
    private readonly IEnumerable<IChangeDetectionStrategy> strategies;

    public AllStrats(IEnumerable<IChangeDetectionStrategy> strategies)
    {
        this.strategies = strategies;
    }

    public IChangeDetectionStrategy GetStrategyByName(string name)
    {
        return strategies.SingleOrDefault(x => x.Name == name) ?? throw new ArgumentException("Strategy not found", nameof(name));
    }

    public string[] Names()
    {
        return strategies.Select(x => x.Name).ToArray();
    }
}

public class ApplicationListViewModel
{
    public ApplicationViewModel[] Applications { get; set; } = Array.Empty<ApplicationViewModel>();

    public bool HasTwoSelected { get; set; }

    public bool Disable { get; set; }

    public void CountIt()
    {
        HasTwoSelected = Applications.Count(x => x.IsSelected) == 2;
        Disable = !HasTwoSelected;
    }
}

public class ApplicationViewModel
{
    private readonly ApplicationListViewModel parent;
    private bool isSelected;

    public ApplicationViewModel(ApplicationListViewModel parent)
    {
        this.parent = parent;
    }

    public string[] AbstractionAttributes { get; init; }
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
    public ModelIdentifier ModelIdentifier { get; init; }

    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            parent.CountIt();
        }
    }
}