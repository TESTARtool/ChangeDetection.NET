namespace Testar.ChangeDetection.Core.Settings;

public class ShowOrientDbElementData : SettingBase<bool>
{
    public ShowOrientDbElementData(ISaveLoadSettings storage) :
       base(storage, nameof(ShowOrientDbElementData), false)
    { }
}