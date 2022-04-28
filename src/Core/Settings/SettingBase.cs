namespace Testar.ChangeDetection.Core.Settings;

public abstract class SettingBase<T>
{
    private readonly ISaveLoadSettings storage;
    private readonly T defaultValue;

    protected SettingBase(ISaveLoadSettings storage, string name, T defaultValue)
    {
        _ = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));

        this.storage = storage;
        this.defaultValue = defaultValue;
        Name = $"cd_{name}";
    }

    public string Name { get; }

    public T Value
    {
        get
        {
            return storage.ContainKey(Name)
              ? storage.GetItem<T>(Name)
              : defaultValue;
        }
        set
        {
            storage.SetItem(Name, value);
        }
    }

    public void SetDefault()
    {
        Value = defaultValue;
    }
}