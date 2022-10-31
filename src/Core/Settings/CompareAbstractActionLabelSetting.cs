namespace Testar.ChangeDetection.Core.Settings;

public class CompareAbstractActionLabelSetting : SettingBase<string>
{
    public CompareAbstractActionLabelSetting(ISaveLoadSettings storage) :
    base(storage, nameof(CompareAbstractActionLabelSetting), "Desc")
    { }
}