using System.Windows.Media.Imaging;

using Autodesk.Revit.UI;

namespace DotBimExport;

public sealed class DotBimExportApp : IExternalApplication {
    public Result OnStartup(UIControlledApplication application) {
        const string panelName = "dotbim";

        RibbonPanel panel = application.GetRibbonPanels()
                                .FirstOrDefault(item => item.Name == panelName)
                            ?? application.CreateRibbonPanel(panelName);

        var buttonData = new PushButtonData(
            "ExportDotBim",
            "Export\nDotBim",
            typeof(DotBimExportCommand).Assembly.Location,
            typeof(DotBimExportCommand).FullName);

        var button = (PushButton) panel.AddItem(buttonData);
        button.ToolTip = "Export a 3D view to dotbim (.bim).";
        
        button.Image = LoadImage("DotBim16x16.png");
        button.LargeImage = LoadImage("DotBim32x32.png");

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) {
        return Result.Succeeded;
    }

    private static BitmapImage LoadImage(string fileName) {
        string assemblyName = typeof(DotBimExportApp).Assembly.GetName().Name;

        return new BitmapImage(new Uri(
            $"pack://application:,,,/{assemblyName};component/Resources/{fileName}", UriKind.Absolute));
    }
}