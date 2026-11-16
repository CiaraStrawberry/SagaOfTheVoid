using UnityEngine;

namespace TrueSync;

public class TrueSyncConfig : ScriptableObject
{
	private const int COLLISION_LAYERS = 32;

	private const int COLLISION_TOGGLES = 516;

	public int syncWindow = 10;

	public int rollbackWindow;

	public int panicWindow = 100;

	public bool physics2DEnabled = true;

	public bool[] physics2DIgnoreMatrix = new bool[516];

	public TSVector2 gravity2D = new TSVector2(0, -10);

	public bool speculativeContacts2D;

	public bool physics3DEnabled = true;

	public bool[] physics3DIgnoreMatrix = new bool[516];

	public TSVector gravity3D = new TSVector(0, -10, 0);

	public bool speculativeContacts3D;

	public bool showStats;

	public FP lockedTimeStep = 0.02f;

	public bool GetIgnoreLayerCollision(int layerA, int layerB)
	{
		if (layerB < layerA)
		{
			int num = layerA;
			layerA = layerB;
			layerB = num;
		}
		int num2 = (64 - layerA + 1) * layerA / 2 + layerB;
		if (physics2DEnabled)
		{
			return physics2DIgnoreMatrix[num2];
		}
		if (physics3DEnabled)
		{
			return physics3DIgnoreMatrix[num2];
		}
		return false;
	}
}
