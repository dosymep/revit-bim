using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using dosymep.Revit.DotBim;

using Microsoft.Win32;

namespace DotBimExport;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
internal sealed class DotBimExportCommand : IExternalCommand {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
        var document = commandData.Application.ActiveUIDocument.Document;

        SaveFileDialog saveFileDialog =
            new() {Filter = "dotbim files (*.bim)|*.bim", FileName = document.Title + ".bim"};
       
        if(saveFileDialog.ShowDialog() != true) {
            return Result.Cancelled;
        }
        
        document.Export(saveFileDialog.FileName,
            new DotBimOptions {
                WithLinks = true,
                WithElementInfo = true,
                WithDocumentInfo = true,
                View = document.ActiveView
            });
        
        return Result.Succeeded;
    }
}