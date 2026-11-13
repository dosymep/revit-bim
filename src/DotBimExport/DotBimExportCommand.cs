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
        SaveFileDialog saveFileDialog = new() {Filter = "dotbim files (*.bim)|*.bim", FileName = ".bim"};
       
        if(saveFileDialog.ShowDialog() != true) {
            return Result.Cancelled;
        }
        
        commandData.Application.ActiveUIDocument.Document.Export(saveFileDialog.SafeFileName,
            new DotBimOptions {
                WithLinks = true,
                WithElementInfo = true,
                WithDocumentInfo = true,
                View = commandData.Application.ActiveUIDocument.ActiveView
            });
        
        return Result.Succeeded;
    }
}