# Usage
```csharp
View view = uiApplication.ActiveUIDocument.ActiveView;
Document document = uiApplication.ActiveUIDocument.Document;

document.Export("path/to/sample.bim",
    new BimExportOptions() {View = view, WithLinks = true, WithElementInfo = true, WithDocumentInfo = true})
```

# Sample

![Result](docs/bim.png "Bim")
![Original](docs/revit.png "Revit")

# Known Issues
It's doesn't work 😂, 
because I don't know how to fetch mesh indices correctly.
But I think this project will be good start point to research.