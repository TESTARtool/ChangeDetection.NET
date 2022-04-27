namespace Testar.ChangeDetection.Core.Settings;

public class AbstractStateLabelSetting : SettingBase<string>
{
    public AbstractStateLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(AbstractStateLabelSetting), "counter")
    { }
}

public class TestSequenceLabelSetting : SettingBase<string>
{
    public TestSequenceLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(TestSequenceLabelSetting), "counter")
    { }
}

public class SequenceNodeLabelSetting : SettingBase<string>
{
    public SequenceNodeLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(SequenceNodeLabelSetting), "counter")
    { }
}

public class ConcreteStateLabelSetting : SettingBase<string>
{
    public ConcreteStateLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(ConcreteStateLabelSetting), "counter")
    { }
}

public class ShowPrefixLabelSettings : SettingBase<bool>
{
    public ShowPrefixLabelSettings(ISaveLoadSettings storage) :
       base(storage, nameof(ShowPrefixLabelSettings), true)
    { }
}

// this could be the new defaults
//// var asDataName = "stateId";
//// var snDataName = "nodeNr";
//// var tsDataName = "sequenceId";
//// var csDataName = "ConcreteIDCustom";