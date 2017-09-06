using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the script that controls who can hear who. It was actually developed before i discovered the concept of photon voice channels,
/// so its a bit redundant. but hey i got the experiance of writing it atleast.
/// </summary>
public class PlayerPrefabVoiceControlScript : MonoBehaviour
{
    // current voice channel.
    public VoiceChannel curchannel;
    // the crosslevel variableholder singleton.
    public CrossLevelVariableHolder crossvar;  
    // the local photonview assosiated with this script.
    public PhotonView locview;
    // the local audiosource assosiated with this script.
    public AudioSource locaudio;
    // the client player in this game instance.
    private PlayerPrefabVoiceControlScript localplayer;
    // the photon transformview instance.
    private PhotonTransformView transformview;

    /// <summary>
    /// the voice channel enum
    /// </summary>
    public enum VoiceChannel
    {
        Muteall,
        TeamOnly,
        All
    }

    /// <summary>
    /// Initialisation.
    /// </summary>
    void Start()
    {
        locview = GetComponent<PhotonView>();
        locaudio = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        GameObject[] voices = GameObject.FindGameObjectsWithTag("Voice");
        foreach (GameObject voice in voices) if (voice.GetComponent<PhotonView>().owner == PhotonNetwork.player) localplayer = voice.GetComponent<PlayerPrefabVoiceControlScript>();
        OnLevelFinishedLoading(SceneManager.GetActiveScene(), new LoadSceneMode());
        if (locview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
        if (GameObject.Find("CrossLevelVariables")) crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
    }

    /// <summary>
    /// Check for muting status.
    /// </summary>
    void Update()
    {
        ingamedebug = locview.ingame;
        if (locview.ingame == false) transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        if (localplayer)
        {
            if (localplayer.curchannel == VoiceChannel.TeamOnly && curchannel == VoiceChannel.TeamOnly && issameteam(PhotonNetwork.player.ID, locview.owner.ID) == true) locaudio.mute = false;
            else if (localplayer.curchannel == VoiceChannel.All && curchannel == VoiceChannel.All) locaudio.mute = false;
            else locaudio.mute = true;
        }
    }

    /// <summary>
    /// Check if 2 photonviews are on the same team
    /// </summary>
    /// <param name="id1">1st photon view</param>
    /// <param name="id2">2nd photon view</param>
    /// <returns>well are they?</returns>
    bool issameteam (int id1,int id2)
    {
        if (PhotonNetwork.inRoom)
        {
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
            List<int> team1 = LobbyScreenController.getteam1();
            List<int> team2 = LobbyScreenController.getteam2();
            if (team1.Contains(id1) && team1.Contains(id2)) return true;
            else if (team2.Contains(id1) && team2.Contains(id2)) return true;
            else return false;
        }
        else return false;
    }
    
    /// <summary>
    /// Reset things upon level load.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="scenemode"></param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (this != null && this.gameObject != null)
        {

            locview = GetComponent<PhotonView>();
            locaudio = GetComponent<AudioSource>();
            if (locview) locview.onSerializeTransformOption = OnSerializeTransform.All;
            transformview = GetComponent<PhotonTransformView>();
            if (locview) locview.World = GameObject.Find("Objects");
            if (transformview) transformview.World = GameObject.Find("Objects");
            if (scene.name != "mptest")
            {
                curchannel = VoiceChannel.All;
                locview.ingame = false;
            }
            else
            {
                if(crossvar == null) if (GameObject.Find("CrossLevelVariables")) crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
                if (crossvar.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crossvar.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip) curchannel = VoiceChannel.TeamOnly;
                else curchannel = VoiceChannel.All;
                if (PhotonNetwork.playerList.Length == 2) curchannel = VoiceChannel.All;
                locview.ingame = true;
            }
        }
    }

    /// <summary>
    /// Sets the voice channel for all clients.
    /// </summary>
    /// <param name="input"></param>
    public void setchannelforall (VoiceChannel input)
    {
        locview.RPC("SetCurrentChannel", PhotonTargets.All, (int)input);
    }

    /// <summary>
    /// networked function that sets the voice channel locally.
    /// </summary>
    /// <param name="input"></param>
    [PunRPC]
    void SetCurrentChannel(int input)
    {
        if (input == 0) curchannel = VoiceChannel.Muteall;
        else if (input == 1) curchannel = VoiceChannel.TeamOnly;
        else if (input == 2) curchannel = VoiceChannel.All;
    }
  
}
