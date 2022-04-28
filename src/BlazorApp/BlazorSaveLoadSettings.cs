using Blazored.LocalStorage;
using Testar.ChangeDetection.Core.Settings;

namespace BlazorApp;

public class BlazorSaveLoadSettings : ISaveLoadSettings
{
    private readonly ISyncLocalStorageService syncLocalStorageService;

    public BlazorSaveLoadSettings(ISyncLocalStorageService syncLocalStorageService)
    {
        this.syncLocalStorageService = syncLocalStorageService;
    }

    public bool ContainKey(string name) => syncLocalStorageService.ContainKey(name);

    public T GetItem<T>(string name) => syncLocalStorageService.GetItem<T>(name);

    public void SetItem<T>(string name, T value) => syncLocalStorageService.SetItem(name, value);
}