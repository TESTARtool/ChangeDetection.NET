namespace BlazorApp;

public class ChangedProperty
{
    public string Key { get; set; }
    public string New { get; set; }
    public string Old { get; set; }

    public KeyValuePair<string, object> NewValue => new KeyValuePair<string, object>(Key, New);
    public KeyValuePair<string, object> OldValue => new KeyValuePair<string, object>(Key, Old);
}