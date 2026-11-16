using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

public interface ISampleHeightsSimple
{
	float SampleHeight(Vector3 position, CellMatrix matrix);
}
