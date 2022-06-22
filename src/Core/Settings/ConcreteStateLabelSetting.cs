namespace Testar.ChangeDetection.Core.Settings;

public class ConcreteStateLabelSetting : SettingBase<string>
{
    public ConcreteStateLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(ConcreteStateLabelSetting), "ConcreteIDCustom")
    { }
}
