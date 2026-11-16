using Autodesk.Revit.DB;

namespace DotBimExport.ViewModels;

public sealed class ViewItemViewModel : ObservableObject {
    public ViewItemViewModel(View3D view) {
        View = view ?? throw new ArgumentNullException(nameof(view));
        Name = view.Name;
    }

    public View3D View { get; }

    public string Name { get; }
}
