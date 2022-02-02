using System.Drawing;

namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class UIATags
{
    public static Tag<bool> UIADragIsGrabbed = From<bool>("UIADragIsGrabbed");

    public static Tag<bool> UIAHasKeyboardFocus = From<bool>("UIAHasKeyboardFocus");

    public static Tag<bool> UIAHorizontallyScrollable = From<bool>("UIAHorizontallyScrollable");

    public static Tag<bool> UIAIsAnnotationPatternAvailable = From<bool>("UIAIsAnnotationPatternAvailable");

    public static Tag<bool> UIAIsContentElement = From<bool>("UIAIsContentElement");

    public static Tag<bool> UIAIsControlElement = From<bool>("UIAIsControlElement");

    public static Tag<bool> UIAIsDataValidForForm = From<bool>("UIAIsDataValidForForm");

    public static Tag<bool> UIAIsDialog = From<bool>("UIAIsDialog");

    public static Tag<bool> UIAIsDockPatternAvailable = From<bool>("UIAIsDockPatternAvailable");

    public static Tag<bool> UIAIsDragPatternAvailable = From<bool>("UIAIsDragPatternAvailable");

    public static Tag<bool> UIAIsDropTargetPatternAvailable = From<bool>("UIAIsDropTargetPatternAvailable");

    public static Tag<bool> UIAIsEnabled = From<bool>("UIAIsEnabled");

    public static Tag<bool> UIAIsExpandCollapsePatternAvailable = From<bool>("UIAIsExpandCollapsePatternAvailable");

    public static Tag<bool> UIAIsGridItemPatternAvailable = From<bool>("UIAIsGridItemPatternAvailable");

    public static Tag<bool> UIAIsGridPatternAvailable = From<bool>("UIAIsGridPatternAvailable");

    public static Tag<bool> UIAIsInvokePatternAvailable = From<bool>("UIAIsInvokePatternAvailable");

    public static Tag<bool> UIAIsItemContainerPatternAvailable = From<bool>("UIAIsItemContainerPatternAvailable");

    public static Tag<bool> UIAIsKeyboardFocusable = From<bool>("UIAIsKeyboardFocusable");

    public static Tag<bool> UIAIsLegacyIAccessiblePatternAvailable = From<bool>("UIAIsLegacyIAccessiblePatternAvailable");

    public static Tag<bool> UIAIsMultipleViewPatternAvailable = From<bool>("UIAIsMultipleViewPatternAvailable");

    public static Tag<bool> UIAIsobjectModelPatternAvailable = From<bool>("UIAIsobjectModelPatternAvailable");

    public static Tag<bool> UIAIsOffscreen = From<bool>("UIAIsOffscreen");

    public static Tag<bool> UIAIsPassword = From<bool>("UIAIsPassword");

    public static Tag<bool> UIAIsPeripheral = From<bool>("UIAIsPeripheral");

    public static Tag<bool> UIAIsRangeValuePatternAvailable = From<bool>("UIAIsRangeValuePatternAvailable");

    public static Tag<bool> UIAIsRequiredForForm = From<bool>("UIAIsRequiredForForm");

    public static Tag<bool> UIAIsScrollItemPatternAvailable = From<bool>("UIAIsScrollItemPatternAvailable");

    public static Tag<bool> UIAIsScrollPatternAvailable = From<bool>("UIAIsScrollPatternAvailable");

    public static Tag<bool> UIAIsSelectionItemPatternAvailable = From<bool>("UIAIsSelectionItemPatternAvailable");

    public static Tag<bool> UIAIsSelectionPatternAvailable = From<bool>("UIAIsSelectionPatternAvailable");

    public static Tag<bool> UIAIsSpreadsheetItemPatternAvailable = From<bool>("UIAIsSpreadsheetItemPatternAvailable");

    public static Tag<bool> UIAIsSpreadsheetPatternAvailable = From<bool>("UIAIsSpreadsheetPatternAvailable");

    public static Tag<bool> UIAIsStylesPatternAvailable = From<bool>("UIAIsStylesPatternAvailable");

    public static Tag<bool> UIAIsSynchronizedInputPatternAvailable = From<bool>("UIAIsSynchronizedInputPatternAvailable");

    public static Tag<bool> UIAIsTableItemPatternAvailable = From<bool>("UIAIsTableItemPatternAvailable");

    public static Tag<bool> UIAIsTablePatternAvailable = From<bool>("UIAIsTablePatternAvailable");

    public static Tag<bool> UIAIsTextChildPatternAvailable = From<bool>("UIAIsTextChildPatternAvailable");

    public static Tag<bool> UIAIsTextPattern2Available = From<bool>("UIAIsTextPattern2Available");

    public static Tag<bool> UIAIsTextPatternAvailable = From<bool>("UIAIsTextPatternAvailable");

    public static Tag<bool> UIAIsTogglePatternAvailable = From<bool>("UIAIsTogglePatternAvailable");

    public static Tag<bool> UIAIsTopmostWindow = From<bool>("UIAIsTopmostWindow");

    public static Tag<bool> UIAIsTransformPattern2Available = From<bool>("UIAIsTransformPattern2Available");

    public static Tag<bool> UIAIsTransformPatternAvailable = From<bool>("UIAIsTransformPatternAvailable");

    public static Tag<bool> UIAIsValuePatternAvailable = From<bool>("UIAIsValuePatternAvailable");

    public static Tag<bool> UIAIsVirtualizedItemPatternAvailable = From<bool>("UIAIsVirtualizedItemPatternAvailable");

    public static Tag<bool> UIAIsWindowModal = From<bool>("UIAIsWindowModal");

    public static Tag<bool> UIAIsWindowPatternAvailable = From<bool>("UIAIsWindowPatternAvailable");

    public static Tag<bool> UIARangeValueIsReadOnly = From<bool>("UIARangeValueIsReadOnly");

    public static Tag<bool> UIASelectionCanSelectMultiple = From<bool>("UIASelectionCanSelectMultiple");

    public static Tag<bool> UIASelectionIsSelectionRequired = From<bool>("UIASelectionIsSelectionRequired");

    public static Tag<bool> UIASelectionItemIsSelected = From<bool>("UIASelectionItemIsSelected");

    public static Tag<bool> UIATransform2CanZoom = From<bool>("UIATransform2CanZoom");

    public static Tag<bool> UIATransformCanMove = From<bool>("UIATransformCanMove");

    public static Tag<bool> UIATransformCanResize = From<bool>("UIATransformCanResize");

    public static Tag<bool> UIATransformCanRotate = From<bool>("UIATransformCanRotate");

    public static Tag<bool> UIAValueIsReadOnly = From<bool>("UIAValueIsReadOnly");

    public static Tag<bool> UIAVerticallyScrollable = From<bool>("UIAVerticallyScrollable");

    public static Tag<bool> UIAWindowCanMaximize = From<bool>("UIAWindowCanMaximize");

    public static Tag<bool> UIAWindowCanMinimize = From<bool>("UIAWindowCanMinimize");

    public static Tag<bool> UIAWindowIsModal = From<bool>("UIAWindowIsModal");

    public static Tag<bool> UIAWindowIsTopmost = From<bool>("UIAWindowIsTopmost");

    public static Tag<double> UIARangeValueLargeChange = From<double>("UIARangeValueLargeChange");

    public static Tag<double> UIARangeValueMaximum = From<double>("UIARangeValueMaximum");

    public static Tag<double> UIARangeValueMinimum = From<double>("UIARangeValueMinimum");

    public static Tag<double> UIARangeValueSmallChange = From<double>("UIARangeValueSmallChange");

    public static Tag<double> UIARangeValueValue = From<double>("UIARangeValueValue");

    public static Tag<double> UIAScrollHorizontalPercent = From<double>("UIAScrollHorizontalPercent");

    public static Tag<double> UIAScrollHorizontalViewSize = From<double>("UIAScrollHorizontalViewSize");

    public static Tag<double> UIAScrollVerticalPercent = From<double>("UIAScrollVerticalPercent");

    public static Tag<double> UIAScrollVerticalViewSize = From<double>("UIAScrollVerticalViewSize");

    public static Tag<double> UIATransform2ZoomLevel = From<double>("UIATransform2ZoomLevel");

    public static Tag<double> UIATransform2ZoomMaximum = From<double>("UIATransform2ZoomMaximum");

    public static Tag<double> UIATransform2ZoomMinimum = From<double>("UIATransform2ZoomMinimum");

    public static Tag<long[]> UIARuntimeId = From<long[]>("UIARuntimeId");

    public static Tag<long> UIAAnnotationAnnotationTypeId = From<long>("UIAAnnotationAnnotationTypeId");

    public static Tag<long> UIAAnnotationTarget = From<long>("UIAAnnotationTarget");

    public static Tag<long> UIAControlType = From<long>("UIAControlType");

    public static Tag<long> UIACulture = From<long>("UIACulture");

    public static Tag<long> UIADockDockPosition = From<long>("UIADockDockPosition ");

    public static Tag<long> UIADropTargetDropTargetEffects = From<long>("UIADropTargetDropTargetEffects");

    public static Tag<long> UIAExpandCollapseExpandCollapseState = From<long>("UIAExpandCollapseExpandCollapseState");

    public static Tag<long> UIAGridColumnCount = From<long>("UIAGridColumnCount");

    public static Tag<long> UIAGridItemColumn = From<long>("UIAGridItemColumn");

    public static Tag<long> UIAGridItemColumnSpan = From<long>("UIAGridItemColumnSpan");

    public static Tag<long> UIAGridItemContainingGrid = From<long>("UIAGridItemContainingGrid");

    public static Tag<long> UIAGridItemRow = From<long>("UIAGridItemRow");

    public static Tag<long> UIAGridItemRowSpan = From<long>("UIAGridItemRowSpan");

    public static Tag<long> UIAGridRowCount = From<long>("UIAGridRowCount");

    public static Tag<long> UIALandmarkType = From<long>("UIALandmarkType");

    public static Tag<long> UIALegacyIAccessibleChildId = From<long>("UIALegacyIAccessibleChildId");

    public static Tag<long> UIALegacyIAccessibleRole = From<long>("UIALegacyIAccessibleRole");

    public static Tag<long> UIALegacyIAccessibleState = From<long>("UIALegacyIAccessibleState");

    public static Tag<long> UIALevel = From<long>("UIALevel");

    public static Tag<long> UIALiveSetting = From<long>("UIALiveSetting");

    public static Tag<long> UIAMultipleViewCurrentView = From<long>("UIAMultipleViewCurrentView");

    public static Tag<long> UIANativeWindowHandle = From<long>("UIANativeWindowHandle");

    public static Tag<long> UIAOrientation = From<long>("UIAOrientation");

    public static Tag<long> UIAPositionInSet = From<long>("UIAPositionInSet");

    public static Tag<long> UIAProcessId = From<long>("UIAProcessId");

    public static Tag<long> UIARotation = From<long>("UIARotation");

    public static Tag<long> UIASelectionItemSelectionContainer = From<long>("UIASelectionItemSelectionContainer");

    public static Tag<long> UIASizeOfSet = From<long>("UIASizeOfSet");

    public static Tag<long> UIAStylesFillColor = From<long>("UIAStylesFillColor");

    public static Tag<long> UIAStylesFillPatternColor = From<long>("UIAStylesFillPatternColor");

    public static Tag<long> UIAStylesStyleId = From<long>("UIAStylesStyleId");

    public static Tag<long> UIATableColumnHeaders = From<long>("UIATableColumnHeaders");

    public static Tag<long> UIATableRowHeaders = From<long>("UIATableRowHeaders");

    public static Tag<long> UIATableRowOrColumnMajor = From<long>("UIATableRowOrColumnMajor");

    public static Tag<long> UIAToggleToggleState = From<long>("UIAToggleToggleState");

    public static Tag<long> UIAVisualEffects = From<long>("UIAVisualEffects");

    public static Tag<long> UIAWindowInteractionState = From<long>("UIAWindowInteractionState");

    public static Tag<long> UIAWindowVisualState = From<long>("UIAWindowVisualState");

    public static Tag<long> UIAWindowWindowInteractionState = From<long>("UIAWindowWindowInteractionState");

    public static Tag<long> UIAWindowWindowVisualState = From<long>("UIAWindowWindowVisualState");

    public static Tag<object> UIADragGrabbedItems = From<object>("UIADragGrabbedItems");

    public static Tag<object> UIALabeledBy = From<object>("UIALabeledBy");

    public static Tag<object> UIALegacyIAccessibleSelection = From<object>("UIALegacyIAccessibleSelection");

    public static Tag<object> UIASelectionSelection = From<object>("UIASelectionSelection");

    public static Tag<object> UIASpreadsheetItemAnnotationobjects = From<object>("UIASpreadsheetItemAnnotationobjects");

    public static Tag<object> UIATableItemColumnHeaderItems = From<object>("UIATableItemColumnHeaderItems");

    public static Tag<object> UIATableItemRowHeaderItems = From<object>("UIATableItemRowHeaderItems");

    public static Tag<Rectangle> UIABoundingRectangle = From<Rectangle>("UIABoundingRectangle");

    public static Tag<string> UIAAcceleratorKey = From<string>("UIAAcceleratorKey");

    public static Tag<string> UIAAccessKey = From<string>("UIAAccessKey");

    public static Tag<string> UIAAnnotationAnnotationTypeName = From<string>("UIAAnnotationAnnotationTypeName");

    public static Tag<string> UIAAnnotationAuthor = From<string>("UIAAnnotationAuthor");

    public static Tag<string> UIAAnnotationDateTime = From<string>("UIAAnnotationDateTime");

    public static Tag<string> UIAAriaProperties = From<string>("UIAAriaProperties");

    public static Tag<string> UIAAriaRole = From<string>("UIAAriaRole");

    public static Tag<string> UIAAutomationId = From<string>("UIAAutomationId");

    public static Tag<string> UIAClassName = From<string>("UIAClassName");

    public static Tag<string> UIADragDropEffect = From<string>("UIADragDropEffect");

    public static Tag<string> UIADragDropEffects = From<string>("UIADragDropEffects");

    public static Tag<string> UIADropTargetDropTargetEffect = From<string>("UIADropTargetDropTargetEffect");

    public static Tag<string> UIAFrameworkId = From<string>("UIAFrameworkId");

    public static Tag<string> UIAFullDescription = From<string>("UIAFullDescription");

    public static Tag<string> UIAHelpText = From<string>("UIAHelpText");

    public static Tag<string> UIAItemStatus = From<string>("UIAItemStatus");

    public static Tag<string> UIAItemType = From<string>("UIAItemType");

    public static Tag<string> UIALegacyIAccessibleDefaultAction = From<string>("UIALegacyIAccessibleDefaultAction");

    public static Tag<string> UIALegacyIAccessibleDescription = From<string>("UIALegacyIAccessibleDescription");

    public static Tag<string> UIALegacyIAccessibleHelp = From<string>("UIALegacyIAccessibleHelp");

    public static Tag<string> UIALegacyIAccessibleKeyboardShortcut = From<string>("UIALegacyIAccessibleKeyboardShortcut");

    public static Tag<string> UIALegacyIAccessibleName = From<string>("UIALegacyIAccessibleName");

    public static Tag<string> UIALegacyIAccessibleValue = From<string>("UIALegacyIAccessibleValue");

    public static Tag<string> UIALocalizedControlType = From<string>("UIALocalizedControlType");

    public static Tag<string> UIALocalizedLandmarkType = From<string>("UIALocalizedLandmarkType");

    public static Tag<string> UIAMultipleViewSupportedViews = From<string>("UIAMultipleViewSupportedViews");

    public static Tag<string> UIAName = From<string>("UIAName");

    public static Tag<string> UIAProviderDescription = From<string>("UIAProviderDescription");

    public static Tag<string> UIASpreadsheetItemAnnotationTypes = From<string>("UIASpreadsheetItemAnnotationTypes");

    public static Tag<string> UIASpreadsheetItemFormula = From<string>("UIASpreadsheetItemFormula");

    public static Tag<string> UIAStylesExtendedProperties = From<string>("UIAStylesExtendedProperties");

    public static Tag<string> UIAStylesFillPatternStyle = From<string>("UIAStylesFillPatternStyle");

    public static Tag<string> UIAStylesShape = From<string>("UIAStylesShape");

    public static Tag<string> UIAStylesStyleName = From<string>("UIAStylesStyleName");

    public static Tag<string> UIAValueValue = From<string>("UIAValueValue");
    public static Tag<bool> UIAIsObjectModelPatternAvailable = From<bool>("UIAIsObjectModelPatternAvailable");
    public static Tag<object> UIASpreadsheetItemAnnotationObjects = From<object>("UIASpreadsheetItemAnnotationObjects"); //array

    public static Tag<T> From<T>(string name)
    {
        return Tag<T>.From(name);
    }
}