using MediatR;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public enum ActionType
{
    Incomming,
    Outgoing
}

public class AbstractStateComparisonStrategy : IChangeDetectionStrategy
{
    private readonly IMediator mediator;
    private readonly IFindStateDifferences findStateDifferences;
    private readonly IHtmlOutputter htmlOutputter;

    public AbstractStateComparisonStrategy(IMediator mediator, IFindStateDifferences findStateDifferences, IHtmlOutputter htmlOutputter)
    {
        this.mediator = mediator;
        this.findStateDifferences = findStateDifferences;
        this.htmlOutputter = htmlOutputter;
    }

    public async Task ExecuteChangeDetectionAsync(Application control, Application test, IFileOutputHandler fileOutputHandler)
    {
        var abstractStatesEqual = control.AbstractionAttributes.SequenceEqual(test.AbstractionAttributes);

        if (!abstractStatesEqual)
        {
            // if Abstract Attributes are different, Abstract Layer is different and no sense to continue
            throw new AbstractAttributesNotTheSameException();
        }

        var addedStates = await findStateDifferences.FindAddedState(control, test).ToArrayAsync();
        var removedStates = await findStateDifferences.FindRemovedState(control, test).ToArrayAsync();

        await htmlOutputter.SaveOutToHtmlAsync(control, test, addedStates, removedStates);

        throw new NotImplementedException();
    }
}

public interface IHtmlOutputter
{
    Task SaveOutToHtmlAsync(Application control, Application test, DeltaState[] addedStates, DeltaState[] removedStates);
}

public class HtmlOutputter
{
    private readonly IOrientDbCommand orientDbCommand;

    public HtmlOutputter(IOrientDbCommand orientDbCommand)
    {
        this.orientDbCommand = orientDbCommand;
    }

    public async Task SaveOutToHtmlAsync(Application control, Application test, DeltaState[] addedStates, DeltaState[] removedStates, IFileOutputHandler fileOutputHandler)
    {
        var template = ResourceFiles.Get("Template.html")
            .InNamespace(typeof(HtmlOutputter).Namespace ?? "")
            .AsString();

        var disappearedStatesHtml = await ChangedStatesHtmlAsync(removedStates, "DisappearedStateTemplate.html", fileOutputHandler).ToListAsync();
        var addedAbstractStatesHtml = await ChangedStatesHtmlAsync(removedStates, "AddedStateTemplate.html", fileOutputHandler).ToListAsync();

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

    private async IAsyncEnumerable<string> ChangedStatesHtmlAsync(DeltaState[] states, string templateName, IFileOutputHandler fileOutputHandler)
    {
        var actionTemplate = "        <p style=\"color: red;\">{{ActionDescription}}</p>";

        var html = ResourceFiles.Get(templateName)
            .InNamespace(typeof(HtmlOutputter).Namespace ?? "")
            .AsString();

        foreach (var removedState in states)
        {
            // get screenshot
            var concreteState = removedState.ConcreteStates.First();
            var fileName = concreteState.ConcreteIDCustom.Value.ToString() + ".xml";
            var filePath = fileOutputHandler.GetFilePath(fileName);
            var screenshotBytes = await orientDbCommand.ExecuteDocumentAsync(concreteState.Screenshot);
            await File.WriteAllBytesAsync(filePath, screenshotBytes);

            var outgoingActions = removedState.OutgoingDeltaActions
                .Select(x => actionTemplate.Replace("{{ActionDescription}}", x.Description))
                .ToList();

            yield return html
                .Replace("{{OutgoingActions}}", string.Join(Environment.NewLine, outgoingActions))
                .Replace("{{AbstractStateScreenshot}}", fileName)
                ;
        }
    }
}