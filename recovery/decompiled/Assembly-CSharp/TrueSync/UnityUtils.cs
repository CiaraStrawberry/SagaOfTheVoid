using System.Collections.Generic;

namespace TrueSync;

public class UnityUtils
{
	public class PlayerComparer : Comparer<PhotonPlayer>
	{
		public override int Compare(PhotonPlayer x, PhotonPlayer y)
		{
			return x.ID - y.ID;
		}
	}

	public class TSBodyComparer : Comparer<TSCollider>
	{
		public override int Compare(TSCollider x, TSCollider y)
		{
			return x.gameObject.name.CompareTo(y.gameObject.name);
		}
	}

	public class TSBody2DComparer : Comparer<TSCollider2D>
	{
		public override int Compare(TSCollider2D x, TSCollider2D y)
		{
			return x.gameObject.name.CompareTo(y.gameObject.name);
		}
	}

	public static PlayerComparer playerComparer = new PlayerComparer();

	public static TSBodyComparer bodyComparer = new TSBodyComparer();

	public static TSBody2DComparer body2DComparer = new TSBody2DComparer();
}
