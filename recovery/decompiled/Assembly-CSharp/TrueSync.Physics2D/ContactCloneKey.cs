namespace TrueSync.Physics2D;

internal struct ContactCloneKey(int keyA, int keyB)
{
	internal static ContactCloneKey empty = new ContactCloneKey(-1, -1);

	public int keyA = keyA;

	public int keyB = keyB;
}
