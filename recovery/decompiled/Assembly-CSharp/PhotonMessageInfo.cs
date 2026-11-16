public struct PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
{
	private readonly int timeInt = timestamp;

	public readonly PhotonPlayer sender = player;

	public readonly PhotonView photonView = view;

	public double timestamp
	{
		get
		{
			uint num = (uint)timeInt;
			double num2 = num;
			return num2 / 1000.0;
		}
	}

	public override string ToString()
	{
		return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", timestamp, sender);
	}
}
