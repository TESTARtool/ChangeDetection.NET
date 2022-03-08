using Microsoft.AspNetCore.Components;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Strategy;

namespace BlazorApp;

public class DiffPageStrategy : IChangeDetectionStrategy
{
    private readonly NavigationManager navigationManager;

    public DiffPageStrategy(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    public string Name => "Different page Viewer";

    public Task ExecuteChangeDetectionAsync(Model control, Model test, IFileOutputHandler fileOutputHandler)
    {
        navigationManager.NavigateTo($"/Diff/{control.ModelIdentifier.Value}/{test.ModelIdentifier.Value}");

        return Task.CompletedTask;
    }
}