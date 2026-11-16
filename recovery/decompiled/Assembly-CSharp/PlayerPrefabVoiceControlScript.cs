using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPrefabVoiceControlScript : MonoBehaviour
{
	public enum VoiceChannel
	{
		Muteall,
		TeamOnly,
		All
	}

	public VoiceChannel curchannel;

	public CrossLevelVariableHolder crossvar;

	public PhotonView locview;

	public AudioSource locaudio;

	private PlayerPrefabVoiceControlScript localplayer;

	public bool ingamedebug;

	private PhotonTransformView transformview;

	private void Start()
	{
		locview = GetComponent<PhotonView>();
		locaudio = GetComponent<AudioSource>();
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.GetComponent<PhotonView>().owner == PhotonNetwork.player)
			{
				localplayer = gameObject.GetComponent<PlayerPrefabVoiceControlScript>();
			}
		}
		OnLevelFinishedLoading(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		if (locview.isMine)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
		}
		if ((bool)GameObject.Find("CrossLevelVariables"))
		{
			crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		}
	}

	private void Update()
	{
		ingamedebug = locview.ingame;
		if (!locview.ingame)
		{
			transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		}
		if ((bool)localplayer)
		{
			if (localplayer.curchannel == VoiceChannel.TeamOnly && curchannel == VoiceChannel.TeamOnly && issameteam(PhotonNetwork.player.ID, locview.owner.ID))
			{
				locaudio.mute = false;
			}
			else if (localplayer.curchannel == VoiceChannel.All && curchannel == VoiceChannel.All)
			{
				locaudio.mute = false;
			}
			else
			{
				locaudio.mute = true;
			}
		}
	}

	private bool issameteam(int id1, int id2)
	{
		if (PhotonNetwork.inRoom)
		{
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			List<int> list = LobbyScreenController.getteam1();
			List<int> list2 = LobbyScreenController.getteam2();
			if (list.Contains(id1) && list.Contains(id2))
			{
				return true;
			}
			if (list2.Contains(id1) && list2.Contains(id2))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if (!(this != null) || !(gameObject != null))
		{
			return;
		}
		locview = GetComponent<PhotonView>();
		locaudio = GetComponent<AudioSource>();
		if ((bool)locview)
		{
			locview.onSerializeTransformOption = OnSerializeTransform.All;
		}
		transformview = GetComponent<PhotonTransformView>();
		if ((bool)locview)
		{
			locview.World = GameObject.Find("Objects");
		}
		if ((bool)transformview)
		{
			transformview.World = GameObject.Find("Objects");
		}
		if (scene.name != "mptest")
		{
			curchannel = VoiceChannel.All;
			locview.ingame = false;
			return;
		}
		if (crossvar == null && (bool)GameObject.Find("CrossLevelVariables"))
		{
			crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		}
		if (crossvar.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crossvar.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
		{
			curchannel = VoiceChannel.TeamOnly;
		}
		else
		{
			curchannel = VoiceChannel.All;
		}
		if (PhotonNetwork.playerList.Length == 2)
		{
			curchannel = VoiceChannel.All;
		}
		locview.ingame = true;
	}

	public void setchannelforall(VoiceChannel input)
	{
		locview.RPC("SetCurrentChannel", PhotonTargets.All, (int)input);
	}

	[PunRPC]
	private void SetCurrentChannel(int input)
	{
		switch (input)
		{
		case 0:
			curchannel = VoiceChannel.Muteall;
			break;
		case 1:
			curchannel = VoiceChannel.TeamOnly;
			break;
		case 2:
			curchannel = VoiceChannel.All;
			break;
		}
	}
}
