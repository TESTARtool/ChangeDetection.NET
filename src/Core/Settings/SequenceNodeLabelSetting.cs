namespace Testar.ChangeDetection.Core.Settings;

public class SequenceNodeLabelSetting : SettingBase<string>
{
    public SequenceNodeLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(SequenceNodeLabelSetting), "nodeNr")
    { }
}
