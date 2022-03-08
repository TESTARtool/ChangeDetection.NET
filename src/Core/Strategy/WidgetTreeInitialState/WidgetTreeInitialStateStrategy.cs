using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace Testar.ChangeDetection.Core.Strategy.WidgetTreeInitialState;

public class WidgetTreeInitialStateStrategy : IChangeDetectionStrategy
{
    private readonly IChangeDetectionHttpClient client;

    public WidgetTreeInitialStateStrategy(IChangeDetectionHttpClient client)
    {
        this.client = client;
    }

    public string Name => "Widget Tree Initial State";

    public async Task ExecuteChangeDetectionAsync(Model control, Model test, IFileOutputHandler fileOutputHandler)
    {
        var controlConcreteId = control.AbstractStates
            .SingleOrDefault(x => x.IsInitial)
            ?.ConcreteStateIds
            ?.First() ?? throw new Exception("Control does not have a intial abstract state");

        var testConcreteId = test.AbstractStates
            .SingleOrDefault(x => x.IsInitial)
            ?.ConcreteStateIds
            ?.First() ?? throw new Exception("Test does not have a initial abstract state");

        var widgetTreeControl = await GetWidgetTreeFromConcreteStateIdAsync(controlConcreteId);
        var controlXmlFilePath = TransformToXml(widgetTreeControl, controlConcreteId, fileOutputHandler);

        var widgetTreeTest = await GetWidgetTreeFromConcreteStateIdAsync(testConcreteId);
        var testXmlFilePath = TransformToXml(widgetTreeControl, controlConcreteId, fileOutputHandler);

        var differences = Compare(controlXmlFilePath, testXmlFilePath);

        var template = ResourceFiles.Get("Template.html")
            .InNamespace(GetType().Namespace ?? "")
            .AsString();

        var differencesHtml = ParseDifferences(differences);

        throw new NotImplementedException();
    }

    private static string ParseDifferences(Difference[] differences)
    {
        const string template = "            <tr><th scope=\"row\">{0}</th><td>{1}</td><td>{2}</td></tr>{3}";

        var result = new StringBuilder();

        foreach (var diff in differences)
        {
            result.AppendFormat(
               template,
               FormatNice(diff.Comparison.ControlDetails.Target?.Name),
               FormatNice(diff.Comparison.ControlDetails.Target?.Value),
               FormatNice(diff.Comparison.TestDetails.Target?.Value),
               Environment.NewLine);
        }

        return result.ToString();
    }

    private static string FormatNice(string? value)
    {
        return (value ?? "-")
            .Replace(Environment.NewLine, "")
            .Trim();
    }

    private static Widget CreateWidgetTree(Widget root, Widget[] widgetEntities)
    {
        var childeren = root.InIsChildOf
                .SelectMany(x => widgetEntities.Where(y => y.OutIsChildOf.Contains(x)))
                .Select(x => CreateWidgetTree(x, widgetEntities))
                .ToArray();

        root.Children.AddRange(childeren);

        return root;
    }

    private static string TransformToXml(Widget root, ConcreteStateId concreteStateId, IFileOutputHandler fileHander)
    {
        var doc = new XmlDocument();
        var treeElement = doc.CreateElement("WidgetTree");
        treeElement.SetAttribute(nameof(ConcreteStateId), concreteStateId.Value);

        var widgetElement = CreateWidgetXml(doc, root);
        treeElement.AppendChild(widgetElement);

        var fileName = $"{concreteStateId.Value}.xml";
        var filePath = fileHander.GetFilePath(fileName);

        doc.Save(filePath);

        return filePath;
    }

    private static XmlElement CreateWidgetXml(XmlDocument doc, Widget widget)
    {
        var element = doc.CreateElement(nameof(Widget));
        element.SetAttribute(nameof(Widget.Role), widget.Role);
        element.SetAttribute(nameof(Widget.Title), widget.Title);

        var properties = doc.CreateElement(nameof(Widget.Properties));

        foreach (var property in widget.Properties)
        {
            var prop = doc.CreateElement(property.Key);
            prop.InnerText = property.Value;
            properties.AppendChild(prop);
        }

        element.AppendChild(properties);
        var childrenElement = doc.CreateElement("Children");
        foreach (var item in widget.Children)
        {
            var childElement = CreateWidgetXml(doc, item);
            childrenElement.AppendChild(childElement);
        }
        element.AppendChild(childrenElement);

        return element;
    }

    private static Difference[] Compare(string controlXmlPath, string testXmlPath)
    {
        var control = Input
            .FromFile(controlXmlPath)
            .Build();

        var test = Input
            .FromFile(testXmlPath)
            .Build();

        var diffb = DiffBuilder
            .Compare(control)
            .WithTest(test)
            .Build();

        return diffb.Differences.ToArray();
    }

    private async Task<Widget> GetWidgetTreeFromConcreteStateIdAsync(ConcreteStateId concreteIDCustom)
    {
        var command = new OrientDbCommand("SELECT FROM (TRAVERSE IN('isChildOf') FROM (SELECT FROM Widget WHERE ConcreteIDCustom = :concreteIDCustom))")
            .AddParameter("concreteIDCustom", concreteIDCustom.Value);

        var widgetsjson = await client.QueryAsync<JsonElement>(command);

        var widgets = new List<Widget>();

        foreach (var json in widgetsjson)
        {
            var widget = json.Deserialize<WidgetJson>();
            var letsee = json.Deserialize<Dictionary<string, object>>();
            var array = json.EnumerateArray();
            foreach (var x in array)
            {
                var yy = x.ToString();
            }
        }

        //var widgets = widgetsjson
        //    .Select(x => (x.Deserialize<WidgetJson>(), x));

        //    .Select(x => mapper.Map<Widget>(x))
        //    .ToArray();

        //var rootWidget = widgets
        //    .Where(x => x.OutIsChildOf?.Length == 0)
        //    .Single();

        //return CreateWidgetTree(rootWidget, widgets);

        throw new NotFiniteNumberException();
    }
}