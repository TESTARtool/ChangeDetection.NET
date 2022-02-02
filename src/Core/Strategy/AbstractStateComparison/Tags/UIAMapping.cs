﻿namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class UIAMapping
{
    private static Dictionary<ITag, ITag> stateTagMappingWindows = new()
    {
        { StateManagementTags.WidgetControlType, UIATags.UIAControlType },
        { StateManagementTags.WidgetWindowHandle, UIATags.UIANativeWindowHandle },
        { StateManagementTags.WidgetIsEnabled, UIATags.UIAIsEnabled },
        { StateManagementTags.WidgetTitle, UIATags.UIAName },
        { StateManagementTags.WidgetHelpText, UIATags.UIAHelpText },
        { StateManagementTags.WidgetAutomationId, UIATags.UIAAutomationId },
        { StateManagementTags.WidgetClassName, UIATags.UIAClassName },
        { StateManagementTags.WidgetFrameworkId, UIATags.UIAFrameworkId },
        { StateManagementTags.WidgetOrientationId, UIATags.UIAOrientation },
        { StateManagementTags.WidgetIsContentElement, UIATags.UIAIsContentElement },
        { StateManagementTags.WidgetIsControlElement, UIATags.UIAIsControlElement },
        { StateManagementTags.WidgetHasKeyboardFocus, UIATags.UIAHasKeyboardFocus },
        { StateManagementTags.WidgetIsKeyboardFocusable, UIATags.UIAIsKeyboardFocusable },
        { StateManagementTags.WidgetItemType, UIATags.UIAItemType },
        { StateManagementTags.WidgetItemStatus, UIATags.UIAItemStatus },
        { StateManagementTags.WidgetPath, Tags.Path },
        { StateManagementTags.WidgetBoundary, UIATags.UIABoundingRectangle },
        { StateManagementTags.WidgetIsOffscreen, UIATags.UIAIsOffscreen },
        { StateManagementTags.WidgetAccelatorKey, UIATags.UIAAcceleratorKey },
        { StateManagementTags.WidgetAccessKey, UIATags.UIAAccessKey },
        { StateManagementTags.WidgetAriaProperties, UIATags.UIAAriaProperties },
        { StateManagementTags.WidgetAriaRole, UIATags.UIAAriaRole },
        { StateManagementTags.WidgetIsDialog, UIATags.UIAIsDialog },
        { StateManagementTags.WidgetIsPassword, UIATags.UIAIsPassword },
        { StateManagementTags.WidgetIsPeripheral, UIATags.UIAIsPeripheral },
        { StateManagementTags.WidgetIsRequiredForForm, UIATags.UIAIsRequiredForForm },
        { StateManagementTags.WidgetLandmarkType, UIATags.UIALandmarkType },
        { StateManagementTags.WidgetGroupLevel, UIATags.UIALevel },
        { StateManagementTags.WidgetLiveSetting, UIATags.UIALiveSetting },
        { StateManagementTags.WidgetSetPosition, UIATags.UIAPositionInSet },
        { StateManagementTags.WidgetSetSize, UIATags.UIASizeOfSet },
        { StateManagementTags.WidgetRotation, UIATags.UIARotation },
        { StateManagementTags.WidgetAnnotationPattern, UIATags.UIAIsAnnotationPatternAvailable },
        { StateManagementTags.WidgetDockPattern, UIATags.UIAIsDockPatternAvailable },
        { StateManagementTags.WidgetDragPattern, UIATags.UIAIsDragPatternAvailable },
        { StateManagementTags.WidgetDropTargetPattern, UIATags.UIAIsDropTargetPatternAvailable },
        { StateManagementTags.WidgetExpandCollapsePattern, UIATags.UIAIsExpandCollapsePatternAvailable },
        { StateManagementTags.WidgetGridItemPattern, UIATags.UIAIsGridItemPatternAvailable },
        { StateManagementTags.WidgetGridPattern, UIATags.UIAIsGridPatternAvailable },
        { StateManagementTags.WidgetInvokePattern, UIATags.UIAIsInvokePatternAvailable },
        { StateManagementTags.WidgetItemContainerPattern, UIATags.UIAIsItemContainerPatternAvailable },
        { StateManagementTags.WidgetLegacyIAccessiblePattern, UIATags.UIAIsLegacyIAccessiblePatternAvailable },
        { StateManagementTags.WidgetMultipleViewPattern, UIATags.UIAIsMultipleViewPatternAvailable },
        { StateManagementTags.WidgetObjectModelPattern, UIATags.UIAIsObjectModelPatternAvailable },
        { StateManagementTags.WidgetRangeValuePattern, UIATags.UIAIsRangeValuePatternAvailable },
        { StateManagementTags.WidgetScrollItemPattern, UIATags.UIAIsScrollItemPatternAvailable },
        { StateManagementTags.WidgetScrollPattern, UIATags.UIAIsScrollPatternAvailable },
        { StateManagementTags.WidgetSelectionItemPattern, UIATags.UIAIsSelectionItemPatternAvailable },
        { StateManagementTags.WidgetSelectionPattern, UIATags.UIAIsSelectionPatternAvailable },
        { StateManagementTags.WidgetSpreadsheetPattern, UIATags.UIAIsSpreadsheetPatternAvailable },
        { StateManagementTags.WidgetSpreadsheetItemPattern, UIATags.UIAIsSpreadsheetItemPatternAvailable },
        { StateManagementTags.WidgetStylesPattern, UIATags.UIAIsStylesPatternAvailable },
        { StateManagementTags.WidgetSynchronizedInputPattern, UIATags.UIAIsSynchronizedInputPatternAvailable },
        { StateManagementTags.WidgetTableItemPattern, UIATags.UIAIsTableItemPatternAvailable },
        { StateManagementTags.WidgetTablePattern, UIATags.UIAIsTablePatternAvailable },
        { StateManagementTags.WidgetTextChildPattern, UIATags.UIAIsTextChildPatternAvailable },
        { StateManagementTags.WidgetTextPattern, UIATags.UIAIsTextPatternAvailable },
        { StateManagementTags.WidgetTextPattern2, UIATags.UIAIsTextPattern2Available },
        { StateManagementTags.WidgetTogglePattern, UIATags.UIAIsTogglePatternAvailable },
        { StateManagementTags.WidgetTransformPattern, UIATags.UIAIsTransformPatternAvailable },
        { StateManagementTags.WidgetTransformPattern2, UIATags.UIAIsTransformPattern2Available },
        { StateManagementTags.WidgetValuePattern, UIATags.UIAIsValuePatternAvailable },
        { StateManagementTags.WidgetVirtualizedItemPattern, UIATags.UIAIsVirtualizedItemPatternAvailable },
        { StateManagementTags.WidgetWindowPattern, UIATags.UIAIsWindowPatternAvailable },
        { StateManagementTags.WidgetAnnotationAnnotationTypeId, UIATags.UIAAnnotationAnnotationTypeId },
        { StateManagementTags.WidgetAnnotationAnnotationTypeName, UIATags.UIAAnnotationAnnotationTypeName },
        { StateManagementTags.WidgetAnnotationAuthor, UIATags.UIAAnnotationAuthor },
        { StateManagementTags.WidgetAnnotationDateTime, UIATags.UIAAnnotationDateTime },
        { StateManagementTags.WidgetAnnotationTarget, UIATags.UIAAnnotationTarget },
        { StateManagementTags.WidgetDockDockPosition, UIATags.UIADockDockPosition },
        { StateManagementTags.WidgetDragDropEffect, UIATags.UIADragDropEffect },
        { StateManagementTags.WidgetDragDropEffects, UIATags.UIADragDropEffects },
        { StateManagementTags.WidgetDragIsGrabbed, UIATags.UIADragIsGrabbed },
        { StateManagementTags.WidgetDragGrabbedItems, UIATags.UIADragGrabbedItems },
        { StateManagementTags.WidgetDropTargetDropTargetEffect, UIATags.UIADropTargetDropTargetEffect },
        { StateManagementTags.WidgetDropTargetDropTargetEffects, UIATags.UIADropTargetDropTargetEffects },
        { StateManagementTags.WidgetExpandCollapseExpandCollapseState, UIATags.UIAExpandCollapseExpandCollapseState },
        { StateManagementTags.WidgetGridColumnCount, UIATags.UIAGridColumnCount },
        { StateManagementTags.WidgetGridRowCount, UIATags.UIAGridRowCount },
        { StateManagementTags.WidgetGridItemColumn, UIATags.UIAGridItemColumn },
        { StateManagementTags.WidgetGridItemColumnSpan, UIATags.UIAGridItemColumnSpan },
        { StateManagementTags.WidgetGridItemContainingGrid, UIATags.UIAGridItemContainingGrid },
        { StateManagementTags.WidgetGridItemRow, UIATags.UIAGridItemRow },
        { StateManagementTags.WidgetGridItemRowSpan, UIATags.UIAGridItemRowSpan },
        { StateManagementTags.WidgetLegacyIAccessibleChildId, UIATags.UIALegacyIAccessibleChildId },
        { StateManagementTags.WidgetLegacyIAccessibleDefaultAction, UIATags.UIALegacyIAccessibleDefaultAction },
        { StateManagementTags.WidgetLegacyIAccessibleDescription, UIATags.UIALegacyIAccessibleDescription },
        { StateManagementTags.WidgetLegacyIAccessibleHelp, UIATags.UIALegacyIAccessibleHelp },
        { StateManagementTags.WidgetLegacyIAccessibleKeyboardShortcut, UIATags.UIALegacyIAccessibleKeyboardShortcut },
        { StateManagementTags.WidgetLegacyIAccessibleName, UIATags.UIALegacyIAccessibleName },
        { StateManagementTags.WidgetLegacyIAccessibleRole, UIATags.UIALegacyIAccessibleRole },
        { StateManagementTags.WidgetLegacyIAccessibleSelection, UIATags.UIALegacyIAccessibleSelection },
        { StateManagementTags.WidgetLegacyIAccessibleState, UIATags.UIALegacyIAccessibleState },
        { StateManagementTags.WidgetLegacyIAccessibleValue, UIATags.UIALegacyIAccessibleValue },
        { StateManagementTags.WidgetMultipleViewCurrentView, UIATags.UIAMultipleViewCurrentView },
        { StateManagementTags.WidgetMultipleViewSupportedViews, UIATags.UIAMultipleViewSupportedViews },
        { StateManagementTags.WidgetRangeValueIsReadOnly, UIATags.UIARangeValueIsReadOnly },
        { StateManagementTags.WidgetRangeValueLargeChange, UIATags.UIARangeValueLargeChange },
        { StateManagementTags.WidgetRangeValueMaximum, UIATags.UIARangeValueMaximum },
        { StateManagementTags.WidgetRangeValueMinimum, UIATags.UIARangeValueMinimum },
        { StateManagementTags.WidgetRangeValueSmallChange, UIATags.UIARangeValueSmallChange },
        { StateManagementTags.WidgetRangeValueValue, UIATags.UIARangeValueValue },
        { StateManagementTags.WidgetSelectionCanSelectMultiple, UIATags.UIASelectionCanSelectMultiple },
        { StateManagementTags.WidgetSelectionIsSelectionRequired, UIATags.UIASelectionIsSelectionRequired },
        { StateManagementTags.WidgetSelectionSelection, UIATags.UIASelectionSelection },
        { StateManagementTags.WidgetSelectionItemIsSelected, UIATags.UIASelectionItemIsSelected },
        { StateManagementTags.WidgetSelectionItemSelectionContainer, UIATags.UIASelectionItemSelectionContainer },
        { StateManagementTags.WidgetSpreadsheetItemFormula, UIATags.UIASpreadsheetItemFormula },
        { StateManagementTags.WidgetSpreadsheetItemAnnotationObjects, UIATags.UIASpreadsheetItemAnnotationObjects },
        { StateManagementTags.WidgetSpreadsheetItemAnnotationTypes, UIATags.UIASpreadsheetItemAnnotationTypes },
        { StateManagementTags.WidgetHorizontallyScrollable, UIATags.UIAHorizontallyScrollable },
        { StateManagementTags.WidgetVerticallyScrollable, UIATags.UIAVerticallyScrollable },
        { StateManagementTags.WidgetScrollHorizontalViewSize, UIATags.UIAScrollHorizontalViewSize },
        { StateManagementTags.WidgetScrollVerticalViewSize, UIATags.UIAScrollVerticalViewSize },
        { StateManagementTags.WidgetScrollHorizontalPercent, UIATags.UIAScrollHorizontalPercent },
        { StateManagementTags.WidgetScrollVerticalPercent, UIATags.UIAScrollVerticalPercent },
        { StateManagementTags.WidgetStylesExtendedProperties, UIATags.UIAStylesExtendedProperties },
        { StateManagementTags.WidgetStylesFillColor, UIATags.UIAStylesFillColor },
        { StateManagementTags.WidgetStylesFillPatternColor, UIATags.UIAStylesFillPatternColor },
        { StateManagementTags.WidgetStylesFillPatternStyle, UIATags.UIAStylesFillPatternStyle },
        { StateManagementTags.WidgetStylesShape, UIATags.UIAStylesShape },
        { StateManagementTags.WidgetStylesStyleId, UIATags.UIAStylesStyleId },
        { StateManagementTags.WidgetStylesStyleName, UIATags.UIAStylesStyleName },
        { StateManagementTags.WidgetTableColumnHeaders, UIATags.UIATableColumnHeaders },
        { StateManagementTags.WidgetTableRowHeaders, UIATags.UIATableRowHeaders },
        { StateManagementTags.WidgetTableRowOrColumnMajor, UIATags.UIATableRowOrColumnMajor },
        { StateManagementTags.WidgetTableItemColumnHeaderItems, UIATags.UIATableItemColumnHeaderItems },
        { StateManagementTags.WidgetTableItemRowHeaderItems, UIATags.UIATableItemRowHeaderItems },
        { StateManagementTags.WidgetToggleToggleState, UIATags.UIAToggleToggleState },
        { StateManagementTags.WidgetTransformCanMove, UIATags.UIATransformCanMove },
        { StateManagementTags.WidgetTransformCanResize, UIATags.UIATransformCanResize },
        { StateManagementTags.WidgetTransformCanRotate, UIATags.UIATransformCanRotate },
        { StateManagementTags.WidgetTransform2CanZoom, UIATags.UIATransform2CanZoom },
        { StateManagementTags.WidgetTransform2ZoomLevel, UIATags.UIATransform2ZoomLevel },
        { StateManagementTags.WidgetTransform2ZoomMaximum, UIATags.UIATransform2ZoomMaximum },
        { StateManagementTags.WidgetTransform2ZoomMinimum, UIATags.UIATransform2ZoomMinimum },
        { StateManagementTags.WidgetValueIsReadOnly, UIATags.UIAValueIsReadOnly },
        { StateManagementTags.WidgetValueValue, UIATags.UIAValueValue },
        { StateManagementTags.WidgetWindowCanMaximize, UIATags.UIAWindowCanMaximize },
        { StateManagementTags.WidgetWindowCanMinimize, UIATags.UIAWindowCanMinimize },
        { StateManagementTags.WidgetWindowIsModal, UIATags.UIAWindowIsModal },
        { StateManagementTags.WidgetWindowIsTopmost, UIATags.UIAWindowIsTopmost },
        { StateManagementTags.WidgetWindowWindowInteractionState, UIATags.UIAWindowWindowInteractionState },
        { StateManagementTags.WidgetWindowWindowVisualState, UIATags.UIAWindowWindowVisualState },
    };

    public static ITag? GetMappedStateTag(ITag mappedTag)
    {
        return stateTagMappingWindows.TryGetValue(mappedTag, out var result)
            ? result
            : null;
    }
}