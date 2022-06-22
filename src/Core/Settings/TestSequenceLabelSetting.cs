namespace Testar.ChangeDetection.Core.Settings;

public class TestSequenceLabelSetting : SettingBase<string>
{
    public TestSequenceLabelSetting(ISaveLoadSettings storage) :
        base(storage, nameof(TestSequenceLabelSetting), "startDateTime")
    { }
}
