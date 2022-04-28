namespace Testar.ChangeDetection.Core.Settings;

public class ShowCompoundLayerSetting : SettingBase<bool>
{
    public ShowCompoundLayerSetting(ISaveLoadSettings storage)
        : base(storage, nameof(ShowCompoundLayerSetting), false) { }
}
