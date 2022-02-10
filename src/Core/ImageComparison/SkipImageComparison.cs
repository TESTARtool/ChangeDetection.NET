namespace Testar.ChangeDetection.Core.ImageComparison;

public class SkipImageComparison : ICompareImages
{
    public byte[] Comparer(string controlFileName, string testFileName)
    {
        return Array.Empty<byte>();
    }
}
