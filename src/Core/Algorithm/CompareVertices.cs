using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface ICompareVertices
{
    void CompareProperties(Vertex state1, Vertex state2);
}

/// <summary>
/// The comparison on vertices is done by iterating of the properties of each vertext
/// If a property is found in state1 but not in state2 it is marked as removed in state2
/// if a property is found in state2 but not in state1 it is marked are added in state2
/// if a property is found in both state, than we compare the values
///     if the values are different they are marked as so
/// </summary>
public class CompareVertices : ICompareVertices
{
    public void CompareProperties(Vertex state1, Vertex state2)
    {
        var state1Properties = state1.Properties.Where(x => !x.Key.StartsWith("CD_")).ToList();
        var state2Properties = state2.Properties.Where(x => !x.Key.StartsWith("CD_")).ToList();

        foreach (var property in state2Properties)
        {
            if (state1.Properties.Any(x => x.Key == property.Key))
            {
                var value1 = state1[property.Key].Value;
                var value2 = property.Value ?? string.Empty;
                if (!value2.Equals(value1))
                {
                    state2.AddProperty($"CD_CO_{property.Key}", value1.ToString() ?? string.Empty);
                    state2.AddProperty($"CD_CN_{property.Key}", value2.ToString() ?? string.Empty);
                }
            }
            else
            {
                state2.AddProperty($"CD_A_{property.Key}", property.Value?.ToString() ?? string.Empty);
            }
        }

        foreach (var property in state1Properties)
        {
            if (!state2.Properties.Any(x => x.Key == property.Key))
            {
                state1.AddProperty($"CD_R_{property.Key}", property.Value?.ToString() ?? string.Empty);
            }
        }
    }
}