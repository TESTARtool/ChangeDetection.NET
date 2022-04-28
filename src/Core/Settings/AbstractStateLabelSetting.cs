namespace Testar.ChangeDetection.Core.Settings;

public class AbstractStateLabelSetting : SettingBase<string>
{
    public AbstractStateLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(AbstractStateLabelSetting), "stateId")
    { }
}

public class TestSequenceLabelSetting : SettingBase<string>
{
    public TestSequenceLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(TestSequenceLabelSetting), "startDateTime")
    { }
}

public class SequenceNodeLabelSetting : SettingBase<string>
{
    public SequenceNodeLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(SequenceNodeLabelSetting), "nodeNr")
    { }
}

public class ConcreteStateLabelSetting : SettingBase<string>
{
    public ConcreteStateLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(ConcreteStateLabelSetting), "ConcreteIDCustom")
    { }
}

public class ShowPrefixLabelSettings : SettingBase<bool>
{
    public ShowPrefixLabelSettings(ISaveLoadSettings storage) :
       base(storage, nameof(ShowPrefixLabelSettings), true)
    { }
}

public class EnableCompareExperimentalFeature : SettingBase<bool>
{
    public EnableCompareExperimentalFeature(ISaveLoadSettings storage) :
       base(storage, nameof(EnableCompareExperimentalFeature), true)
    { }
}