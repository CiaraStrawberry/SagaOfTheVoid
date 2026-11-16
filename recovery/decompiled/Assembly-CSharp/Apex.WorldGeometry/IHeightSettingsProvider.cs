using System.Collections;
using Apex.DataStructures;

namespace Apex.WorldGeometry;

public interface IHeightSettingsProvider
{
	IEnumerator AssignHeightSettings(CellMatrix matrix, MatrixBounds bounds);
}
