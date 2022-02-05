namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public record MyTag<T>
(
    string Name,
    Type Type
)
{
    private static HashSet<MyTag<T>> existingTags = new();

    public static MyTag<T> From(string name)
    {
        var tag = new MyTag<T>(name, typeof(T));
        existingTags.Add(tag);

        return tag;
    }
}

public interface ITag
{
    string Name { get; }
    Type Type { get; }
}

public class Tag<T> : ITag
{
    private static HashSet<Tag<T>> existingTags
        = new();

    private Tag(string name)
    {
        this.Name = name;
        this.Type = typeof(T);
    }

    public string Name { get; }

    public Type Type { get; }

    public static Tag<T> From(string name)
    {
        var tag = new Tag<T>(name);
        existingTags.Add(tag);

        return tag;
    }
}