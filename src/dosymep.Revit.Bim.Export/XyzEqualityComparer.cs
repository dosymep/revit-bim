using System.Collections.Generic;

using Autodesk.Revit.DB;

namespace dosymep.Revit.Bim.Export;

internal sealed class XyzEqualityComparer : IEqualityComparer<XYZ> {
    public bool Equals(XYZ x, XYZ y) {
        if(ReferenceEquals(x, y)) {
            return true;
        }

        if(ReferenceEquals(x, null)) {
            return false;
        }

        if(ReferenceEquals(y, null)) {
            return false;
        }

        if(x.GetType() != y.GetType()) {
            return false;
        }

        return x.IsAlmostEqualTo(y);
    }

    public int GetHashCode(XYZ obj) {
        unchecked {
            int hashCode = obj.Z.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.Y.GetHashCode();
            hashCode = (hashCode * 397) ^ obj.X.GetHashCode();
            return hashCode;
        }
    }
}