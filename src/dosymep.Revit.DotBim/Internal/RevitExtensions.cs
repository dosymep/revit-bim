using System.Globalization;

using Autodesk.Revit.DB;

using dotbim;

using Element = dotbim.Element;

namespace dosymep.Revit.DotBim.Internal;

internal static class RevitExtensions {
    public static IEnumerable<double> Enumerate(this XYZ self) {
        yield return Convert(self.X);
        yield return Convert(self.Y);
        yield return Convert(self.Z);
    }

    extension(Autodesk.Revit.DB.Element self) {
        public Element ToBim() {
            Element element = new() {
                Guid = self.UniqueId,
                Type = self.GetType().Name,
                FaceColors = [],
                Vector = new Vector {X = 0, Y = 0, Z = 0},
                Rotation = new Rotation {Qx = 0, Qy = 0, Qz = 0, Qw = 1}
            };

            return element;
        }

        public Dictionary<string, string> ToInfo() {
            string[] paramNames = self.Parameters
                .Cast<Parameter>()
                .Select(item => item.Definition.Name).Distinct()
                .OrderBy(item => item, StringComparer.Ordinal)
                .ToArray();

            return paramNames
                .Select(item => self.LookupParameter(item))
                .Where(item => item != null)
                .Select(item => item.ToParam())
                .Where(item => !string.IsNullOrEmpty(item.Item2))
                .ToDictionary(item => item.Item1, item => item.Item2!);
        }
    }

    private static (string, string?) ToParam(this Parameter param) {
        string value = param.AsValueString();
        if(string.IsNullOrEmpty(value)) {
            return param.StorageType switch {
                StorageType.None => ((string, string?)) (param.Definition.Name, null),
                StorageType.Integer => (param.Definition.Name, param.AsInteger().ToString()),
                StorageType.Double => (param.Definition.Name, param.AsDouble().ToString(CultureInfo.InvariantCulture)),
                StorageType.String => (param.Definition.Name, param.AsString()),
                StorageType.ElementId => (param.Definition.Name, param.AsElementId().ToString()),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        return (param.Definition.Name, value);
    }

#if REVIT2021_OR_GREATER
    private static double Convert(double value) {
        return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Meters);
    }
#else
    private static double Convert(double value) {
        return UnitUtils.ConvertFromInternalUnits(value, DisplayUnitType.DUT_METERS);
    }
#endif
}