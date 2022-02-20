using System.Diagnostics.CodeAnalysis;
using System.Text;
using Testar.ChangeDetection.Core.ImageComparison;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public interface IHtmlOutputter
{
    Task SaveOutToHtmlAsync(Application application, DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler);
}

public class HtmlOutputter : IHtmlOutputter
{
    private readonly IOrientDbCommandExecuter orientDbCommand;
    private readonly ICompareImages compareImages;
    private readonly IStateModelDifferenceJsonWidget stateModelDifferenceJsonWidget;

    public HtmlOutputter(IOrientDbCommandExecuter orientDbCommand, ICompareImages compareImages, IStateModelDifferenceJsonWidget stateModelDifferenceJsonWidget)
    {
        this.orientDbCommand = orientDbCommand;
        this.compareImages = compareImages;
        this.stateModelDifferenceJsonWidget = stateModelDifferenceJsonWidget;
    }

    public async Task SaveOutToHtmlAsync(Application application, DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler)
    {
        var template = ResourceFiles.Get("Template.html")
            .InNamespace(typeof(HtmlOutputter).Namespace ?? "")
            .AsString();

        var disappearedStatesHtml = await ChangedStatesHtmlAsync(removedStates, "DisappearedStateTemplate.html", fileOutputHandler).ToListAsync();
        var addedAbstractStatesHtml = await ChangedStatesHtmlAsync(addedStates, "AddedStateTemplate.html", fileOutputHandler).ToListAsync();

        var imageOrWidgetTreeComparison = await AddImageOrWidgetTreeComparisonAsync(application, addedStates, removedStates, fileOutputHandler);

        var html = template
            .Replace("{{NumberOfDisappearedAbstractStates}}", removedStates.Length.ToString())
            .Replace("{{NumberOfNewAbstractStates}}", addedStates.Length.ToString())
            .Replace("{{DissappearedAbstractStates}}", string.Join(Environment.NewLine, disappearedStatesHtml))
            .Replace("{{AddedAbstractStates}}", string.Join(Environment.NewLine, addedAbstractStatesHtml))
            .Replace("{{ImageOrWidgetTreeComparison}}", imageOrWidgetTreeComparison);
        ;

        var fileName = "DifferenceReport.html";
        var path = fileOutputHandler.GetFilePath(fileName);

        await File.WriteAllTextAsync(path, html);
    }

    private async Task<string> AddImageOrWidgetTreeComparisonAsync(Application application, DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler)
    {
        var tags = AbstractAttributesTags(application);

        var html = new StringBuilder();

        foreach (var addedState in addedStates)
        {
            var incommingAddedActions = addedState.IncommingDeltaActions;

            foreach (var removedState in removedStates)
            {
                var incommingRemovedActions = removedState.IncommingDeltaActions;

                var intersection = incommingAddedActions.Intersect(incommingRemovedActions, new DeltaActionComparere());

                if (intersection.Any())
                {
                    var newConcreteStateFileName = addedState.ConcreteStates.First().ConcreteIDCustom.Value;
                    var removedConcreteState = removedState.ConcreteStates.First().ConcreteIDCustom.Value;

                    var comparisonBytes = compareImages.Comparer
                        (
                        fileOutputHandler.GetFilePath(newConcreteStateFileName + ".png"),
                        fileOutputHandler.GetFilePath(removedConcreteState + ".png")
                        );

                    if (comparisonBytes.Length > 0)
                    {
                        var imageDiff = $"{newConcreteStateFileName}_diff_{removedConcreteState}.png";
                        await File.WriteAllBytesAsync(fileOutputHandler.GetFilePath(imageDiff), comparisonBytes);

                        var imgs = $"<p><img src='{newConcreteStateFileName + ".png"}' alt='control image' />" +
                            $"<img src='{removedConcreteState + ".png"}' alt='test image' />" +
                            $"<img src='{imageDiff}' alt='diff image' /></p>";

                        html.AppendLine(imgs);
                    }

                    var intersectionTexts = intersection.Select(x => $"{x.ActionId.Value}, {x.Description}");

                    var actionDesc = string.Join(",", intersectionTexts);

                    html.AppendLine($"<p style='color:blue;'>We have reached this State with Action: {actionDesc}</p>");

                    var newWidgets = await stateModelDifferenceJsonWidget.GetNewWidgets(removedState, addedState, fileOutputHandler).ToArrayAsync();

                    foreach (var widget in newWidgets)
                    {
                        html.AppendLine($"<p>This Widget is completely new in the new Model: {widget.AbstractIDCustom}</p>");
                        html.Append("<p>");

                        foreach (var tag in tags)
                        {
                            var propertyValue = typeof(WidgetJson).GetProperty(tag.Name)?.GetValue(widget);
                            if (propertyValue is not null)
                            {
                                html.Append($"{tag.Name}: {propertyValue}");
                            }
                        }

                        html.Append("</p>");

                        // html.AppendLine($"<p>UIAName:{widget.UIAName} UIAControlType:{widget.UIAControlType}</p>");
                    }
                }
            }
        }

        return html.ToString();
    }

    private string CapitalizeWordsAndRemoveSpaces(string attribute)
    {
        var words = attribute.Split(' ');
        var capitalizeWord = new StringBuilder();
        foreach (var word in words)
        {
            var fist = word.Substring(0, 1).ToUpper();
            var rest = word.Substring(1);
            capitalizeWord.Append(fist);
            capitalizeWord.Append(rest);
        }

        return capitalizeWord.ToString().Trim();
    }

    private HashSet<ITag> AbstractAttributesTags(Application control)
    {
        // Update Set object "abstractAttributesTags" with the Tags
        // we need to check for Widget Tree difference
        var mangementTags = control.AbstractionAttributes
            .Select(CapitalizeWordsAndRemoveSpaces)
            .Select(x => StateManagementTags.GetTagFromSettingsString(x))
            .Where(x => x is not null)
            .ToHashSet();

        var uiAmappings = mangementTags
            .Select(x => UIAMapping.GetMappedStateTag(x!))
            .Where(x => x is not null)
            .ToList();

        var wdMapping = mangementTags
            .Select(x => WebTagsMapping.GetMappedStateTag(x!))
            .Where(x => x is not null)
            .ToList();

        var tags = uiAmappings
            .Union(wdMapping)
            .ToHashSet();

        return tags!;
    }

    private async IAsyncEnumerable<string> ChangedStatesHtmlAsync(DeltaState[] states, string templateName, IFileOutputHandler fileOutputHandler)
    {
        var actionTemplate = "        <p style=\"color: red;\">{{ActionDescription}}</p>";

        var html = ResourceFiles.Get(templateName)
            .InNamespace(typeof(HtmlOutputter).Namespace ?? "")
            .AsString();

        foreach (var state in states)
        {
            var screenshotFileName = await SaveScreenshotAsync(state, fileOutputHandler);

            var outgoingActions = state.OutgoingDeltaActions
                .Select(x => actionTemplate.Replace("{{ActionDescription}}", x.Description))
                .ToList();

            yield return html
                .Replace("{{AbstractStateId}}", state.StateId.Value)
                .Replace("{{OutgoingActions}}", string.Join(Environment.NewLine, outgoingActions))
                .Replace("{{AbstractStateScreenshot}}", screenshotFileName)
                ;
        }
    }

    private async Task<string> SaveScreenshotAsync(DeltaState state, IFileOutputHandler fileOutputHandler)
    {
        var concreteState = state.ConcreteStates.First();
        var fileName = concreteState.ConcreteIDCustom.Value.ToString() + ".png";
        var filePath = fileOutputHandler.GetFilePath(fileName);
        var screenshotBytes = await orientDbCommand.ExecuteDocumentAsync(concreteState.Screenshot);
        await File.WriteAllBytesAsync(filePath, screenshotBytes);

        return fileName;
    }

    private class DeltaActionComparere : IEqualityComparer<DeltaAction>
    {
        public bool Equals(DeltaAction? x, DeltaAction? y)
        {
            if (x is null && y is null) return true;
            if (x is null && y is not null) return false;
            if (x is not null && y is null) return false;

            return x!.ActionId == y!.ActionId;
        }

        public int GetHashCode([DisallowNull] DeltaAction obj)
        {
            return obj.ActionId.GetHashCode();
        }
    }
}