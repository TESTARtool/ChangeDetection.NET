namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class AbstractStateComparisonStrategy : IChangeDetectionStrategy
{
    private readonly IFindStateDifferences findStateDifferences;
    private readonly IHtmlOutputter htmlOutputter;

    public AbstractStateComparisonStrategy(IFindStateDifferences findStateDifferences, IHtmlOutputter htmlOutputter)
    {
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

        await htmlOutputter.SaveOutToHtmlAsync(control, addedStates, removedStates, fileOutputHandler);
    }
}