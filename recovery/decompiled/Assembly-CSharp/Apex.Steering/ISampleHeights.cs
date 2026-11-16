using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

public interface ISampleHeights : ISampleHeightsSimple
{
	HeightSamplingMode samplingStrategy { get; }

	float SampleHeight(Vector3 position);

	bool TrySampleHeight(Vector3 position, out float height);

	bool TrySampleHeight(Vector3 position, CellMatrix matrix, out float height);
}
