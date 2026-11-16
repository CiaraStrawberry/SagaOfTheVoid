using Photon;
using UnityEngine;
using UnityEngine.UI;

public class RpsDemoConnect : PunBehaviour
{
	public InputField InputField;

	public string UserId;

	public string previousRoom;

	private const string MainSceneName = "DemoRPS-Scene";

	private const string NickNamePlayerPrefsKey = "NickName";

	private void Start()
	{
		InputField.text = ((!PlayerPrefs.HasKey("NickName")) ? string.Empty : PlayerPrefs.GetString("NickName"));
	}

	public void ApplyUserIdAndConnect()
	{
		string text = "DemoNick";
		if (InputField != null && !string.IsNullOrEmpty(InputField.text))
		{
			text = InputField.text;
			PlayerPrefs.SetString("NickName", text);
		}
		Debug.Log("Nickname: " + text + " userID: " + UserId, this);
		if (PhotonNetwork.AuthValues == null)
		{
			PhotonNetwork.AuthValues = new AuthenticationValues();
		}
		PhotonNetwork.playerName = text;
		PhotonNetwork.ConnectUsingSettings("0.5");
		PhotonHandler.StopFallbackSendAckThread();
	}

	public override void OnConnectedToMaster()
	{
		UserId = PhotonNetwork.player.userId;
		if (!string.IsNullOrEmpty(previousRoom))
		{
			Debug.Log("ReJoining previous room: " + previousRoom);
			PhotonNetwork.ReJoinRoom(previousRoom);
			previousRoom = null;
		}
		else
		{
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnJoinedLobby()
	{
		OnConnectedToMaster();
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			MaxPlayers = 2,
			PlayerTtl = 5000
		}, null);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room: " + PhotonNetwork.room.name);
		previousRoom = PhotonNetwork.room.name;
	}

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		previousRoom = null;
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		Debug.Log(string.Concat("Disconnected due to: ", cause, ". this.previousRoom: ", previousRoom));
	}
}
