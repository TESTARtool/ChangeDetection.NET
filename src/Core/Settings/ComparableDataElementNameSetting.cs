namespace Testar.ChangeDetection.Core.Settings;

public class ComparableDataElementNameSetting : SettingBase<string>
{
    public ComparableDataElementNameSetting(ISaveLoadSettings storage) :
        base(storage, nameof(ComparableDataElementNameSetting), "actionId")
    { }
}