using System.Collections;

namespace Apex.WorldGeometry;

public interface IClearanceProvider
{
	IEnumerator SetClearance(CellMatrix matrix);

	IEnumerator Reset(CellMatrix matrix);
}
