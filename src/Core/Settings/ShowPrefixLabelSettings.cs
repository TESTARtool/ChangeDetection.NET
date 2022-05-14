namespace Testar.ChangeDetection.Core.Settings;

public class ShowPrefixLabelSettings : SettingBase<bool>
{
    public ShowPrefixLabelSettings(ISaveLoadSettings storage) :
       base(storage, nameof(ShowPrefixLabelSettings), true)
    { }
}
