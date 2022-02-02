namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class WebTags
{
    public static Tag<bool> WebHasKeyboardFocus = From<bool>("WebHasKeyboardFocus");
    public static Tag<bool> WebHorizontallyScrollable = From<bool>("WebHorizontallyScrollable");
    public static Tag<bool> WebIsBlocked = From<bool>("WebIsBlocked");
    public static Tag<bool> WebIsClickable = From<bool>("WebIsClickable");
    public static Tag<bool> WebIsContentElement = From<bool>("WebIsContentElement");
    public static Tag<bool> WebIsControlElement = From<bool>("WebIsControlElement");
    public static Tag<bool> WebIsEnabled = From<bool>("WebIsEnabled");
    public static Tag<bool> WebIsFullOnScreen = From<bool>("WebIsFullOnScreen");
    public static Tag<bool> WebIsKeyboardFocusable = From<bool>("WebIsKeyboardFocusable");
    public static Tag<bool> WebIsOffScreen = From<bool>("WebIsOffScreen");
    public static Tag<bool> WebIsShadow = From<bool>("WebIsShadow");
    public static Tag<bool> WebIsTopmostWindow = From<bool>("WebIsTopmostWindow");
    public static Tag<bool> WebIsWindowModal = From<bool>("WebIsWindowModal");
    public static Tag<bool> WebScrollPattern = From<bool>("WebScrollPattern");
    public static Tag<bool> WebVerticallyScrollable = From<bool>("WebVerticallyScrollable");
    public static Tag<double> WebScrollHorizontalPercent = From<double>("WebScrollHorizontalPercent");
    public static Tag<double> WebScrollHorizontalViewSize = From<double>("WebScrollHorizontalViewSize");
    public static Tag<double> WebScrollVerticalPercent = From<double>("WebScrollVerticalPercent");
    public static Tag<double> WebScrollVerticalViewSize = From<double>("WebScrollVerticalViewSize");
    public static Tag<double> WebZIndex = From<double>("WebZIndex");
    public static Tag<long[]> WebRuntimeId = From<long[]>("WebRuntimeId");
    public static Tag<long> WebControlType = From<long>("WebControlType");
    public static Tag<long> WebCulture = From<long>("WebCulture");
    public static Tag<long> WebNativeWindowHandle = From<long>("WebNativeWindowHandle");
    public static Tag<long> WebOrientation = From<long>("WebOrientation");
    public static Tag<long> WebProcessId = From<long>("WebProcessId");
    public static Tag<long> WebWindowInteractionState = From<long>("WebWindowInteractionState");
    public static Tag<long> WebWindowVisualState = From<long>("WebWindowVisualState");
    public static Tag<string> Desc = From<string>("Desc");
    public static Tag<string> WebAcceleratorKey = From<string>("WebAcceleratorKey");
    public static Tag<string> WebAccessKey = From<string>("WebAccessKey");
    public static Tag<string> WebAlt = From<string>("WebAlt");
    public static Tag<string> WebAutomationId = From<string>("WebAutomationId");
    public static Tag<string> WebBoundary = From<string>("WebBoundary");
    public static Tag<string> WebCssClasses = From<string>("WebCssClasses");
    public static Tag<string> WebDisplay = From<string>("WebDisplay");
    public static Tag<string> WebFrameworkId = From<string>("WebFrameworkId");
    public static Tag<string> WebGenericTitle = From<string>("WebGenericTitle");
    public static Tag<string> WebHelpText = From<string>("WebHelpText");
    public static Tag<string> WebHref = From<string>("WebHref");
    public static Tag<string> WebId = From<string>("WebId");
    public static Tag<string> WebItemStatus = From<string>("WebItemStatus");
    public static Tag<string> WebItemType = From<string>("WebItemType");
    public static Tag<string> WebLocalizedControlType = From<string>("WebLocalizedControlType");
    public static Tag<string> WebName = From<string>("WebName");
    public static Tag<string> WebSrc = From<string>("WebSrc");
    public static Tag<string> WebStyle = From<string>("WebStyle");
    public static Tag<string> WebTagName = From<string>("WebTagName");
    public static Tag<string> WebTarget = From<string>("WebTarget");
    public static Tag<string> WebTextContent = From<string>("WebTextContent");
    public static Tag<string> WebTitle = From<string>("WebTitle");
    public static Tag<string> WebType = From<string>("WebType");
    public static Tag<string> WebValue = From<string>("WebValue");

    public static Tag<T> From<T>(string name)
    {
        return Tag<T>.From(name);
    }
}