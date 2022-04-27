namespace Testar.ChangeDetection.Core.Settings;

public interface ISaveLoadSettings
{
    T GetItem<T>(string name);

    bool ContainKey(string name);

    void SetItem<T>(string name, T value);
}
