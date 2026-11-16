using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Excelsior/ExcelsiorUtils")]
public class ExcelsiorUtils : MonoBehaviour
{
	public GameObject target;

	public AudioSource audioSource;

	public AudioClip audioClip;

	public Text textContent;

	public Slider slider;

	private void Start()
	{
	}

	private void playSound()
	{
		if ((bool)audioSource && audioSource.enabled)
		{
			audioSource.clip = audioClip;
		}
		audioSource.Play();
	}

	private void Update()
	{
	}

	public void SetPosition(int _scaleValue)
	{
		target.transform.position = Vector3.one * _scaleValue;
		playSound();
	}

	public void SetPositionAndScaleTo(GameObject _object)
	{
		target.transform.localPosition = _object.transform.localPosition * -1f;
		target.transform.localScale = _object.transform.localScale;
		playSound();
	}

	public void SetUniformScale(int _scaleValue)
	{
		target.transform.localScale = Vector3.one * _scaleValue;
		playSound();
	}

	public void SetUniformWidthHeight(int _widthHeightValue)
	{
		target.GetComponent<Image>().rectTransform.sizeDelta = Vector3.one * _widthHeightValue;
		playSound();
	}

	public void SetImageColor(int _colorID)
	{
		Color32 color = _colorID switch
		{
			1 => (Color32)Color.blue, 
			2 => (Color32)Color.red, 
			_ => (Color32)Color.yellow, 
		};
		target.GetComponent<Image>().color = color;
		playSound();
	}

	public void OpenWebPage()
	{
		Application.OpenURL("http://www.assetstore.unity3d.com/#!/content/69794");
		playSound();
	}

	public void GetSliderValue_ToText()
	{
		textContent.text = slider.value.ToString();
	}
}
