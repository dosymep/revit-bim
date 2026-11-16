using Autodesk.Revit.DB;

namespace DotBimExport.ViewModels;

public sealed class CategoryItemViewModel : ObservableObject {
    private bool _isSelected = true;

    public CategoryItemViewModel(ElementId id, string name) {
        Id = id;
        Name = name;
    }

    public ElementId Id { get; }

    public string Name { get; }

    public bool IsSelected {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
