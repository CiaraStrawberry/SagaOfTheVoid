using UnityEngine;
using UnityEngine.SceneManagement;

public class VoiceControlScript : MonoBehaviour
{
	private PositinonRelativeToHeadset posscripot;

	public TextMesh VoiceIndicator;

	public GameObject[] voices;

	public PlayerPrefabVoiceControlScript locplayer;

	public GameObject VoiceModeSelector;

	private void Start()
	{
		posscripot = GetComponent<PositinonRelativeToHeadset>();
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		InvokeRepeating("Rep", 0f, 1f);
		OnLevelFinishedLoading(default, LoadSceneMode.Single);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			ToggleVoiceMode();
		}
	}

	private void Updatelocplayer()
	{
		if ((bool)locplayer)
		{
			VoiceIndicator.text = "Voice Channel = " + getvoicechanneltext(locplayer.curchannel);
		}
	}

	private void Rep()
	{
		if (!PhotonNetwork.offlineMode)
		{
			Updatelocplayer();
			if (!VoiceModeSelector.GetActive())
			{
				VoiceModeSelector.SetActive(value: true);
			}
			if ((!(locplayer == null) || !(SceneManager.GetActiveScene().name == "mptest")) && !(SceneManager.GetActiveScene().name == "MainMenuIntroLobby"))
			{
				return;
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
			foreach (GameObject gameObject in array)
			{
				if (gameObject.GetComponent<PlayerPrefabVoiceControlScript>().locview.owner == PhotonNetwork.player)
				{
					locplayer = gameObject.GetComponent<PlayerPrefabVoiceControlScript>();
				}
			}
		}
		else if (VoiceModeSelector.GetActive())
		{
			VoiceModeSelector.SetActive(value: false);
		}
	}

	private string getvoicechanneltext(PlayerPrefabVoiceControlScript.VoiceChannel input)
	{
		return input switch
		{
			PlayerPrefabVoiceControlScript.VoiceChannel.All => "all", 
			PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly => "Team Only", 
			_ => "Mute all", 
		};
	}

	private void ToggleVoiceMode()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<PlayerPrefabVoiceControlScript>().locview.owner == PhotonNetwork.player)
			{
				locplayer = gameObject.GetComponent<PlayerPrefabVoiceControlScript>();
			}
		}
		PlayerPrefabVoiceControlScript.VoiceChannel input = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
		if (locplayer.curchannel == PlayerPrefabVoiceControlScript.VoiceChannel.All)
		{
			input = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
		}
		if (locplayer.curchannel == PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly)
		{
			input = PlayerPrefabVoiceControlScript.VoiceChannel.Muteall;
		}
		if (locplayer.curchannel == PlayerPrefabVoiceControlScript.VoiceChannel.Muteall)
		{
			input = PlayerPrefabVoiceControlScript.VoiceChannel.All;
		}
		locplayer.setchannelforall(input);
		Updatelocplayer();
	}

	[PunRPC]
	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
	}
}
