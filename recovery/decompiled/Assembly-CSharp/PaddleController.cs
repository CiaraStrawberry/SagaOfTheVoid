using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.UI;

public class PaddleController : TrueSyncBehaviour
{
	private const byte INPUT_KEY_HORIZONTAL = 1;

	private const float MARGIN = 0.1f;

	public int maxX = 4;

	[AddTracking]
	public int speedX = 1;

	public GameObject paddleCountPrefab;

	private Text paddleCountText;

	[AddTracking]
	private int score;

	public static Dictionary<bool, PaddleController> paddlesBySide = new Dictionary<bool, PaddleController>();

	public override void OnSyncedStart()
	{
		paddleCountText = Object.Instantiate(paddleCountPrefab).GetComponent<Text>();
		paddleCountText.transform.SetParent(GameObject.Find("Canvas").transform, worldPositionStays: false);
		if (owner.Id == 1)
		{
			Material material = Resources.Load("RedMatter", typeof(Material)) as Material;
			GetComponent<MeshRenderer>().material = material;
			tsRigidBody2D.position = new TSVector2(0, 8);
			paddlesBySide[true] = this;
		}
		else
		{
			tsRigidBody2D.position = new TSVector2(0, -8);
			paddlesBySide[false] = this;
		}
	}

	private void Update()
	{
		if (paddleCountText != null)
		{
			paddleCountText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
		}
	}

	public override void OnSyncedInput()
	{
		int value = 0;
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			value = -1;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			value = 1;
		}
		TrueSyncInput.SetInt(1, value);
	}

	public override void OnSyncedUpdate()
	{
		int num = TrueSyncInput.GetInt(1);
		TSVector2 position = tsRigidBody2D.position;
		if (num < 0)
		{
			position.x -= (FP)speedX;
		}
		else if (num > 0)
		{
			position.x += (FP)speedX;
		}
		if (FP.Abs(position.x) <= (float)maxX + 0.1f)
		{
			tsRigidBody2D.position = position;
		}
	}

	public void Score()
	{
		score++;
		paddleCountText.text = score + string.Empty;
	}
}
