using Autodesk.Revit.DB;

using File = dotbim.File;
using Mesh = dotbim.Mesh;
using Element = dotbim.Element;

namespace dosymep.Revit.DotBim.Internal;

internal class DotBimExportContext : IExportContext {
    private readonly File _dotBimFile = new() {
        SchemaVersion = "1.2.0", Meshes = [], Elements = []
    };

    private readonly string _fileName;
    private readonly Document _document;
    private readonly DotBimOptions _options;
    private readonly HashSet<ElementId>? _categoryIds;
    private readonly Stack<Transform> _transforms = new();

    private int _meshId;
    private Mesh? _currentMesh;
    private Element? _currentElement;
    private Document? _currentDocument;

    private int[] _currentColor = {200, 200, 200, 255};

    public DotBimExportContext(Document document, string fileName, DotBimOptions options) {
        if(string.IsNullOrWhiteSpace(fileName)) {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
        }

        _options = options ?? throw new ArgumentNullException(nameof(options));
        _document = document ?? throw new ArgumentNullException(nameof(document));
        
        _fileName = fileName;
        _currentDocument = _document;

        if(options.CategoryIds != null) {
            _categoryIds = new HashSet<ElementId>(options.CategoryIds);
        }

        _transforms.Push(Transform.Identity);
    }

    private Transform CurrentTransform => _transforms.Peek();

    private bool IsExported(Category? category) {
        if(_categoryIds == null) {
            return true;
        }

        return category != null && _categoryIds.Contains(category.Id);
    }

    public bool Start() {
        if(_options.WithDocumentInfo) {
            _dotBimFile.Info = _document.ProjectInformation.ToInfo();
        }

        return true;
    }

    public void Finish() {
        _dotBimFile.Save(_fileName);
    }

    public RenderNodeAction OnElementBegin(ElementId elementId) {
        if(_currentDocument is null) {
            throw new InvalidOperationException("Current document is not set.");
        }

        Autodesk.Revit.DB.Element? element = _currentDocument.GetElement(elementId);
        if(element == null || !IsExported(element.Category)) {
            return RenderNodeAction.Skip;
        }

        _currentElement = element.ToBim();

        if(_options.WithElementInfo) {
            _currentElement.Info = element.ToInfo();
        }

        _currentElement.MeshId = _meshId++;
        _currentMesh = new Mesh {
            MeshId = _currentElement.MeshId, Indices = [], Coordinates = []
        };

        return RenderNodeAction.Proceed;
    }

    public void OnElementEnd(ElementId elementId) {
        if(_currentMesh != null && _currentMesh.Indices.Count > 0) {
            _dotBimFile.Meshes.Add(_currentMesh);
            _dotBimFile.Elements.Add(_currentElement);
        }

        _currentMesh = null;
        _currentElement = null;
    }

    public RenderNodeAction OnInstanceBegin(InstanceNode node) {
        Transform transform = CurrentTransform;
        
        _transforms.Push(transform.Multiply(node.GetTransform()));
       
        return RenderNodeAction.Proceed;
    }

    public void OnInstanceEnd(InstanceNode node) {
        _transforms.Pop();
    }

    public RenderNodeAction OnLinkBegin(LinkNode node) {
        if(!_options.WithLinks) {
            return RenderNodeAction.Skip;
        }

        Transform transform = CurrentTransform;

        _currentDocument = node.GetDocument();
        _transforms.Push(transform.Multiply(node.GetTransform()));

        return RenderNodeAction.Proceed;
    }

    public void OnLinkEnd(LinkNode node) {
        if(_options.WithLinks) {
            _transforms.Pop();
            _currentDocument = _document;
        }
    }

    public void OnMaterial(MaterialNode node) {
        int alpha = (int) Math.Round((1.0 - node.Transparency) * 255);
        _currentColor = new[] {node.Color.Red, node.Color.Green, node.Color.Blue, Math.Max(0, Math.Min(255, alpha))};
    }

    public void OnPolymesh(PolymeshTopology node) {
        if(_currentMesh is null) {
            throw new InvalidOperationException("Current mesh is not set.");
        }

        if(_currentElement is null) {
            throw new InvalidOperationException("Current element is not set.");
        }

        Transform transform = CurrentTransform;

        bool mirrored = transform.HasReflection;
        int offset = _currentMesh.Coordinates.Count / 3;

        foreach(XYZ point in node.GetPoints()) {
            _currentMesh.Coordinates.AddRange(
                transform.OfPoint(point).Enumerate());
        }

        foreach(PolymeshFacet facet in node.GetFacets()) {
            _currentMesh.Indices.Add(offset + facet.V1);
            _currentMesh.Indices.Add(offset + (mirrored ? facet.V3 : facet.V2));
            _currentMesh.Indices.Add(offset + (mirrored ? facet.V2 : facet.V3));

            _currentElement.FaceColors.AddRange(_currentColor);
        }
    }

    #region Not Implemented

    public bool IsCanceled() { return false; }
    public RenderNodeAction OnFaceBegin(FaceNode node) { return RenderNodeAction.Skip; }
    public RenderNodeAction OnViewBegin(ViewNode node) { return RenderNodeAction.Proceed; }
    public void OnViewEnd(ElementId elementId) { }
    public void OnFaceEnd(FaceNode node) { }
    public void OnRPC(RPCNode node) { }
    public void OnLight(LightNode node) { }

    #endregion

#if REVIT2016
    public void OnDaylightPortal(DaylightPortalNode node) { }
#endif
    
}