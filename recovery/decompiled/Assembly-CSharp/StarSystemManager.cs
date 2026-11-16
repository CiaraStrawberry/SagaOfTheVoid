using System.Collections.Generic;
using System.Linq;
using Galaxia;
using UnityEngine;
using UnityEngine.UI;

public class StarSystemManager : MonoBehaviour
{
	public Galaxy galaxy;

	public StarFinder finder;

	public Sprite SystemTexture;

	public float SystemGUISize = 64f;

	[SerializeField]
	[HideInInspector]
	public List<GameObject> Systems;

	private string[] Names = new string[35]
	{
		"Albireo", "Cygnus X-1", "16 Cygni", "Kelu-1", "Pismis 24-1", "Castor", "40 Eridani", "Polaris", "Solaris", "TV Crateris",
		"Alpha Centauri", "Anus", "Avalon", "Darwin IV", "Far Away", "Gaia", "Gor", "Hydros", "K-PAX", "Kerbin",
		"Krull", "LittleBigPlanet", "LV-1201", "LV-426", "New Terra", "Omicron Persei 8", "Pandora", "Hoth", "Planet X", "Ragnarok",
		"Rosetta", "Terminus", "Titan", "Zeist", "Mustafar"
	};

	private void Start()
	{
		Galaxia.Random.seed = galaxy.Particles[0].Prefab.Seed;
		Systems = new List<GameObject>();
		List<Galaxia.Particle> list = finder.Find(galaxy.Particles[0], Names.Length);
		List<string> list2 = Names.OrderBy((string n) => Galaxia.Random.Next()).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			int planets = Galaxia.Random.Next(0, 5);
			GameObject gameObject = new GameObject("System", typeof(RectTransform), typeof(Image));
			gameObject.GetComponent<RectTransform>().sizeDelta = Vector3.one * SystemGUISize;
			gameObject.transform.position = list[num].position;
			gameObject.transform.rotation = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f));
			Image component = gameObject.GetComponent<Image>();
			component.sprite = SystemTexture;
			SystemGUICircle systemGUICircle = gameObject.AddComponent<SystemGUICircle>();
			systemGUICircle.Size = SystemGUISize;
			gameObject.transform.SetParent(transform, worldPositionStays: true);
			systemGUICircle.SetPlanets(planets);
			StarSystem starSystem = gameObject.AddComponent<StarSystem>();
			starSystem.Name = ((num >= list2.Count) ? "Unknown" : list2[num]);
			starSystem.Planets = planets;
			Systems.Add(gameObject);
		}
	}
}
