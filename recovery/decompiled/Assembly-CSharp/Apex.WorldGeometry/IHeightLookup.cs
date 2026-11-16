using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

public interface IHeightLookup
{
	bool hasHeights { get; }

	int heightCount { get; }

	bool Add(int x, int z, float height);

	bool TryGetHeight(int x, int z, out float height);

	void Cleanup();

	IHeightLookup PrepareForUpdate(MatrixBounds suggestedBounds, out MatrixBounds requiredBounds);

	void FinishUpdate(IHeightLookup updatedHeights);

	void Render(Vector3 position, float pointGranularity, Color drawColor);
}
