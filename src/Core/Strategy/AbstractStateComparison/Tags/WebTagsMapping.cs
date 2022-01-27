namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class WebTagsMapping
{
    private static Dictionary<ITag, ITag> stateTagMappingWebdriver = new()
    {
        { StateManagementTags.WebWidgetId, WebTags.WebId },
        { StateManagementTags.WebWidgetName, WebTags.WebName },
        { StateManagementTags.WebWidgetTagName, WebTags.WebTagName },
        { StateManagementTags.WebWidgetTextContent, WebTags.WebTextContent },
        { StateManagementTags.WebWidgetTitle, WebTags.WebTitle },
        { StateManagementTags.WebWidgetHref, WebTags.WebHref },
        { StateManagementTags.WebWidgetValue, WebTags.WebValue },
        { StateManagementTags.WebWidgetStyle, WebTags.WebStyle },
        { StateManagementTags.WebWidgetTarget, WebTags.WebTarget },
        { StateManagementTags.WebWidgetAlt, WebTags.WebAlt },
        { StateManagementTags.WebWidgetType, WebTags.WebType },
        { StateManagementTags.WebWidgetCssClasses, WebTags.WebCssClasses },
        { StateManagementTags.WebWidgetDisplay, WebTags.WebDisplay },
        { StateManagementTags.WebWidgetIsOffScreen, WebTags.WebIsOffScreen },
        { StateManagementTags.WebWidgetSrc, WebTags.WebSrc },
        { StateManagementTags.WidgetControlType, WebTags.WebTagName },
        { StateManagementTags.WidgetTitle, WebTags.WebGenericTitle },
        { StateManagementTags.WidgetIsEnabled, WebTags.WebIsEnabled },
        { StateManagementTags.WidgetBoundary, WebTags.WebBoundary },
        { StateManagementTags.WidgetPath, Tags.Path },
        { StateManagementTags.WidgetIsContentElement, WebTags.WebIsContentElement },
        { StateManagementTags.WidgetIsControlElement, WebTags.WebIsControlElement },
    };

    public static ITag? GetMappedStateTag(ITag tag)
    {
        return stateTagMappingWebdriver.TryGetValue(tag, out var result)
            ? result
            : null;
    }
}