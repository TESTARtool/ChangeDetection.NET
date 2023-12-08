using Testar.ChangeDetection.Core.Settings;

namespace GherkinTests.Algorithm;
internal class SaveLoadSettings : ISaveLoadSettings
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
