using Testar.ChangeDetection.Core;

namespace BlazorApp.ViewModels;

public class ApplicationViewModel
{
    public string[] AbstractionAttributes { get; init; }
    public string ApplicationName { get; init; }
    public string ApplicationVersion { get; init; }
    public ModelIdentifier ModelIdentifier { get; init; }
    public bool IsSelected { get; set; }
}