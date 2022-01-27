using System.Text;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public interface IHtmlOutputter
{
    Task SaveOutToHtmlAsync(DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler);
}

public class HtmlOutputter : IHtmlOutputter
{
    private readonly IOrientDbCommand orientDbCommand;

    public HtmlOutputter(IOrientDbCommand orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async Task SaveOutToHtmlAsync(DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler)
    {
        var template = ResourceFiles.Get("Template.html")
            .InNamespace(typeof(HtmlOutputter).Namespace ?? "")
            .AsString();

        var disappearedStatesHtml = await ChangedStatesHtmlAsync(removedStates, "DisappearedStateTemplate.html", fileOutputHandler).ToListAsync();
        var addedAbstractStatesHtml = await ChangedStatesHtmlAsync(addedStates, "AddedStateTemplate.html", fileOutputHandler).ToListAsync();

        var html = template
            .Replace("{{NumberOfDisappearedAbstractStates}}", removedStates.Length.ToString())
            .Replace("{{NumberOfNewAbstractStates}}", addedStates.Length.ToString())
            .Replace("{{DissappearedAbstractStates}}", string.Join(Environment.NewLine, disappearedStatesHtml))
            .Replace("{{AddedAbstractStates}}", string.Join(Environment.NewLine, addedAbstractStatesHtml))
            ;

        var fileName = "DifferenceReport.html";
        var path = fileOutputHandler.GetFilePath(fileName);

        await File.WriteAllTextAsync(path, html);
    }

    private void AddImageOrWidgetTreeComparison(Application control, DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler)
    {
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

        var tags = uiAmappings.Union(wdMapping)
            .ToHashSet();

        return tags;
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
}