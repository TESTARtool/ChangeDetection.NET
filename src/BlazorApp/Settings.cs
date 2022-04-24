using Blazored.LocalStorage;

namespace BlazorApp;

public abstract class Settings<T>
{
    private readonly ISyncLocalStorageService localStorage;
    private T? value;

    protected Settings(ISyncLocalStorageService localStorage, string name, T defaultValue)
    {
        this.localStorage = localStorage;
        Name = name;
        Default = defaultValue;
    }

    public T Default { get; }
    public string Name { get; }

    public T Value
    {
        get
        {
            if (value is not null)
            {
                return value;
            }

            return localStorage.ContainKey(Name)
              ? localStorage.GetItem<T>(Name)
              : Default;
        }
        set
        {
            localStorage.SetItem(Name, value);
            this.value = value;
        }
    }
}

public class ShowCompoundLayerSetting : Settings<bool>
{
    public ShowCompoundLayerSetting(ISyncLocalStorageService storage)
        : base(storage, "cdShowCompoundLayers", false) { }
}