using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class VoiceControlScript : MonoBehaviour {
    PositinonRelativeToHeadset posscripot;
    public TextMesh VoiceIndicator;
    public GameObject[] voices;
    public PlayerPrefabVoiceControlScript locplayer;
    public GameObject VoiceModeSelector;
    // Use this for initialization
    void Start() {
       
        posscripot = GetComponent<PositinonRelativeToHeadset>();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        InvokeRepeating("Rep", 0, 1);
        OnLevelFinishedLoading(new Scene(), new LoadSceneMode());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) ToggleVoiceMode();
    }
    void Updatelocplayer ()
    {
          if (locplayer) VoiceIndicator.text = "Voice Channel = " + getvoicechanneltext(locplayer.curchannel);
    }
    void Rep ()
    {
       
        if (PhotonNetwork.offlineMode == false)
        {
            Updatelocplayer(); 
           // if (VoiceModeSelector.GetActive() == false) VoiceModeSelector.SetActive(true);
            if (VoiceModeSelector.GetActive() == false) VoiceModeSelector.SetActive(true);
            if (locplayer == null && SceneManager.GetActiveScene().name == "mptest" || SceneManager.GetActiveScene().name == "MainMenuIntroLobby")
               foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice"))
                    if (gam.GetComponent<PlayerPrefabVoiceControlScript>().locview.owner == PhotonNetwork.player) locplayer = gam.GetComponent<PlayerPrefabVoiceControlScript>();
        }
        else if (VoiceModeSelector.GetActive() == true) VoiceModeSelector.SetActive(false);
    }

    // Update is called once per frame
    string getvoicechanneltext (PlayerPrefabVoiceControlScript.VoiceChannel input)
    {
        if (input == PlayerPrefabVoiceControlScript.VoiceChannel.All) return "all";
        else if (input == PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly) return "Team Only";
        else return "Mute all";
    }
    void ToggleVoiceMode()
    {
       
        
           foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice")) if (gam.GetComponent<PlayerPrefabVoiceControlScript>().locview.owner == PhotonNetwork.player) locplayer = gam.GetComponent<PlayerPrefabVoiceControlScript>();
            PlayerPrefabVoiceControlScript.VoiceChannel nextchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
            if (locplayer.curchannel == PlayerPrefabVoiceControlScript.VoiceChannel.All) nextchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
            if (locplayer.curchannel == PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly) nextchannel = PlayerPrefabVoiceControlScript.VoiceChannel.Muteall;
            if (locplayer.curchannel == PlayerPrefabVoiceControlScript.VoiceChannel.Muteall) nextchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
        locplayer.setchannelforall(nextchannel);
         Updatelocplayer();
    }
    [PunRPC]
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
      //  if(GameObject.Find("Controllers")) locplayer = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
    }
}
