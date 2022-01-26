namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class StateManagementTags
{
    // a widget's control type
    public static Tag<string> WidgetControlType = Tag<string>.From("Widget control type");

    // the internal handle to a widget's window
    public static Tag<long> WidgetWindowHandle = Tag<long>.From("Widget window handle");

    // is the widget enabled?
    public static Tag<bool> WidgetIsEnabled = Tag<bool>.From("Widget is enabled");

    // the widget's title
    public static Tag<string> WidgetTitle = Tag<string>.From("Widget title");

    // a help text string that may be set for the widget
    public static Tag<string> WidgetHelpText = Tag<string>.From("Widget helptext");

    // the automation id for a particular version of a widget's application
    public static Tag<string> WidgetAutomationId = Tag<string>.From("Widget automation id");

    public static Tag<object> WidgetSpreadsheetItemAnnotationObjects = Tag<object>.From("WidgetSpreadsheetItemAnnotationObjects"); //array

    // the widget's classname
    public static Tag<string> WidgetClassName = Tag<string>.From("Widget class name");

    // // identifier for the framework that this widget belongs to (swing, flash, wind32, etc..)
    public static Tag<string> WidgetFrameworkId = Tag<string>.From("Widget framework id");

    // identifier for the orientation that the widget may or may not have
    public static Tag<string> WidgetOrientationId = Tag<string>.From("Widget orientation id");

    // is the widget a content element?
    public static Tag<bool> WidgetIsContentElement = Tag<bool>.From("Widget is a content element");

    // is the widget a control element?
    public static Tag<bool> WidgetIsControlElement = Tag<bool>.From("Widget is a control element");

    // the widget currently has keyboard focus
    public static Tag<bool> WidgetHasKeyboardFocus = Tag<bool>.From("Widget has keyboard focus");

    // it is possible for the widget to receive keyboard focus
    public static Tag<bool> WidgetIsKeyboardFocusable = Tag<bool>.From("Widget can have keyboard focus");

    // the item type of the widget
    public static Tag<string> WidgetItemType = Tag<string>.From("Widget item type");

    // a string describing the item status of the widget
    public static Tag<string> WidgetItemStatus = Tag<string>.From("Widget item status");

    // the path in the widget tree that leads to the widget
    public static Tag<string> WidgetPath = Tag<string>.From("Path to the widget");

    // the on-screen boundaries for the widget (coordinates)
    public static Tag<string> WidgetBoundary = Tag<string>.From("Widget on-screen boundaries");

    // is the widget off-screen?
    public static Tag<bool> WidgetIsOffscreen = Tag<bool>.From("Widget is off-screen");

    // accelator key combinations for the widget
    public static Tag<string> WidgetAccelatorKey = Tag<string>.From("Widget accelator key");

    // access key that will trigger the widget
    public static Tag<string> WidgetAccessKey = Tag<string>.From("Widget access key");

    // Aria properties of a uia element
    public static Tag<string> WidgetAriaProperties = Tag<string>.From("Widget aria properties");

    // Aria role of a UIA element
    public static Tag<string> WidgetAriaRole = Tag<string>.From("Widget aria role");

    // is the widget a dialog window?
    public static Tag<bool> WidgetIsDialog = Tag<bool>.From("Widget is a dialog windows");

    // does the widget contain password info?
    public static Tag<bool> WidgetIsPassword = Tag<bool>.From("Widget contains password info");

    // Indicated whether the Widget/UIA element represents peripheral UI.
    public static Tag<bool> WidgetIsPeripheral = Tag<bool>.From("Widget represents peripheral UI");

    // is the widget required input for a form?
    public static Tag<bool> WidgetIsRequiredForForm = Tag<bool>.From("Widget is required input for a form");

    // Is the widget/uiaelement part of a landmark/group?
    public static Tag<long> WidgetLandmarkType = Tag<long>.From("Widget element grouping");

    // the level in a hierarchical group
    public static Tag<long> WidgetGroupLevel = Tag<long>.From("Widget's level in hierarchy");

    // widget's live setting
    public static Tag<long> WidgetLiveSetting = Tag<long>.From("Widget's live setting");

    // widget's position compared to its siblings
    public static Tag<long> WidgetSetPosition = Tag<long>.From("Widget's position in sibling set");

    // the size of the set of the element and its siblings
    public static Tag<long> WidgetSetSize = Tag<long>.From("Widget's sibling set size (inclusive)");

    // the angle of the widget's rotation
    public static Tag<long> WidgetRotation = Tag<long>.From("Widget's rotation (degrees)");

    // widget pattern tags
    public static Tag<bool> WidgetAnnotationPattern = Tag<bool>.From("Widget Annotation Pattern");

    public static Tag<bool> WidgetDockPattern = Tag<bool>.From("Widget Dock Pattern");

    public static Tag<bool> WidgetDragPattern = Tag<bool>.From("Widget Drag Pattern");

    public static Tag<bool> WidgetDropTargetPattern = Tag<bool>.From("Widget DropTarget Pattern");

    public static Tag<bool> WidgetExpandCollapsePattern = Tag<bool>.From("Widget ExpandCollapse Pattern");

    public static Tag<bool> WidgetGridItemPattern = Tag<bool>.From("Widget GridItem Pattern");

    public static Tag<bool> WidgetGridPattern = Tag<bool>.From("Widget Grid Pattern");

    public static Tag<bool> WidgetInvokePattern = Tag<bool>.From("Widget Invoke Pattern");

    public static Tag<bool> WidgetItemContainerPattern = Tag<bool>.From("Widget ItemContainer Pattern");

    public static Tag<bool> WidgetLegacyIAccessiblePattern = Tag<bool>.From("Widget LegacyIAccessible Pattern");

    public static Tag<bool> WidgetMultipleViewPattern = Tag<bool>.From("Widget MultipleView Pattern");

    public static Tag<bool> WidgetObjectModelPattern = Tag<bool>.From("Widget objectModel Pattern");

    public static Tag<bool> WidgetRangeValuePattern = Tag<bool>.From("Widget RangeValue Pattern");

    public static Tag<bool> WidgetScrollItemPattern = Tag<bool>.From("Widget ScrollItem Pattern");

    public static Tag<bool> WidgetScrollPattern = Tag<bool>.From("Widget Scroll Pattern");

    public static Tag<bool> WidgetSelectionItemPattern = Tag<bool>.From("Widget SelectionItem Pattern");

    public static Tag<bool> WidgetSelectionPattern = Tag<bool>.From("Widget Selection Pattern");

    public static Tag<bool> WidgetSpreadsheetPattern = Tag<bool>.From("Widget Spreadsheet Pattern");

    public static Tag<bool> WidgetSpreadsheetItemPattern = Tag<bool>.From("Widget SpreadsheetItem Pattern");

    public static Tag<bool> WidgetStylesPattern = Tag<bool>.From("Widget Styles Pattern");

    public static Tag<bool> WidgetSynchronizedInputPattern = Tag<bool>.From("Widget SynchronizedInput Pattern");

    public static Tag<bool> WidgetTableItemPattern = Tag<bool>.From("Widget TableItem Pattern");

    public static Tag<bool> WidgetTablePattern = Tag<bool>.From("Widget Table Pattern");

    public static Tag<bool> WidgetTextChildPattern = Tag<bool>.From("Widget TextChild Pattern");

    public static Tag<bool> WidgetTextPattern = Tag<bool>.From("Widget Text Pattern");

    public static Tag<bool> WidgetTextPattern2 = Tag<bool>.From("Widget Text Pattern2");

    public static Tag<bool> WidgetTogglePattern = Tag<bool>.From("Widget Toggle Pattern");

    public static Tag<bool> WidgetTransformPattern = Tag<bool>.From("Widget Transform Pattern");

    public static Tag<bool> WidgetTransformPattern2 = Tag<bool>.From("Widget Transform Pattern2");

    public static Tag<bool> WidgetValuePattern = Tag<bool>.From("Widget Value Pattern");

    public static Tag<bool> WidgetVirtualizedItemPattern = Tag<bool>.From("Widget VirtualizedItem Pattern");

    public static Tag<bool> WidgetWindowPattern = Tag<bool>.From("Widget Window Pattern");

    // annotation pattern
    public static Tag<long> WidgetAnnotationAnnotationTypeId = Tag<long>.From("WidgetAnnotationAnnotationTypeId");

    // widget pattern property tags
    public static Tag<string> WidgetAnnotationAnnotationTypeName = Tag<string>.From("WidgetAnnotationAnnotationTypeName");

    public static Tag<string> WidgetAnnotationAuthor = Tag<string>.From("WidgetAnnotationAuthor");

    public static Tag<string> WidgetAnnotationDateTime = Tag<string>.From("WidgetAnnotationDateTime");

    public static Tag<long> WidgetAnnotationTarget = Tag<long>.From("WidgetAnnotationTarget");

    // dock pattern
    public static Tag<long> WidgetDockDockPosition = Tag<long>.From("WidgetDockDockPosition ");

    // drag control pattern
    public static Tag<string> WidgetDragDropEffect = Tag<string>.From("WidgetDragDropEffect");

    // check
    public static Tag<string> WidgetDragDropEffects = Tag<string>.From("WidgetDragDropEffects");

    // array
    public static Tag<bool> WidgetDragIsGrabbed = Tag<bool>.From("WidgetDragIsGrabbed");

    public static Tag<object> WidgetDragGrabbedItems = Tag<object>.From("WidgetDragGrabbedItems");

    // drop target control pattern
    public static Tag<string> WidgetDropTargetDropTargetEffect = Tag<string>.From("WidgetDropTargetDropTargetEffect");

    // array
    public static Tag<long> WidgetDropTargetDropTargetEffects = Tag<long>.From("WidgetDropTargetDropTargetEffects");

    // expande/collapse pattern
    public static Tag<long> WidgetExpandCollapseExpandCollapseState = Tag<long>.From("WidgetExpandCollapseExpandCollapseState");

    // array
    // grid control pattern
    public static Tag<long> WidgetGridColumnCount = Tag<long>.From("WidgetGridColumnCount");

    public static Tag<long> WidgetGridRowCount = Tag<long>.From("WidgetGridRowCount");

    // grid item control pattern
    public static Tag<long> WidgetGridItemColumn = Tag<long>.From("WidgetGridItemColumn");

    public static Tag<long> WidgetGridItemColumnSpan = Tag<long>.From("WidgetGridItemColumnSpan");

    public static Tag<long> WidgetGridItemContainingGrid = Tag<long>.From("WidgetGridItemContainingGrid");

    public static Tag<long> WidgetGridItemRow = Tag<long>.From("WidgetGridItemRow");

    public static Tag<long> WidgetGridItemRowSpan = Tag<long>.From("WidgetGridItemRowSpan");

    // LegacyIAccessible control pattern
    public static Tag<long> WidgetLegacyIAccessibleChildId = Tag<long>.From("WidgetLegacyIAccessibleChildId");

    public static Tag<string> WidgetLegacyIAccessibleDefaultAction = Tag<string>.From("WidgetLegacyIAccessibleDefaultAction");

    public static Tag<string> WidgetLegacyIAccessibleDescription = Tag<string>.From("WidgetLegacyIAccessibleDescription");

    public static Tag<string> WidgetLegacyIAccessibleHelp = Tag<string>.From("WidgetLegacyIAccessibleHelp");

    public static Tag<string> WidgetLegacyIAccessibleKeyboardShortcut = Tag<string>.From("WidgetLegacyIAccessibleKeyboardShortcut");

    public static Tag<string> WidgetLegacyIAccessibleName = Tag<string>.From("WidgetLegacyIAccessibleName");

    public static Tag<long> WidgetLegacyIAccessibleRole = Tag<long>.From("WidgetLegacyIAccessibleRole");

    public static Tag<object> WidgetLegacyIAccessibleSelection = Tag<object>.From("WidgetLegacyIAccessibleSelection");

    // list/array
    public static Tag<long> WidgetLegacyIAccessibleState = Tag<long>.From("WidgetLegacyIAccessibleState");

    public static Tag<string> WidgetLegacyIAccessibleValue = Tag<string>.From("WidgetLegacyIAccessibleValue");

    // MultipleView control pattern
    public static Tag<long> WidgetMultipleViewCurrentView = Tag<long>.From("WidgetMultipleViewCurrentView");

    public static Tag<string> WidgetMultipleViewSupportedViews = Tag<string>.From("WidgetMultipleViewSupportedViews");

    // range value control pattern
    public static Tag<bool> WidgetRangeValueIsReadOnly = Tag<bool>.From("WidgetRangeValueIsReadOnly");

    // array
    public static Tag<long> WidgetRangeValueLargeChange = Tag<long>.From("WidgetRangeValueLargeChange");

    public static Tag<long> WidgetRangeValueMaximum = Tag<long>.From("WidgetRangeValueMaximum");

    public static Tag<long> WidgetRangeValueMinimum = Tag<long>.From("WidgetRangeValueMinimum");

    public static Tag<long> WidgetRangeValueSmallChange = Tag<long>.From("WidgetRangeValueSmallChange");

    public static Tag<long> WidgetRangeValueValue = Tag<long>.From("WidgetRangeValueValue");

    // selection control pattern
    public static Tag<bool> WidgetSelectionCanSelectMultiple = Tag<bool>.From("WidgetSelectionCanSelectMultiple");

    public static Tag<bool> WidgetSelectionIsSelectionRequired = Tag<bool>.From("WidgetSelectionIsSelectionRequired");

    public static Tag<object> WidgetSelectionSelection = Tag<object>.From("WidgetSelectionSelection");

    // selection item control pattern
    public static Tag<bool> WidgetSelectionItemIsSelected = Tag<bool>.From("WidgetSelectionItemIsSelected");

    // array
    public static Tag<long> WidgetSelectionItemSelectionContainer = Tag<long>.From("WidgetSelectionItemSelectionContainer");

    // spreadsheet item control panel
    public static Tag<string> WidgetSpreadsheetItemFormula = Tag<string>.From("WidgetSpreadsheetItemFormula");

    public static Tag<object> WidgetSpreadsheetItemAnnotationobjects = Tag<object>.From("WidgetSpreadsheetItemAnnotationobjects");

    //array
    public static Tag<string> WidgetSpreadsheetItemAnnotationTypes = Tag<string>.From("WidgetSpreadsheetItemAnnotationTypes");

    // scroll pattern
    public static Tag<bool> WidgetHorizontallyScrollable = Tag<bool>.From("WidgetHorizontallyScrollable");

    // array
    public static Tag<bool> WidgetVerticallyScrollable = Tag<bool>.From("WidgetVerticallyScrollable");

    public static Tag<double> WidgetScrollHorizontalViewSize = Tag<double>.From("WidgetScrollHorizontalViewSize");

    public static Tag<double> WidgetScrollVerticalViewSize = Tag<double>.From("WidgetScrollVerticalViewSize");

    public static Tag<double> WidgetScrollHorizontalPercent = Tag<double>.From("WidgetScrollHorizontalPercent");

    public static Tag<double> WidgetScrollVerticalPercent = Tag<double>.From("WidgetScrollVerticalPercent");

    // styles control pattern
    public static Tag<string> WidgetStylesExtendedProperties = Tag<string>.From("WidgetStylesExtendedProperties");

    public static Tag<long> WidgetStylesFillColor = Tag<long>.From("WidgetStylesFillColor");

    public static Tag<long> WidgetStylesFillPatternColor = Tag<long>.From("WidgetStylesFillPatternColor");

    public static Tag<string> WidgetStylesFillPatternStyle = Tag<string>.From("WidgetStylesFillPatternStyle");

    public static Tag<string> WidgetStylesShape = Tag<string>.From("WidgetStylesShape");

    public static Tag<long> WidgetStylesStyleId = Tag<long>.From("WidgetStylesStyleId");

    public static Tag<string> WidgetStylesStyleName = Tag<string>.From("WidgetStylesStyleName");

    // table control pattern
    public static Tag<long> WidgetTableColumnHeaders = Tag<long>.From("WidgetTableColumnHeaders");

    public static Tag<long> WidgetTableRowHeaders = Tag<long>.From("WidgetTableRowHeaders");

    // array
    public static Tag<long> WidgetTableRowOrColumnMajor = Tag<long>.From("WidgetTableRowOrColumnMajor");

    // array
    // table item control panel
    public static Tag<object> WidgetTableItemColumnHeaderItems = Tag<object>.From("WidgetTableItemColumnHeaderItems");

    public static Tag<object> WidgetTableItemRowHeaderItems = Tag<object>.From("WidgetTableItemRowHeaderItems");

    // toggle control pattern
    public static Tag<long> WidgetToggleToggleState = Tag<long>.From("WidgetToggleToggleState");

    //array
    // transform pattern
    public static Tag<bool> WidgetTransformCanMove = Tag<bool>.From("WidgetTransformCanMove");

    // array
    public static Tag<bool> WidgetTransformCanResize = Tag<bool>.From("WidgetTransformCanResize");

    public static Tag<bool> WidgetTransformCanRotate = Tag<bool>.From("WidgetTransformCanRotate");

    //transform 2 pattern
    public static Tag<bool> WidgetTransform2CanZoom = Tag<bool>.From("WidgetTransform2CanZoom");

    public static Tag<long> WidgetTransform2ZoomLevel = Tag<long>.From("WidgetTransform2ZoomLevel");

    public static Tag<long> WidgetTransform2ZoomMaximum = Tag<long>.From("WidgetTransform2ZoomMaximum");

    public static Tag<long> WidgetTransform2ZoomMinimum = Tag<long>.From("WidgetTransform2ZoomMinimum");

    // Value control pattern
    public static Tag<bool> WidgetValueIsReadOnly = Tag<bool>.From("WidgetValueIsReadOnly");

    public static Tag<string> WidgetValueValue = Tag<string>.From("WidgetValueValue");

    // window control pattern
    public static Tag<bool> WidgetWindowCanMaximize = Tag<bool>.From("WidgetWindowCanMaximize");

    public static Tag<bool> WidgetWindowCanMinimize = Tag<bool>.From("WidgetWindowCanMinimize");

    public static Tag<bool> WidgetWindowIsModal = Tag<bool>.From("WidgetWindowIsModal");

    public static Tag<bool> WidgetWindowIsTopmost = Tag<bool>.From("WidgetWindowIsTopmost");

    public static Tag<long> WidgetWindowWindowInteractionState = Tag<long>.From("WidgetWindowWindowInteractionState");

    // check
    public static Tag<long> WidgetWindowWindowVisualState = Tag<long>.From("WidgetWindowWindowVisualState");

    // Web widgets
    public static Tag<string> WebWidgetId = Tag<string>.From("Web Widget id");

    // check
    public static Tag<string> WebWidgetName = Tag<string>.From("Web Widget name");

    public static Tag<string> WebWidgetTagName = Tag<string>.From("Web Widget tag name");

    public static Tag<string> WebWidgetTextContent = Tag<string>.From("Web Widget text content");

    public static Tag<string> WebWidgetTitle = Tag<string>.From("Web Widget title");

    public static Tag<string> WebWidgetHref = Tag<string>.From("Web Widget href");

    public static Tag<string> WebWidgetValue = Tag<string>.From("Web Widget value");

    public static Tag<string> WebWidgetStyle = Tag<string>.From("Web Widget style");

    public static Tag<string> WebWidgetTarget = Tag<string>.From("Web Widget target");

    public static Tag<string> WebWidgetAlt = Tag<string>.From("Web Widget alt");

    public static Tag<string> WebWidgetType = Tag<string>.From("Web Widget type");

    public static Tag<string> WebWidgetCssClasses = Tag<string>.From("Web Widget css classes");

    public static Tag<string> WebWidgetDisplay = Tag<string>.From("Web Widget display");

    public static Tag<bool> WebWidgetIsOffScreen = Tag<bool>.From("Web Widget Is Off Screen");

    public static Tag<string> WebWidgetSrc = Tag<string>.From("Web Widget src");

    private static readonly Dictionary<string, ITag> stateManagementTags = new()
    {
        //            add(WidgetWindowHandle); // this property changes between different executions of the sut
        //            WidgetIsDialog, (deactived for now, because the UIA API does not seem to recognize it)

        { nameof(WidgetControlType), WidgetControlType },
        { nameof(WidgetIsEnabled), WidgetIsEnabled },
        { nameof(WidgetTitle), WidgetTitle },
        { nameof(WidgetHelpText), WidgetHelpText },
        { nameof(WidgetAutomationId), WidgetAutomationId },
        { nameof(WidgetClassName), WidgetClassName },
        { nameof(WidgetFrameworkId), WidgetFrameworkId },
        { nameof(WidgetOrientationId), WidgetOrientationId },
        { nameof(WidgetIsContentElement), WidgetIsContentElement },
        { nameof(WidgetIsControlElement), WidgetIsControlElement },
        { nameof(WidgetHasKeyboardFocus), WidgetHasKeyboardFocus },
        { nameof(WidgetIsKeyboardFocusable), WidgetIsKeyboardFocusable },
        { nameof(WidgetItemType), WidgetItemType },
        { nameof(WidgetItemStatus), WidgetItemStatus },
        { nameof(WidgetPath), WidgetPath },
        { nameof(WidgetBoundary), WidgetBoundary },
        { nameof(WidgetAccelatorKey), WidgetAccelatorKey },
        { nameof(WidgetAccessKey), WidgetAccessKey },
        { nameof(WidgetAriaProperties), WidgetAriaProperties },
        { nameof(WidgetAriaRole), WidgetAriaRole },
        { nameof(WidgetIsPassword), WidgetIsPassword },
        { nameof(WidgetIsPeripheral), WidgetIsPeripheral },
        { nameof(WidgetIsRequiredForForm), WidgetIsRequiredForForm },
        { nameof(WidgetLandmarkType), WidgetLandmarkType },
        { nameof(WidgetGroupLevel), WidgetGroupLevel },
        { nameof(WidgetLiveSetting), WidgetLiveSetting },
        { nameof(WidgetSetPosition), WidgetSetPosition },
        { nameof(WidgetSetSize), WidgetSetSize },
        { nameof(WidgetRotation), WidgetRotation },

        { nameof(WidgetAnnotationPattern), WidgetAnnotationPattern },
        { nameof(WidgetDockPattern), WidgetDockPattern },
        { nameof(WidgetDragPattern), WidgetDragPattern },
        { nameof(WidgetDropTargetPattern), WidgetDropTargetPattern },
        { nameof(WidgetExpandCollapsePattern), WidgetExpandCollapsePattern },
        { nameof(WidgetGridItemPattern), WidgetGridItemPattern },
        { nameof(WidgetGridPattern), WidgetGridPattern },
        { nameof(WidgetInvokePattern), WidgetInvokePattern },
        { nameof(WidgetItemContainerPattern), WidgetItemContainerPattern },
        { nameof(WidgetLegacyIAccessiblePattern), WidgetLegacyIAccessiblePattern },
        { nameof(WidgetMultipleViewPattern), WidgetMultipleViewPattern },
        { nameof(WidgetObjectModelPattern), WidgetObjectModelPattern },
        { nameof(WidgetRangeValuePattern), WidgetRangeValuePattern },
        { nameof(WidgetScrollItemPattern), WidgetScrollItemPattern },
        { nameof(WidgetScrollPattern), WidgetScrollPattern },
        { nameof(WidgetSelectionItemPattern), WidgetSelectionItemPattern },
        { nameof(WidgetSelectionPattern), WidgetSelectionPattern },
        { nameof(WidgetSpreadsheetPattern), WidgetSpreadsheetPattern },
        { nameof(WidgetSpreadsheetItemPattern), WidgetSpreadsheetItemPattern },
        { nameof(WidgetStylesPattern), WidgetStylesPattern },
        { nameof(WidgetSynchronizedInputPattern), WidgetSynchronizedInputPattern },
        { nameof(WidgetTableItemPattern), WidgetTableItemPattern },
        { nameof(WidgetTablePattern), WidgetTablePattern },
        { nameof(WidgetTextChildPattern), WidgetTextChildPattern },
        { nameof(WidgetTextPattern), WidgetTextPattern },
        { nameof(WidgetTextPattern2), WidgetTextPattern2 },
        { nameof(WidgetTogglePattern), WidgetTogglePattern },
        { nameof(WidgetTransformPattern), WidgetTransformPattern },
        { nameof(WidgetTransformPattern2), WidgetTransformPattern2 },
        { nameof(WidgetValuePattern), WidgetValuePattern },
        { nameof(WidgetValueValue), WidgetValueValue },
        { nameof(WidgetVirtualizedItemPattern), WidgetVirtualizedItemPattern },
        { nameof(WidgetWindowPattern), WidgetWindowPattern },

        { nameof(WebWidgetId), WebWidgetId },
        { nameof(WebWidgetName), WebWidgetName },
        { nameof(WebWidgetTagName), WebWidgetTagName },
        { nameof(WebWidgetTextContent), WebWidgetTextContent },
        { nameof(WebWidgetTitle), WebWidgetTitle },
        { nameof(WebWidgetHref), WebWidgetHref },
        { nameof(WebWidgetValue), WebWidgetValue },
        { nameof(WebWidgetStyle), WebWidgetStyle },
        { nameof(WebWidgetTarget), WebWidgetTarget },
        { nameof(WebWidgetAlt), WebWidgetAlt },
        { nameof(WebWidgetType), WebWidgetType },
        { nameof(WebWidgetCssClasses), WebWidgetCssClasses },
        { nameof(WebWidgetDisplay), WebWidgetDisplay },
        { nameof(WebWidgetIsOffScreen), WebWidgetIsOffScreen },
        { nameof(WebWidgetSrc), WebWidgetSrc },
    };

    public static ITag? GetTagFromSettingsString(string name)
    {
        return stateManagementTags[name];
    }
}