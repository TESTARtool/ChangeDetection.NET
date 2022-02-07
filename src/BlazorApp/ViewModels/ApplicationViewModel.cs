using System.ComponentModel.DataAnnotations;
using Testar.ChangeDetection.Core;

namespace BlazorApp.ViewModels;

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