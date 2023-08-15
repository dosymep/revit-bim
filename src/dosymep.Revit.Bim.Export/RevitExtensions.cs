using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;

using dotbim;

using Color = Autodesk.Revit.DB.Color;
using Element = dotbim.Element;

namespace dosymep.Revit.Bim.Export;

internal static class RevitExtensions {
    public static IEnumerable<XYZ> Enumerate(this PolymeshFacet polymeshFacet, IList<XYZ> points) {
        yield return points[polymeshFacet.V1];
        yield return points[polymeshFacet.V2];
        yield return points[polymeshFacet.V3];
    }

    public static IEnumerable<int> Enumerate(this Color self) {
        yield return self.Red;
        yield return self.Green;
        yield return self.Blue;
    }

    public static IEnumerable<double> Enumerate(this XYZ self) {
        yield return Convert(self.X);
        yield return Convert(self.Y);
        yield return Convert(self.Z);
    }

    public static dotbim.Color ToBim(this Color self) {
        return new dotbim.Color {R = self.Red, G = self.Green, B = self.Blue};
    }

    public static Vector ToBim(this XYZ self) {
        return new Vector {X = Convert(self.X), Y = Convert(self.Y), Z = Convert(self.Z)};
    }

    public static Element ToBim(this Autodesk.Revit.DB.Element self) {
        Element element = new Element();
        element.Guid = Guid.NewGuid().ToString(); //self.UniqueId.Substring(0, 36);
        element.Type = self.GetType().Name;
        element.MeshId = self.Id.IntegerValue;
        //element.Vector = new dotbim.Vector();
        element.FaceColors = new List<int>();
        //element.Info = new Dictionary<string, string>();
        return element;
    }

    private static double Convert(double value) {
        return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
    }
}