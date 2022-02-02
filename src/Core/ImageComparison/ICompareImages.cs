namespace Testar.ChangeDetection.Core.ImageComparison;

public interface ICompareImages
{
    byte[] Comparer(string controlFileName, string testFileName);
}