using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
      
        locview = GetComponent<PhotonView>();
     //   DontDestroyOnLoad(this.gameObject);
        locaudio = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        GameObject[] voices = GameObject.FindGameObjectsWithTag("Voice");
        foreach (GameObject voice in voices) if (voice.GetComponent<PhotonView>().owner == PhotonNetwork.player) localplayer = voice.GetComponent<PlayerPrefabVoiceControlScript>();
       
        OnLevelFinishedLoading(SceneManager.GetActiveScene(),new LoadSceneMode());
   //   if( GameObject.Find("Controllers"))  unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        if (locview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
        //  locview.transforms 
       if(GameObject.Find("CrossLevelVariables")) crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
    }

    // Update is called once per frame
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
    PhotonTransformView transformview;
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (this != null && this.gameObject != null)
        {

            //   if (PhotonNetwork.room == null) Destroy(this.gameObject);
            locview = GetComponent<PhotonView>();
            locaudio = GetComponent<AudioSource>();
            if (locview) locview.onSerializeTransformOption = OnSerializeTransform.All;
            transformview = GetComponent<PhotonTransformView>();
            if (locview) locview.World = GameObject.Find("Objects");
            if (transformview) transformview.World = GameObject.Find("Objects");
            //   if (GameObject.Find("Controllers")) localplayer = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
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
    public void setchannelforall (VoiceChannel input)
    {
        locview.RPC("SetCurrentChannel", PhotonTargets.All, (int)input);
    }
    [PunRPC]
    void SetCurrentChannel(int input)
    {
        if (input == 0) curchannel = VoiceChannel.Muteall;
        else if (input == 1) curchannel = VoiceChannel.TeamOnly;
        else if (input == 2) curchannel = VoiceChannel.All;
    }
  
}
