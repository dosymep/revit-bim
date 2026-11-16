using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using dosymep.Revit.DotBim;

using DotBimExport.Resources;
using DotBimExport.ViewModels;

using Microsoft.Win32;

namespace DotBimExport;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
internal sealed class DotBimExportCommand : IExternalCommand {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
        Localization.Apply(commandData.Application.Application.Language);

        Document document = commandData.Application.ActiveUIDocument.Document;
        if(!Has3DViews(document)) {
            MessageBox.Show(
                Strings.No3DViews,
                Strings.MainWindowTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return Result.Cancelled;
        }

        MainViewModel viewModel = new(document);
        MainWindow window = new(viewModel);
#if REVIT2019_OR_GREATER
        new WindowInteropHelper(window).Owner = commandData.Application.MainWindowHandle;
#else
        new WindowInteropHelper(window).Owner = Process.GetCurrentProcess().MainWindowHandle;
#endif
        if(window.ShowDialog() != true
           || viewModel.SelectedView is not { } selectedView
           || viewModel.NameLanguage is not { } nameLanguage) {
            return Result.Cancelled;
        }

        SaveFileDialog saveFileDialog = new() {
            Filter = Strings.SaveFileFilter,
            FileName = document.Title + ".bim"
        };

        if(saveFileDialog.ShowDialog() != true) {
            return Result.Cancelled;
        }

        document.Export(
            saveFileDialog.FileName,
            new DotBimOptions {
                View = selectedView.View,
                WithLinks = viewModel.ExportLinks,
                WithElementInfo = viewModel.ExportElementInfo,
                WithDocumentInfo = viewModel.ExportDocumentInfo,
                CategoryIds = viewModel.SelectedCategoryIds,
                NameLanguage = nameLanguage.Language
            });

        return Result.Succeeded;
    }

    private static bool Has3DViews(Document document) {
        using FilteredElementCollector collector = new(document);
        return collector
            .OfClass(typeof(View3D))
            .Cast<View3D>()
            .Any(view => !view.IsTemplate);
    }
}
