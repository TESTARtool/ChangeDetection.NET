using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Differences;

public interface ICompareVertices
{
    void CompareProperties(Vertex state1, Vertex state2);
}

public class CompareVertices : ICompareVertices
{
    public void CompareProperties(Vertex state1, Vertex state2)
    {
        // this is still to be done
    }
}