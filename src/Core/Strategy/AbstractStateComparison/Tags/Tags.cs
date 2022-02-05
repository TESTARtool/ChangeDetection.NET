namespace Testar.ChangeDetection.Core.Strategy.AbstractStateComparison;

public class Tags
{
    public static Tag<bool> Blocked = From<bool>("Blocked");
    public static Tag<bool> Enabled = From<bool>("Enabled");
    public static Tag<bool> Foreground = From<bool>("Foreground");
    public static Tag<bool> IsRunning = From<bool>("IsRunning");
    public static Tag<bool> Modal = From<bool>("Modal");
    public static Tag<bool> Rendered = From<bool>("Rendered");
    public static Tag<double> Angle = From<double>("Angle");
    public static Tag<double> MaxZIndex = From<double>("MaxZIndex");
    public static Tag<double> MinZIndex = From<double>("MinZIndex");
    public static Tag<double> ZIndex = From<double>("ZIndex");
    public static Tag<long> HANDLE = From<long>("HANDLE");
    public static Tag<long> HWND = From<long>("HWND");
    public static Tag<long> PID = From<long>("PID");
    public static Tag<long> TimeStamp = From<long>("TimeStamp");
    public static Tag<string> Desc = From<string>("Desc");
    public static Tag<string> Path = From<string>("Path");
    public static Tag<string> ScreenshotPath = From<string>("ScreenshotPath");
    public static Tag<string> TargetID = From<string>("TargetID");
    public static Tag<string> Text = From<string>("Text");
    public static Tag<string> Title = From<string>("Title");
    public static Tag<string> ToolTipText = From<string>("ToolTipText");
    public static Tag<string> ValuePattern = From<string>("ValuePattern");

    public static Tag<T> From<T>(string name)
    {
        return Tag<T>.From(name);
    }
}