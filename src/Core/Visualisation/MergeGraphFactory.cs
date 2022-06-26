using System.Diagnostics;
using System.Text.Json;
using Testar.ChangeDetection.Core.Algorithm;
using Testar.ChangeDetection.Core.Graph;

namespace Testar.ChangeDetection.Core.Visualisation;

public interface IMergeGraphFactory
{
    AppGraph Create(CompareResults compareResults);
}

public class MergeGraphFactory : IMergeGraphFactory
{
    public AppGraph Create(CompareResults compareResults)
    {
        var oldGraph = compareResults.OldGraphApp.Elements.ToList();
        var newGraph = compareResults.NewGraphApp.Elements.ToList();

        CleanUpData(oldGraph);
        CleanUpData(newGraph);

        // create a lookup table with the new and old Ids for each corresponding states
        var newIds = CreateNewIdsList(newGraph);

        // First all nodes and edges from Gn are added to Gm
        var mergeGraph = new List<GraphElement>();
        foreach (var item in newGraph)
        {
            item.AddClass("NewVersion");
            if (item.IsAbstractState)
            {
                if (item.Document.Properties.ContainsKey("CD_CorrespondingId"))
                {
                    item.AddClass("OldVersion");
                    item.AddClass("Match");
                }
                else
                {
                    item.AddClass("New");
                }

                // for now only add the Abstract state and actions
                mergeGraph.Add(item);
            }

            if (item.IsAbstractAction)
            {
                if (item.IsHandeld)
                {
                    item.AddClass("OldVersion");
                    item.AddClass("Match");
                }
                else
                {
                    item.AddClass("New");
                }

                // for now only add the Abstract state and actions
                mergeGraph.Add(item);
            }
        }

        // Then all non-matching nodes from Go are added to Gm
        var oldIds = new Dictionary<string, string>();
        var nodes = oldGraph.Where(x => x.IsAbstractState);
        foreach (var node in nodes)
        {
            if (node.Document.Properties.ContainsKey("CD_CorrespondingId"))
            {
                // this is a matching node, skip but recorde Id because we need to wire it later
                oldIds.Add(node.Document.Id, node.Document["stateId"].Value);
            }
            else
            {
                // this is a non-matching node. Add to merge Graph
                node.AddClass("Removed");
                node.AddClass("OldVersion");
                mergeGraph.Add(node);
            }
        }

        // Finally, all edges from Go are added to Gm and wired
        var oldEdges = oldGraph.Where(x => x.IsAbstractAction);

        foreach (var edge in oldEdges)
        {
            if (edge.Document.TargetId is null || edge.Document.SourceId is null)
            {
                throw new InvalidOperationException("Either TargetId or SourceId is null");
            }

            // we need to wired non exsiting target and sources to existing Gn Ids
            if (oldIds.TryGetValue(edge.Document.TargetId, out var stateId))
            {
                if (!newIds.ContainsKey(stateId))
                {
                    Debugger.Break();
                }

                edge.Document.TargetId = newIds[stateId] ?? throw new InvalidOperationException($"Id is missing here: '{stateId}'");
            }

            if (oldIds.TryGetValue(edge.Document.SourceId, out var stateIdForSourceId))
            {
                if (!newIds.ContainsKey(stateIdForSourceId))
                {
                    Debugger.Break();
                }

                edge.Document.SourceId = newIds[stateIdForSourceId] ?? throw new InvalidOperationException($"Id is missing here: '{stateId}'");
            }

            mergeGraph.Add(edge);
        }

        try
        {
            var json = JsonSerializer.Serialize(mergeGraph);
        }
        catch (Exception ex)
        {
            var x = ex.Message;
        }
        return new AppGraph(mergeGraph);
    }

    /// <summary>
    /// Returns a list with old state id (from the correspondingId) with the new node id
    /// </summary>
    /// <param name="newGraph"></param>
    /// <returns></returns>
    private static Dictionary<string, string> CreateNewIdsList(List<GraphElement> newGraph)
    {
        var dic = new Dictionary<string, string>();

        var states = newGraph
            .Where(x => x.IsAbstractState && x.Document.Properties.ContainsKey("CD_CorrespondingId"));

        foreach (var item in states)
        {
            var oldStateId = item["CD_CorrespondingId"].Value;

            if (dic.ContainsKey(oldStateId))
            {
                Debugger.Break();
            }

            dic.Add(oldStateId, item.Document.Id);
        }

        return dic;
    }

    private static void CleanUpData(List<GraphElement> mergeGraph)
    {
        foreach (var element in mergeGraph.ToList())
        {
            element.Document.Properties.Remove("parent");

            if (element.Classes.Contains("Parent"))
            {
                mergeGraph.Remove(element);
            }
        }
    }
}