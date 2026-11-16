using System.Collections.Generic;
using UnityEngine;

public class SDP01_StarshipGraveyard : MonoBehaviour
{
	public GameObject DebrisFieldPrefab;

	public float scatterAmount = 50f;

	public int NumberOfDebrisFieldsX = 4;

	public int NumberOfDebrisFieldsZ = 4;

	private int totalDebrisFieldCount;

	public float OffsetMIN = -15f;

	public float OffsetMAX = 15f;

	private List<Vector3> possiblePositions;

	private void Start()
	{
		possiblePositions = new List<Vector3>();
		totalDebrisFieldCount = NumberOfDebrisFieldsX * NumberOfDebrisFieldsZ;
		float num = 0f - (float)NumberOfDebrisFieldsX * scatterAmount / 2f;
		float num2 = 0f - (float)NumberOfDebrisFieldsZ * scatterAmount / 2f;
		for (int i = 0; i < NumberOfDebrisFieldsX; i++)
		{
			for (int j = 0; j < NumberOfDebrisFieldsZ; j++)
			{
				Vector3 item = new Vector3(num + (float)i * scatterAmount, 0f, num2 + (float)j * scatterAmount);
				possiblePositions.Add(item);
			}
		}
		for (int k = 0; k < totalDebrisFieldCount; k++)
		{
			GenerateDebrisField(possiblePositions[k], k);
		}
	}

	private void GenerateDebrisField(Vector3 fieldPosition, int debrisIndex)
	{
		fieldPosition.x += Random.Range(OffsetMIN, OffsetMAX);
		fieldPosition.y = Random.Range(OffsetMIN, OffsetMAX);
		fieldPosition.z += Random.Range(OffsetMIN, OffsetMAX);
		GameObject gameObject = Object.Instantiate(DebrisFieldPrefab, fieldPosition, Quaternion.identity);
		gameObject.name = "DebrisField_" + debrisIndex;
		gameObject.transform.parent = base.gameObject.transform;
	}
}
