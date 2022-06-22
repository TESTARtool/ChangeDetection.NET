using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

public interface ICompareVertices
{
    void CompareProperties(Vertex oldState, Vertex newState);
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
    public void CompareProperties(Vertex oldState, Vertex newState)
    {
        var oldStateProperties = oldState.Properties.Where(x => !x.Key.StartsWith("CD_")).ToList();
        var newStateProperties = newState.Properties.Where(x => !x.Key.StartsWith("CD_")).ToList();

        foreach (var property in newStateProperties)
        {
            if (oldState.Properties.Any(x => x.Key == property.Key))
            {
                var value1 = oldState[property.Key].Value;
                var value2 = property.Value ?? string.Empty;
                if (!value2.Equals(value1))
                {
                    newState.AddProperty($"CD_CO_{property.Key}", value1.ToString() ?? string.Empty);
                    newState.AddProperty($"CD_CN_{property.Key}", value2.ToString() ?? string.Empty);
                }
            }
            else
            {
                newState.AddProperty($"CD_A_{property.Key}", property.Value?.ToString() ?? string.Empty);
            }
        }

        foreach (var property in oldStateProperties)
        {
            if (!newState.Properties.Any(x => x.Key == property.Key))
            {
                oldState.AddProperty($"CD_R_{property.Key}", property.Value?.ToString() ?? string.Empty);
            }
        }
    }
}