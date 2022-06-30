namespace BlazorApp.ViewModels;

public class TitleAndProperties
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<KeyValuePair<string, object>> Properties { get; set; }
}