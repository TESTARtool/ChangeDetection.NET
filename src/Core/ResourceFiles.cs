using System.Reflection;

namespace Testar.ChangeDetection.Core;

public class ResourceFiles
{
    private readonly string name;
    private string @namespace;
    private Assembly assembly;

    private ResourceFiles(string name)
    {
        this.name = name;
        this.assembly = Assembly.GetExecutingAssembly();
        this.@namespace = string.Empty;
    }

    public static ResourceFiles Get(string name) => new(name);

    public ResourceFiles InNamespace(string @namespace)
    {
        this.@namespace = @namespace;
        return this;
    }

    public ResourceFiles InAssembly(Assembly assembly)
    {
        this.assembly = assembly;
        return this;
    }

    public Stream AsStream()
    {
        var fileName = CreateFileName();
        var stream = assembly
            .GetManifestResourceStream(fileName);

        if (stream is null)
        {
            var files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            throw new FileNotFoundException($"'{fileName}' was not found, the following files are available: {string.Join(Environment.NewLine, files)}");
        }

        return stream;
    }

    public byte[] AsBytes()
    {
        using var stream = AsStream();
        using var memoryStream = new MemoryStream();

        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public string AsString()
    {
        using var stream = AsStream();
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    private string CreateFileName()
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            return name;
        }

        return $"{@namespace}.{name}"
            .Replace("..", ".");
    }
}