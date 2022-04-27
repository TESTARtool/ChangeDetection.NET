namespace Testar.ChangeDetection.Core.Settings;

public abstract class SettingBase<T>
{
    private readonly ISaveLoadSettings storage;
    private T value;

    protected SettingBase(ISaveLoadSettings storage, string name, T defaultValue)
    {
        _ = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));

        this.storage = storage;
        Name = $"cd_{name}";
        value = storage.ContainKey(Name)
            ? storage.GetItem<T>(Name)
            : defaultValue;
    }

    public string Name { get; }

    public T Value
    {
        get { return value; }
        set
        {
            if (!this.value!.Equals(value))
            {
                storage.SetItem(Name, value);
                this.value = value;
            }
        }
    }
}