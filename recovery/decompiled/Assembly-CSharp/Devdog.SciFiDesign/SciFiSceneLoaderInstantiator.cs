using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class SciFiSceneLoaderInstantiator : MonoBehaviour
{
	[Required]
	public SciFiSceneLoaderHelper helper;

	public string sceneToLoad;

	public void Instantiate()
	{
		SciFiSceneLoaderHelper sciFiSceneLoaderHelper = Object.Instantiate(helper);
		sciFiSceneLoaderHelper.LoadScene(sceneToLoad);
	}
}
