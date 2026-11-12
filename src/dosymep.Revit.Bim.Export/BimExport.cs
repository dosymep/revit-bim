using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using dotbim;

using Element = dotbim.Element;
using Mesh = dotbim.Mesh;

namespace dosymep.Revit.Bim.Export;

internal class BimExport : IExportContext {
    private readonly File _bimFile = new() {
        SchemaVersion = "1.1.0", Meshes = new List<Mesh>(), Elements = new List<Element>()
    };

    private readonly Document _document;
    private readonly string _fileName;
    private readonly BimExportOptions _options;
    private readonly Stack<Transform> _transforms = new();
    
    private int _meshId;
    private Mesh _currentMesh;
    private Element _currentElement;
    private Document _currentDocument;
    
    private int[] _currentColor = { 200, 200, 200, 255 };
    
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

        _currentMesh = new Mesh();
        _currentMesh.MeshId = _currentElement.MeshId;

        _currentMesh.Indices = new List<int>();
        _currentMesh.Coordinates = new List<double>();

        return RenderNodeAction.Proceed;
    }

    public void OnElementEnd(ElementId elementId) {
        if(_currentMesh != null && _currentMesh.Indices.Count > 0) {
            _bimFile.Meshes.Add(_currentMesh);
            _bimFile.Elements.Add(_currentElement);
        }
        
        _currentMesh = default;
        _currentElement = default;
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
        int alpha = (int) System.Math.Round(
            (1.0 - node.Transparency) * 255);

        _currentColor = new[] {
            (int) node.Color.Red,
            (int) node.Color.Green,
            (int) node.Color.Blue,
            System.Math.Max(0, System.Math.Min(255, alpha))
        };
    }

    public void OnPolymesh(PolymeshTopology node) {
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
}