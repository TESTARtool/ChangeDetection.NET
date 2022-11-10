namespace Testar.ChangeDetection.Core.Settings;

public partial class AbstractStateLabelSetting : SettingBase<string>
{
    public AbstractStateLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(AbstractStateLabelSetting), "stateId")
    { }
}