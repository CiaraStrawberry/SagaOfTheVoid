using System;
using ExitGames.Client.Photon;

namespace TrueSync;

public class PhotonTrueSyncCommunicator : ICommunicator
{
	private LoadBalancingPeer loadBalancingPeer;

	private static PhotonNetwork.EventCallback lastEventCallback;

	internal PhotonTrueSyncCommunicator(LoadBalancingPeer loadBalancingPeer)
	{
		this.loadBalancingPeer = loadBalancingPeer;
	}

	public int RoundTripTime()
	{
		return loadBalancingPeer.RoundTripTime;
	}

	public void OpRaiseEvent(byte eventCode, object message, bool reliable, int[] toPlayers)
	{
		if (loadBalancingPeer.PeerState == PeerStateValue.Connected)
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.TargetActors = toPlayers;
			loadBalancingPeer.OpRaiseEvent(eventCode, message, reliable, raiseEventOptions);
		}
	}

	public void AddEventListener(OnEventReceived onEventReceived)
	{
		if (lastEventCallback != null)
		{
			PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Remove(PhotonNetwork.OnEventCall, lastEventCallback);
		}
		lastEventCallback = (byte eventCode, object content, int senderId) =>
		{
			onEventReceived(eventCode, content);
		};
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Combine(PhotonNetwork.OnEventCall, lastEventCallback);
	}
}
