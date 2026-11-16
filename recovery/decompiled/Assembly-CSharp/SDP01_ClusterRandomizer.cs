using System.Collections.Generic;
using UnityEngine;

public class SDP01_ClusterRandomizer : MonoBehaviour
{
	private Transform myTransform;

	private Transform[] childTransforms;

	private Vector3[] originalLocalPositions;

	private Vector3[] originalLocalRotationEulers;

	public bool ClusterFloats;

	private bool clusterFloatSetup;

	public float DriftSpeedMIN;

	public float DriftSpeedMAX;

	public float DriftRotateSpeedMIN;

	public float DriftRotateSpeedMAX;

	private List<SDP01_DriftObject> objectsToDrift;

	private void Awake()
	{
		myTransform = gameObject.transform;
		childTransforms = gameObject.GetComponentsInChildren<Transform>();
		originalLocalPositions = new Vector3[childTransforms.Length];
		originalLocalRotationEulers = new Vector3[childTransforms.Length];
		if (childTransforms.Length > 0)
		{
			for (int i = 0; i < childTransforms.Length; i++)
			{
				ref Vector3 reference = ref originalLocalPositions[i];
				reference = childTransforms[i].localPosition;
				ref Vector3 reference2 = ref originalLocalRotationEulers[i];
				reference2 = childTransforms[i].localRotation.eulerAngles;
			}
		}
	}

	public void RandomizePositionAndRotation(float scatterDistanceMIN, float scatterDistanceMAX)
	{
		if (childTransforms.Length > 0)
		{
			for (int i = 0; i < childTransforms.Length; i++)
			{
				childTransforms[i].localPosition = originalLocalPositions[i];
				childTransforms[i].localRotation = Quaternion.Euler(originalLocalRotationEulers[i]);
			}
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (childTransforms.Length > 0)
		{
			for (int j = 0; j < childTransforms.Length; j++)
			{
				zero = childTransforms[j].localPosition;
				zero2 = childTransforms[j].localRotation.eulerAngles;
				float num = Random.Range(scatterDistanceMIN, scatterDistanceMAX);
				float x = Random.Range(-75f, 75f);
				float y = Random.Range(-75f, 75f);
				float z = Random.Range(-75f, 75f);
				Vector3 vector = childTransforms[j].position - myTransform.position;
				zero += vector * num;
				zero2 = new Vector3(x, y, z);
				childTransforms[j].localPosition = zero;
				childTransforms[j].localRotation = Quaternion.Euler(zero2);
			}
		}
	}

	private void Update()
	{
		if (!ClusterFloats || clusterFloatSetup)
		{
			return;
		}
		objectsToDrift = new List<SDP01_DriftObject>();
		if (childTransforms.Length > 0)
		{
			for (int i = 0; i < childTransforms.Length; i++)
			{
				if (Random.Range(0, 100) < 35)
				{
					SDP01_DriftObject sDP01_DriftObject = childTransforms[i].gameObject.AddComponent<SDP01_DriftObject>();
					if (sDP01_DriftObject != null)
					{
						objectsToDrift.Add(sDP01_DriftObject);
					}
				}
			}
		}
		if (objectsToDrift.Count > 0)
		{
			for (int j = 0; j < objectsToDrift.Count; j++)
			{
				objectsToDrift[j].DriftSpeedX = Random.Range(DriftSpeedMIN, DriftSpeedMAX);
				objectsToDrift[j].DriftSpeedY = Random.Range(DriftSpeedMIN, DriftSpeedMAX);
				objectsToDrift[j].DriftSpeedZ = Random.Range(DriftSpeedMIN, DriftSpeedMAX);
				objectsToDrift[j].DriftRotateSpeedX = Random.Range(DriftRotateSpeedMIN, DriftRotateSpeedMAX);
				objectsToDrift[j].DriftRotateSpeedY = Random.Range(DriftRotateSpeedMIN, DriftRotateSpeedMAX);
				objectsToDrift[j].DriftRotateSpeedZ = Random.Range(DriftRotateSpeedMIN, DriftRotateSpeedMAX);
				objectsToDrift[j].Setup = true;
			}
		}
		clusterFloatSetup = true;
	}
}
