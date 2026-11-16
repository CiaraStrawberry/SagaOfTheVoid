using System.Collections;
using UnityEngine;

namespace DynamicFogAndMist;

public class Game : MonoBehaviour
{
	private GameObject ball;

	private GUIStyle labelStyle;

	private int score;

	private int impacts;

	private int fireCount;

	private string scoreText;

	private static Game _instance;

	public static Game instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = GameObject.Find("Player");
				_instance = gameObject.GetComponent<Game>();
			}
			return _instance;
		}
	}

	private void Start()
	{
		score = 0;
		fireCount = 0;
		ball = GameObject.Find("Ball");
		ball.SetActive(value: false);
		GameObject gameObject = new GameObject("Pillars");
		GameObject original = GameObject.Find("Pillar");
		for (float num = -100f; num < 100f; num += 20f)
		{
			for (float num2 = -100f; num2 < 100f; num2 += 10f)
			{
				GameObject gameObject2 = Object.Instantiate(original);
				gameObject2.transform.SetParent(gameObject.transform);
				gameObject2.transform.position = new Vector3(num2, -30f, num);
				gameObject2.transform.localScale = new Vector3(8f, Random.Range(20f, 40f) * (Mathf.Abs(gameObject2.transform.position.x) * 0.01f + 1f), 16f);
				if (Random.value > 0.8f)
				{
					AddBonusCylinder(gameObject2.transform);
				}
			}
		}
		UpdateScoreText();
	}

	private void AddBonusCylinder(Transform pillarTransform)
	{
		Vector3 position = pillarTransform.position + Vector3.up * pillarTransform.localScale.y * 0.5f;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		Material material = Object.Instantiate(Resources.Load<Material>("Materials/DFMLambertSolidColor"));
		material.color = new Color(1f, 0.5f, 0.1f);
		gameObject.GetComponent<Renderer>().sharedMaterial = material;
		gameObject.transform.position = position;
		gameObject.transform.localScale = Vector3.one * 3f;
		gameObject.transform.SetParent(pillarTransform, worldPositionStays: true);
		gameObject.AddComponent<BonusCylinderHit>();
	}

	private void FixedUpdate()
	{
		transform.position += transform.forward * Time.deltaTime * 20f;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StartCoroutine(LaunchBall());
		}
	}

	private void OnGUI()
	{
		if (labelStyle == null)
		{
			labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.normal.textColor = Color.black;
			labelStyle.fontSize = 40;
		}
		GUI.Label(new Rect(10f, 10f, 1000f, 60f), scoreText, labelStyle);
	}

	private IEnumerator LaunchBall()
	{
		fireCount++;
		UpdateScoreText();
		GameObject ballClone = Object.Instantiate(ball);
		ballClone.transform.position = Camera.main.transform.position;
		ballClone.SetActive(value: true);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Rigidbody rb = ballClone.GetComponent<Rigidbody>();
		rb.velocity = ray.direction * 80f + Camera.main.transform.forward * 20f;
		ballClone.transform.Find("Sounds/ShootSound").GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(10f);
		Object.Destroy(ballClone);
	}

	public void AnnotateScore(int points)
	{
		score += points;
		impacts++;
		UpdateScoreText();
	}

	private void UpdateScoreText()
	{
		scoreText = "Score: " + score;
		if (fireCount > 0)
		{
			string text = scoreText;
			scoreText = text + "  Balls Fired: " + fireCount + " (" + ((float)impacts * 100f / (float)fireCount).ToString("F1") + "%)";
		}
	}
}
