using Testar.ChangeDetection.Core.Settings;

namespace Testar.ChangeDetection.ConsoleApp;

internal class ConsoleSaveLoadSettings : ISaveLoadSettings
{
    public bool ContainKey(string name)
    {
        return false;
    }

    public T GetItem<T>(string name)
    {
        return default(T);
    }

    public void SetItem<T>(string name, T value)
    {
    }
}