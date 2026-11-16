using UnityEngine;
using UnityEngine.UI;

public class MatchJoiner : MonoBehaviour
{
	public Text btnText;

	private string roomName;

	public void Join()
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	public void UpdateRoom(RoomInfo room)
	{
		roomName = room.Name;
		btnText.text = roomName;
	}
}
