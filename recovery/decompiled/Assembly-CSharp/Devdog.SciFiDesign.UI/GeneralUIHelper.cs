using Devdog.General;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

public class GeneralUIHelper : MonoBehaviour
{
	public void DebugLog(string msg)
	{
		Debug.Log(msg, gameObject);
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

	public void PlayAudioClip(AudioClip clip)
	{
		AudioClipInfo audioClipInfo = new AudioClipInfo();
		audioClipInfo.audioClip = clip;
		audioClipInfo.loop = false;
		audioClipInfo.pitch = 1f;
		audioClipInfo.volume = 1f;
		AudioManager.AudioPlayOneShot(audioClipInfo);
	}

	public void SwapSprite(Sprite sprite)
	{
		GetComponent<Image>().sprite = sprite;
	}

	public void LoadLevel(int id)
	{
		SceneUtility.LoadScene(id);
	}

	public void LoadLevel(string scene)
	{
		SceneUtility.LoadScene(scene);
	}
}
