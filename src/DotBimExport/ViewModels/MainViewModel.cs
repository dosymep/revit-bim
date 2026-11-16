using System.Collections.ObjectModel;
using System.Windows.Input;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

using DotBimExport.Resources;

namespace DotBimExport.ViewModels;

/// <summary>
/// Export settings for the dotbim dialog.
/// </summary>
public sealed class MainViewModel : ObservableObject {
    private readonly Document _document;
    private IReadOnlyList<ViewItemViewModel> _views = [];
    private IReadOnlyList<LanguageItemViewModel> _languages = [];
    private ObservableCollection<CategoryItemViewModel> _categories = [];
    private ViewItemViewModel? _selectedView;
    private LanguageItemViewModel? _nameLanguage;
    private bool _exportElementInfo = true;
    private bool _exportLinks = true;

    /// <summary>
    /// Initializes commands for the given document. Views, language and categories are loaded by <see cref="LoadViewCommand"/>.
    /// </summary>
    /// <param name="document">Document whose 3D views, language and categories are listed.</param>
    public MainViewModel(Document document) {
        _document = document ?? throw new ArgumentNullException(nameof(document));

        LoadViewCommand = new RelayCommand(LoadView);
        AcceptViewCommand = new RelayCommand(AcceptView);

        ClearCategoriesCommand = new RelayCommand(() => SetCategoriesSelected(false));
        SelectAllCategoriesCommand = new RelayCommand(() => SetCategoriesSelected(true));
    }

    /// <summary>
    /// Raised when export settings are accepted.
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <summary>
    /// Raised when the user must be told why export cannot start.
    /// </summary>
    public event EventHandler<string>? AlertRequested;

    public ICommand LoadViewCommand { get; }

    public ICommand AcceptViewCommand { get; }

    public ICommand ClearCategoriesCommand { get; }

    public ICommand SelectAllCategoriesCommand { get; }

    public IReadOnlyList<ViewItemViewModel> Views {
        get => _views;
        private set => SetProperty(ref _views, value);
    }

    public IReadOnlyList<LanguageItemViewModel> Languages {
        get => _languages;
        private set => SetProperty(ref _languages, value);
    }

    public ObservableCollection<CategoryItemViewModel> Categories {
        get => _categories;
        private set => SetProperty(ref _categories, value);
    }

    public ViewItemViewModel? SelectedView {
        get => _selectedView;
        set => SetProperty(ref _selectedView, value);
    }

    public LanguageItemViewModel? NameLanguage {
        get => _nameLanguage;
        set => SetProperty(ref _nameLanguage, value);
    }

    public bool ExportLinks {
        get => _exportLinks;
        set => SetProperty(ref _exportLinks, value);
    }

    public bool ExportElementInfo {
        get => _exportElementInfo;
        set => SetProperty(ref _exportElementInfo, value);
    }

    /// <summary>
    /// Gets the selected category ids, or <see langword="null"/> when every listed category is selected.
    /// </summary>
    public IReadOnlyCollection<ElementId>? SelectedCategoryIds {
        get {
            if(Categories.All(item => item.IsSelected)) {
                return null;
            }

            return Categories
                .Where(item => item.IsSelected)
                .Select(item => item.Id)
                .ToArray();
        }
    }

    private void LoadView() {
        Views = LoadViews(out ViewItemViewModel? selectedView);
        SelectedView = selectedView;

        Languages = LoadLanguages(out LanguageItemViewModel? nameLanguage);
        NameLanguage = nameLanguage;

        Categories = LoadCategories();
    }

    private IReadOnlyList<ViewItemViewModel> LoadViews(out ViewItemViewModel? selectedView) {
        using FilteredElementCollector collector = new(_document);

        List<ViewItemViewModel> views = collector
            .OfClass(typeof(View3D))
            .Cast<View3D>()
            .Where(view => !view.IsTemplate)
            .OrderBy(view => view.Name, StringComparer.CurrentCultureIgnoreCase)
            .Select(view => new ViewItemViewModel(view))
            .ToList();

        selectedView = _document.ActiveView is not View3D active || active.IsTemplate
            ? null
            : views.FirstOrDefault(item => item.View.Id == active.Id);

        selectedView ??= views.FirstOrDefault();

        return views;
    }

    private IReadOnlyList<LanguageItemViewModel> LoadLanguages(out LanguageItemViewModel? nameLanguage) {
        LanguageItemViewModel[] languages = Enum.GetValues(typeof(LanguageType))
            .Cast<LanguageType>()
            .Where(language => language != LanguageType.Unknown)
            .Select(language => new LanguageItemViewModel(language))
            .ToArray();

        LanguageType current = _document.Application.Language;
        nameLanguage = languages.FirstOrDefault(item => item.Language == current)
                       ?? languages.FirstOrDefault();

        return languages;
    }

    private ObservableCollection<CategoryItemViewModel> LoadCategories() {
        ObservableCollection<CategoryItemViewModel> categories = [];
       
        foreach(Category category in _document.Settings.Categories
                    .Cast<Category>()
                    .Where(category => category.CategoryType == CategoryType.Model && category.Parent == null)
                    .Where(category => !string.IsNullOrWhiteSpace(category.Name))
                    .OrderBy(category => category.Name, StringComparer.CurrentCultureIgnoreCase)) {
            
            categories.Add(new CategoryItemViewModel(category.Id, category.Name));
        }

        return categories;
    }

    private void SetCategoriesSelected(bool isSelected) {
        foreach(CategoryItemViewModel item in Categories) {
            item.IsSelected = isSelected;
        }
    }

    private void AcceptView() {
        if(SelectedView == null) {
            AlertRequested?.Invoke(this, Strings.SelectView);
            return;
        }

        if(NameLanguage == null) {
            AlertRequested?.Invoke(this, Strings.SelectNameLanguage);
            return;
        }

        if(Categories.All(item => !item.IsSelected)) {
            AlertRequested?.Invoke(this, Strings.SelectCategory);
            return;
        }

        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
