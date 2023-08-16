using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using dotbim;

using Element = dotbim.Element;
using Mesh = dotbim.Mesh;

namespace dosymep.Revit.Bim.Export;

internal class BimExport : IExportContext {
    private readonly File _bimFile = new() {
        SchemaVersion = "1.1.0",
        Meshes = new List<Mesh>(),
        Elements = new List<Element>()
    };

    private readonly Document _document;
    private readonly string _fileName;
    private readonly BimExportOptions _options;
    private readonly Stack<Transform> _transforms = new();

    private Document _currentDocument;
    private Element _currentElement;

    private int _currentIndex;
    private Mesh _currentMesh;
    private Dictionary<XYZ, int> _indexCache = new(new XyzEqualityComparer());

    private int _meshId;

    public BimExport(Document document, string fileName, BimExportOptions options) {
        _document = document;
        _fileName = fileName;
        _options = options;
        _currentDocument = _document;

        _transforms.Push(Transform.Identity);
    }

    private Transform CurrentTransform => _transforms.Peek();

    public bool Start() {
        if(_options.WithDocumentInfo) {
            _bimFile.Info = _document.ProjectInformation.ToInfo();
        }
        
        return true;
    }

    public void Finish() {
        _bimFile.Save(_fileName);
    }

    public bool IsCanceled() {
        return false;
    }

    public RenderNodeAction OnViewBegin(ViewNode node) {
        return RenderNodeAction.Proceed;
    }

    public void OnViewEnd(ElementId elementId) {
    }

    public RenderNodeAction OnElementBegin(ElementId elementId) {
        Autodesk.Revit.DB.Element element = _currentDocument.GetElement(elementId);
        _currentElement = element.ToBim();
        if(_options.WithElementInfo) {
            _currentElement.Info = element.ToInfo();
        }

        _currentElement.MeshId = _meshId++;
        _bimFile.Elements.Add(_currentElement);

        _currentMesh = new Mesh();
        _currentMesh.MeshId = _currentElement.MeshId;

        _currentMesh.Indices = new List<int>();
        _currentMesh.Coordinates = new List<double>();
        _bimFile.Meshes.Add(_currentMesh);

        return RenderNodeAction.Proceed;
    }

    public void OnElementEnd(ElementId elementId) {
        _currentMesh = default;
        _currentIndex = default;
        _currentElement = default;
        _indexCache = new Dictionary<XYZ, int>();
    }

    public RenderNodeAction OnInstanceBegin(InstanceNode node) {
        _transforms.Push(CurrentTransform.Multiply(node.GetTransform()));
        return RenderNodeAction.Proceed;
    }

    public void OnInstanceEnd(InstanceNode node) {
        _transforms.Pop();
    }

    public RenderNodeAction OnLinkBegin(LinkNode node) {
        if(!_options.WithLinks) {
            return RenderNodeAction.Skip;
        }
        _currentDocument = node.GetDocument();
        _transforms.Push(CurrentTransform.Multiply(node.GetTransform()));

        return RenderNodeAction.Proceed;
    }

    public void OnLinkEnd(LinkNode node) {
        _transforms.Pop();
        _currentDocument = _document;
    }

    public RenderNodeAction OnFaceBegin(FaceNode node) {
        return RenderNodeAction.Skip;
    }

    public void OnFaceEnd(FaceNode node) {
    }

    public void OnRPC(RPCNode node) {
    }

    public void OnLight(LightNode node) {
    }

    public void OnMaterial(MaterialNode node) {
        _currentElement.FaceColors.AddRange(node.Color.Enumerate());
        _currentElement.FaceColors.Add((int) ((100 - node.Transparency) * 2.55555555));
    }

    public void OnPolymesh(PolymeshTopology node) {
        Transform currentTransform = CurrentTransform;
        IList<XYZ> points = node.GetPoints()
            .Select(item => currentTransform.OfPoint(item))
            .ToArray();

        foreach(PolymeshFacet polymeshFacet in node.GetFacets()) {
            foreach(XYZ point in polymeshFacet.Enumerate(points)) {
                int index;
                if(_indexCache.TryGetValue(point, out int result)) {
                    index = result;
                } else {
                    index = _currentIndex++;
                    _indexCache.Add(point, _currentIndex);
                }

                _currentMesh.Indices.Add(index);
                _currentMesh.Coordinates.AddRange(point.Enumerate());
            }
        }
    }
}