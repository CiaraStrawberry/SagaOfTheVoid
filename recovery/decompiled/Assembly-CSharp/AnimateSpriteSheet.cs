using System.Collections;
using UnityEngine;

internal class AnimateSpriteSheet : MonoBehaviour
{
	public int Columns = 5;

	public int Rows = 5;

	public float FramesPerSecond = 10f;

	public bool RunOnce = true;

	private Material materialCopy;

	public float RunTimeInSeconds => 1f / FramesPerSecond * (float)(Columns * Rows);

	private void Start()
	{
		materialCopy = new Material(GetComponent<Renderer>().sharedMaterial);
		GetComponent<Renderer>().sharedMaterial = materialCopy;
		Vector2 value = new Vector2(1f / (float)Columns, 1f / (float)Rows);
		GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", value);
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateTiling());
	}

	private IEnumerator UpdateTiling()
	{
		float x = 0f;
		float y = 0f;
		Vector2 offset = Vector2.zero;
		do
		{
			for (int i = Rows - 1; i >= 0; i--)
			{
				y = (float)i / (float)Rows;
				for (int j = 0; j <= Columns - 1; j++)
				{
					x = (float)j / (float)Columns;
					offset.Set(x, y);
					GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
					yield return new WaitForSeconds(1f / FramesPerSecond);
				}
			}
		}
		while (!RunOnce);
	}
}
