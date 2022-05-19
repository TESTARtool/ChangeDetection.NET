using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

/// <summary>
/// Implements where to start finding differences
/// </summary>
public interface IStartingAbstractState
{
    (Vertex abstractState1, Vertex abstractState2) DetemineStartingStates(AppGraph graphApp1, AppGraph graphApp2);
}