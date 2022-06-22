namespace Testar.ChangeDetection.Core.Settings;

public class EnableCompareExperimentalFeature : SettingBase<bool>
{
    public EnableCompareExperimentalFeature(ISaveLoadSettings storage) :
       base(storage, nameof(EnableCompareExperimentalFeature), false)
    { }
}
