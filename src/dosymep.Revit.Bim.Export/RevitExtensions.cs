using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        Element element = new();
        element.Guid = Guid.NewGuid().ToString(); //self.UniqueId.Substring(0, 36);
        element.Type = self.GetType().Name;
        //element.Vector = new dotbim.Vector();
        element.FaceColors = new List<int>();
        return element;
    }

    public static Dictionary<string, string> ToInfo(this Autodesk.Revit.DB.Element self) {
        string[] paramNames = self.Parameters
            .Cast<Parameter>()
            .Select(item => item.Definition.Name).Distinct()
            .OrderBy(item => item)
            .ToArray();

        return paramNames
            .Select(item => self.LookupParameter(item))
            .Where(item => item != null)
            .Select(item => item.ToParam())
            .Where(item => !string.IsNullOrEmpty(item.Item2))
            .ToDictionary(item => item.Item1, item => item.Item2);
    }

    private static (string, string) ToParam(this Parameter param) {
        string value = param.AsValueString();
        if(string.IsNullOrEmpty(value)) {
            switch(param.StorageType) {
                case StorageType.None:
                    return (param.Definition.Name, default);
                case StorageType.Integer:
                    return (param.Definition.Name, param.AsInteger().ToString());
                case StorageType.Double:
                    return (param.Definition.Name, param.AsDouble().ToString(CultureInfo.InvariantCulture));
                case StorageType.String:
                    return (param.Definition.Name, param.AsString());
                case StorageType.ElementId:
                    return (param.Definition.Name, param.AsElementId().ToString());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return (param.Definition.Name, value);
    }

    private static double Convert(double value) {
        return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
    }
}