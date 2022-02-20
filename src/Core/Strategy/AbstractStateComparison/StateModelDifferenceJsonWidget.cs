using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public interface IStateModelDifferenceJsonWidget
{
    IAsyncEnumerable<WidgetJson> GetNewWidgets(DeltaState removedState, DeltaState addedState, IFileOutputHandler fileOutputHandler);

    Task<WidgetJson[]> FetchWidgetTreeInfo(ConcreteIDCustom concreteIDCustom);
}

public class StateModelDifferenceJsonWidget : IStateModelDifferenceJsonWidget
{
    private readonly IOrientDbCommandExecuter orientDbCommand;

    public StateModelDifferenceJsonWidget(IOrientDbCommandExecuter orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async IAsyncEnumerable<WidgetJson> GetNewWidgets(DeltaState removedState, DeltaState addedState, IFileOutputHandler fileOutputHandler)
    {
        var removedStateId = removedState.ConcreteStates.First().ConcreteIDCustom;
        var removedStateWidgetTree = await FetchWidgetTreeInfo(removedStateId);
        var removedStateJson = JsonConvert.SerializeObject(removedStateWidgetTree);
        var removedStatePath = fileOutputHandler.GetFilePath($"wigettree_{removedStateId.Value}.json");
        File.WriteAllText(removedStatePath, removedStateJson);

        var addedStateId = addedState.ConcreteStates.First().ConcreteIDCustom;
        var addedStateWidgetTree = await FetchWidgetTreeInfo(addedStateId);
        var addedStateJdon = JsonConvert.SerializeObject(addedStateWidgetTree);
        var addedStatePath = fileOutputHandler.GetFilePath($"wigettree_{addedStateId.Value}.json");
        File.WriteAllText(addedStatePath, addedStateJdon);

        // find differences, what is new?

        foreach (var widget in addedStateWidgetTree)
        {
            if (!widget.AbstractIDCustom.Contains("SAC"))
            {
                var removedAbstractIdCustoms = removedStateWidgetTree.Select(x => x.AbstractIDCustom);

                if (!removedAbstractIdCustoms.Contains(widget.AbstractIDCustom))
                {
                    // widget is new
                    yield return widget;
                }
            }
        }
    }

    public async Task<WidgetJson[]> FetchWidgetTreeInfo(ConcreteIDCustom concreteIDCustom)
    {
        var sql = $"SELECT FROM (TRAVERSE IN('isChildOf') FROM (SELECT FROM Widget WHERE ConcreteIDCustom = '{concreteIDCustom.Value}'))";

        var widgets = await orientDbCommand.ExecuteQueryAsync<WidgetJson>(sql);

        return widgets.ToArray();
    }
}

public class WidgetJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("Abs(R_k_T_k_P)ID")]
    public string AbsRKTKPID { get; set; }

    [JsonPropertyName("UIAControlType")]
    public int UIAControlType { get; set; }

    [JsonPropertyName("Shape")]
    public string Shape { get; set; }

    [JsonPropertyName("UIAScrollVerticalViewSize")]
    public double UIAScrollVerticalViewSize { get; set; }

    [JsonPropertyName("UIAWindowInteractionState")]
    public int UIAWindowInteractionState { get; set; }

    [JsonPropertyName("ToolTipText")]
    public string ToolTipText { get; set; }

    [JsonPropertyName("UIAIsScrollPatternAvailable")]
    public bool UIAIsScrollPatternAvailable { get; set; }

    [JsonPropertyName("UIAIsKeyboardFocusable")]
    public bool UIAIsKeyboardFocusable { get; set; }

    [JsonPropertyName("UIAScrollHorizontalViewSize")]
    public double UIAScrollHorizontalViewSize { get; set; }

    [JsonPropertyName("Desc")]
    public string Desc { get; set; }

    [JsonPropertyName("UIAHelpText")]
    public string UIAHelpText { get; set; }

    [JsonPropertyName("ZIndex")]
    public double ZIndex { get; set; }

    [JsonPropertyName("UIAWindowVisualState")]
    public int UIAWindowVisualState { get; set; }

    [JsonPropertyName("UIAIsContentElement")]
    public bool UIAIsContentElement { get; set; }

    [JsonPropertyName("UIAOrientation")]
    public int UIAOrientation { get; set; }

    [JsonPropertyName("HitTester")]
    public string HitTester { get; set; }

    [JsonPropertyName("UIAVerticallyScrollable")]
    public bool UIAVerticallyScrollable { get; set; }

    [JsonPropertyName("Path")]
    public string Path { get; set; }

    [JsonPropertyName("UIACulture")]
    public int UIACulture { get; set; }

    [JsonPropertyName("UIAIsWindowModal")]
    public bool UIAIsWindowModal { get; set; }

    [JsonPropertyName("AbstractIDCustom")]
    public string AbstractIDCustom { get; set; }

    [JsonPropertyName("UIAName")]
    public string UIAName { get; set; }

    [JsonPropertyName("UIAHorizontallyScrollable")]
    public bool UIAHorizontallyScrollable { get; set; }

    [JsonPropertyName("widgetId")]
    public string WidgetId { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; }

    [JsonPropertyName("Enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("counter")]
    public int Counter { get; set; }

    [JsonPropertyName("Abs(R)ID")]
    public string AbsRID { get; set; }

    [JsonPropertyName("ConcreteIDCustom")]
    public string ConcreteIDCustom { get; set; }

    [JsonPropertyName("Role")]
    public string Role { get; set; }

    [JsonPropertyName("UIAIsControlElement")]
    public bool UIAIsControlElement { get; set; }

    [JsonPropertyName("Abs(R_k_T)ID")]
    public string AbsRKTID { get; set; }

    [JsonPropertyName("UIAIsTopmostWindow")]
    public bool UIAIsTopmostWindow { get; set; }

    [JsonPropertyName("UIAScrollVerticalPercent")]
    public double UIAScrollVerticalPercent { get; set; }

    [JsonPropertyName("UIAHasKeyboardFocus")]
    public bool UIAHasKeyboardFocus { get; set; }

    [JsonPropertyName("ConcreteID")]
    public string ConcreteID { get; set; }

    [JsonPropertyName("UIAScrollHorizontalPercent")]
    public double UIAScrollHorizontalPercent { get; set; }

    [JsonPropertyName("Blocked")]
    public bool Blocked { get; set; }

    [JsonPropertyName("AbstractID")]
    public string AbstractID { get; set; }
}