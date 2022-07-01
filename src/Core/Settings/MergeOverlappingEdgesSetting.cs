namespace Testar.ChangeDetection.Core.Settings;

public class MergeOverlappingEdgesSetting : SettingBase<bool>
{
    public MergeOverlappingEdgesSetting(ISaveLoadSettings storage) :
     base(storage, nameof(MergeOverlappingEdgesSetting), true)
    { }
}