using System.Collections.Generic;
using UnityEngine;

public class SDP01_WreckageController : MonoBehaviour
{
	private Transform myTransform;

	public bool RandomizeDebris;

	public bool DebrisFloats = true;

	public float DriftSpeedMIN = -0.05f;

	public float DriftSpeedMAX = 0.05f;

	public float DriftRotateSpeedMIN = -1f;

	public float DriftRotateSpeedMAX = 1f;

	public GameObject[] DebrisClusterPrefabs;

	public float ClusterScatterDistanceMIN = 2.5f;

	public float ClusterScatterDistanceMAX = 5.5f;

	public bool GenerateDebrisClouds = true;

	public int NumberOfCloudsMIN = 1;

	public int NumberOfCloudsMAX = 4;

	public GameObject[] DebrisCloudPrefabs;

	private List<GameObject> debrisCloudGOs;

	public int NumberOfClusters;

	private SDP01_ClusterRandomizer[] clusterScripts;

	public bool SpawnDebrisPieces = true;

	public int NumberDebrisPiecesMIN = 5;

	public int NumberDebrisPiecesMAX = 15;

	public float PieceDistanceMIN = 8f;

	public float PieceDistanceMAX = 20f;

	public float PieceScaleMIN = 0.5f;

	public float PieceScaleMAX = 1.5f;

	public GameObject[] DebrisPiecePrefabs;

	public Transform PiecePlacementTransform;

	public int DebrisPieceCount;

	private List<GameObject> debrisPiecesGOList;

	public bool SpawnDebrisQuadPieces = true;

	public int NumberDebrisQuadPiecesMIN = 5;

	public int NumberDebrisQuadPiecesMAX = 15;

	public float QuadPieceDistanceMIN = 8f;

	public float QuadPieceDistanceMAX = 20f;

	public float QuadPieceScaleMIN = 0.5f;

	public float QuadPieceScaleMAX = 1.5f;

	public GameObject[] DebrisQuadPiecePrefabs;

	public int DebrisQuadPieceCount;

	private List<GameObject> debrisQuadPiecesGOList;

	private void DoSpawnDebrisPieces()
	{
		if (PiecePlacementTransform != null && DebrisPiecePrefabs.Length > 0)
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			float num = 0f;
			float num2 = 0f;
			int num3 = Random.Range(NumberDebrisPiecesMIN, NumberDebrisPiecesMAX);
			for (int i = 0; i < num3; i++)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				zero = myTransform.position;
				PiecePlacementTransform.position = zero;
				vector.x = Random.Range(-360f, 360f);
				vector.y = Random.Range(-360f, 360f);
				vector.z = Random.Range(-360f, 360f);
				num = Random.Range(PieceDistanceMIN, PieceDistanceMAX);
				PiecePlacementTransform.rotation = Quaternion.Euler(vector);
				PiecePlacementTransform.position += PiecePlacementTransform.forward * num;
				zero = PiecePlacementTransform.position;
				vector.x = Random.Range(-360f, 360f);
				vector.y = Random.Range(-360f, 360f);
				vector.z = Random.Range(-360f, 360f);
				zero2 = vector;
				num2 = Random.Range(PieceScaleMIN, PieceScaleMAX);
				SpawnDebrisPiece(zero, zero2, i, num2);
			}
			DebrisPieceCount = debrisPiecesGOList.Count;
		}
	}

	private void SpawnDebrisPiece(Vector3 piecePosition, Vector3 pieceRotationEulers, int pieceNumber, float pieceScale)
	{
		int num = Random.Range(0, DebrisPiecePrefabs.Length);
		GameObject gameObject = Object.Instantiate(DebrisPiecePrefabs[num], piecePosition, Quaternion.Euler(pieceRotationEulers));
		gameObject.name = "DebrisPiece_" + pieceNumber;
		gameObject.transform.parent = myTransform;
		gameObject.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
		debrisPiecesGOList.Add(gameObject);
	}

	private void DoSpawnDebrisQuadPieces()
	{
		if (PiecePlacementTransform != null && DebrisQuadPiecePrefabs.Length > 0)
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			float num = 0f;
			float num2 = 0f;
			int num3 = Random.Range(NumberDebrisQuadPiecesMIN, NumberDebrisQuadPiecesMAX);
			for (int i = 0; i < num3; i++)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				zero = myTransform.position;
				PiecePlacementTransform.position = zero;
				vector.x = Random.Range(-360f, 360f);
				vector.y = Random.Range(-360f, 360f);
				vector.z = Random.Range(-360f, 360f);
				num = Random.Range(PieceDistanceMIN, PieceDistanceMAX);
				PiecePlacementTransform.rotation = Quaternion.Euler(vector);
				PiecePlacementTransform.position += PiecePlacementTransform.forward * num;
				zero = PiecePlacementTransform.position;
				vector.x = Random.Range(-360f, 360f);
				vector.y = Random.Range(-360f, 360f);
				vector.z = Random.Range(-360f, 360f);
				zero2 = vector;
				num2 = Random.Range(QuadPieceScaleMIN, QuadPieceScaleMAX);
				SpawnDebrisQuadPiece(zero, zero2, i, num2);
			}
			DebrisQuadPieceCount = debrisQuadPiecesGOList.Count;
		}
	}

	private void SpawnDebrisQuadPiece(Vector3 piecePosition, Vector3 pieceRotationEulers, int pieceNumber, float pieceScale)
	{
		int num = Random.Range(0, DebrisQuadPiecePrefabs.Length);
		GameObject gameObject = Object.Instantiate(DebrisQuadPiecePrefabs[num], piecePosition, Quaternion.Euler(pieceRotationEulers));
		gameObject.name = "DebrisQuadPiece_" + pieceNumber;
		gameObject.transform.parent = myTransform;
		gameObject.transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
		debrisQuadPiecesGOList.Add(gameObject);
	}

	private void Start()
	{
		myTransform = gameObject.transform;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (DebrisClusterPrefabs.Length > 0)
		{
			for (int i = 0; i < DebrisClusterPrefabs.Length; i++)
			{
				if (Random.Range(0, 100) < 35)
				{
					float x = Random.Range(-10f, 10f);
					float y = Random.Range(-10f, 10f);
					float z = Random.Range(-10f, 10f);
					float x2 = Random.Range(-180f, 180f);
					float y2 = Random.Range(-180f, 180f);
					float z2 = Random.Range(-180f, 180f);
					zero = new Vector3(x, y, z);
					zero2 = new Vector3(x2, y2, z2);
					GenerateDebrisCluster(i, zero, zero2);
				}
			}
		}
		RandomizeDebris = true;
		clusterScripts = gameObject.GetComponentsInChildren<SDP01_ClusterRandomizer>();
		NumberOfClusters = clusterScripts.Length;
		if (DebrisFloats && clusterScripts.Length > 0)
		{
			for (int j = 0; j < clusterScripts.Length; j++)
			{
				clusterScripts[j].DriftSpeedMIN = DriftSpeedMIN;
				clusterScripts[j].DriftSpeedMAX = DriftSpeedMAX;
				clusterScripts[j].DriftRotateSpeedMIN = DriftRotateSpeedMIN;
				clusterScripts[j].DriftRotateSpeedMAX = DriftRotateSpeedMAX;
				clusterScripts[j].ClusterFloats = true;
			}
		}
		if (GenerateDebrisClouds)
		{
			debrisCloudGOs = new List<GameObject>();
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			if (DebrisCloudPrefabs.Length > 0)
			{
				int num = Random.Range(NumberOfCloudsMIN, NumberOfCloudsMAX);
				for (int k = 0; k < num; k++)
				{
					float x3 = Random.Range(-5f, 5f);
					float y3 = Random.Range(-5f, 5f);
					float z3 = Random.Range(-5f, 5f);
					float x4 = Random.Range(-180f, 180f);
					float y4 = Random.Range(-180f, 180f);
					float z4 = Random.Range(-180f, 180f);
					zero3 = new Vector3(x3, y3, z3);
					zero4 = new Vector3(x4, y4, z4);
					GenerateDebrisCloud(k, zero3, zero4);
				}
			}
		}
		if (SpawnDebrisPieces)
		{
			debrisPiecesGOList = new List<GameObject>();
			DoSpawnDebrisPieces();
		}
		if (SpawnDebrisQuadPieces)
		{
			debrisQuadPiecesGOList = new List<GameObject>();
			DoSpawnDebrisQuadPieces();
		}
	}

	private void Update()
	{
		if (!RandomizeDebris)
		{
			return;
		}
		if (GenerateDebrisClouds)
		{
			if (debrisCloudGOs.Count > 0)
			{
				for (int i = 0; i < debrisCloudGOs.Count; i++)
				{
					Object.Destroy(debrisCloudGOs[i]);
				}
				debrisCloudGOs.Clear();
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (DebrisCloudPrefabs.Length > 0)
			{
				int num = Random.Range(NumberOfCloudsMIN, NumberOfCloudsMAX);
				for (int j = 0; j < num; j++)
				{
					float x = Random.Range(-5f, 5f);
					float y = Random.Range(-5f, 5f);
					float z = Random.Range(-5f, 5f);
					float x2 = Random.Range(-180f, 180f);
					float y2 = Random.Range(-180f, 180f);
					float z2 = Random.Range(-180f, 180f);
					zero = new Vector3(x, y, z);
					zero2 = new Vector3(x2, y2, z2);
					GenerateDebrisCloud(j, zero, zero2);
				}
			}
		}
		Vector3 zero3 = Vector3.zero;
		Vector3 zero4 = Vector3.zero;
		if (clusterScripts.Length > 0)
		{
			for (int k = 0; k < clusterScripts.Length; k++)
			{
				float x3 = Random.Range(-10f, 10f);
				float y3 = Random.Range(-10f, 10f);
				float z3 = Random.Range(-10f, 10f);
				float x4 = Random.Range(-180f, 180f);
				float y4 = Random.Range(-180f, 180f);
				float z4 = Random.Range(-180f, 180f);
				zero3 = new Vector3(x3, y3, z3);
				zero4 = new Vector3(x4, y4, z4);
				clusterScripts[k].gameObject.transform.localPosition = zero3;
				clusterScripts[k].gameObject.transform.localRotation = Quaternion.Euler(zero4);
				clusterScripts[k].RandomizePositionAndRotation(ClusterScatterDistanceMIN, ClusterScatterDistanceMAX);
			}
		}
		if (SpawnDebrisPieces)
		{
			if (debrisPiecesGOList.Count > 0)
			{
				for (int l = 0; l < debrisPiecesGOList.Count; l++)
				{
					Object.Destroy(debrisPiecesGOList[l]);
				}
				debrisPiecesGOList.Clear();
			}
			DoSpawnDebrisPieces();
		}
		RandomizeDebris = false;
	}

	private void GenerateDebrisCluster(int debrisIndex, Vector3 debrisLocalPosition, Vector3 clusterLocalRotationEulers)
	{
		GameObject gameObject = Object.Instantiate(DebrisClusterPrefabs[debrisIndex], myTransform.position, myTransform.rotation);
		gameObject.name = "DebrisCluster_" + debrisIndex;
		gameObject.transform.parent = myTransform;
		gameObject.transform.localPosition = debrisLocalPosition;
		gameObject.transform.localRotation = Quaternion.Euler(clusterLocalRotationEulers);
	}

	private void GenerateDebrisCloud(int cloudIndex, Vector3 cloudLocalPosition, Vector3 cloudLocalRotationEulers)
	{
		int num = Random.Range(0, DebrisCloudPrefabs.Length);
		GameObject gameObject = Object.Instantiate(DebrisCloudPrefabs[num], myTransform.position, myTransform.rotation);
		gameObject.name = "DebrisCloud_" + cloudIndex;
		gameObject.transform.parent = myTransform;
		gameObject.transform.localPosition = cloudLocalPosition;
		gameObject.transform.localRotation = Quaternion.Euler(cloudLocalRotationEulers);
		debrisCloudGOs.Add(gameObject);
	}
}
