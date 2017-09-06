using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  this class controls the voice settings of the attached player prefab.
/// </summary>
public class VoiceControlScript : MonoBehaviour {

    // the player prefab referance.
    PositinonRelativeToHeadset posscripot;
    // the indicator to tell if the 
    public TextMesh VoiceIndicator;
    // the other voice gameobjects.
    public GameObject[] voices;
    // the local player prefabvoicecontrolscript instance.
    public PlayerPrefabVoiceControlScript locplayer;
    // the voice mode selector singleton.
    public GameObject VoiceModeSelector;

    /// <summary>
    /// initalisation script.
    /// </summary>
    void Start()
    {
        posscripot = GetComponent<PositinonRelativeToHeadset>();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        InvokeRepeating("Rep", 0, 1);
        OnLevelFinishedLoading(new Scene(), new LoadSceneMode());
    }

    /// <summary>
    /// allow the player to toggle voice mode.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) ToggleVoiceMode();
    }

    /// <summary>
    /// update the local voice player with the relevant data.
    /// </summary>
    void Updatelocplayer ()
    {
          if (locplayer) VoiceIndicator.text = "Voice Channel = " + getvoicechanneltext(locplayer.curchannel);
    }

    /// <summary>
    /// Function that runs every second to allow the voice settings to update.
    /// </summary>
    void Rep()
    {

        if (PhotonNetwork.offlineMode == false)
        {
            Updatelocplayer();
            if (VoiceModeSelector.GetActive() == false) VoiceModeSelector.SetActive(true);
            if (locplayer == null && SceneManager.GetActiveScene().name == "mptest" || SceneManager.GetActiveScene().name == "MainMenuIntroLobby")
                foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice"))
                    if (gam.GetComponent<PlayerPrefabVoiceControlScript>().locview.owner == PhotonNetwork.player) locplayer = gam.GetComponent<PlayerPrefabVoiceControlScript>();
        }
        else if (VoiceModeSelector.GetActive() == true) VoiceModeSelector.SetActive(false);
    }


    /// <summary>
    /// Gets the voice channel of a player.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string getvoicechanneltext (PlayerPrefabVoiceControlScript.VoiceChannel input)
    {
        if (input == PlayerPrefabVoiceControlScript.VoiceChannel.All) return "all";
        else if (input == PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly) return "Team Only";
        else return "Mute all";
    }

    /// <summary>
    /// allows the player to change the voice mode of themselves.
    /// </summary>
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
}
