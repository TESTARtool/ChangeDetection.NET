using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Algorithm;

/// <summary>
/// Implements where to start finding differences
/// </summary>
public interface IStartingAbstractState
{
    (Vertex oldAbstractState, Vertex newAbstractState) Find(AppGraph oldGraphApp, AppGraph newGraphApp);
}