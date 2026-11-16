using System;
using System.Runtime.InteropServices;
using Oculus.Avatar;
using UnityEngine;

public class OvrAvatarAssetMesh : OvrAvatarAsset
{
	public Mesh mesh;

	private ovrAvatarSkinnedMeshPose skinnedBindPose;

	public string[] jointNames;

	public OvrAvatarAssetMesh(ulong _assetId, IntPtr asset)
	{
		assetID = _assetId;
		ovrAvatarMeshAssetData ovrAvatarMeshAssetData2 = CAPI.ovrAvatarAsset_GetMeshData(asset);
		mesh = new Mesh();
		mesh.name = "Procedural Geometry for asset " + _assetId;
		long num = ovrAvatarMeshAssetData2.vertexCount;
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector4[] array3 = new Vector4[num];
		Vector2[] array4 = new Vector2[num];
		BoneWeight[] array5 = new BoneWeight[num];
		long num2 = Marshal.SizeOf(typeof(ovrAvatarMeshVertex));
		long num3 = ovrAvatarMeshAssetData2.vertexBuffer.ToInt64();
		for (long num4 = 0L; num4 < num; num4++)
		{
			long num5 = num2 * num4;
			ovrAvatarMeshVertex ovrAvatarMeshVertex2 = (ovrAvatarMeshVertex)Marshal.PtrToStructure(new IntPtr(num3 + num5), typeof(ovrAvatarMeshVertex));
			ref Vector3 reference = ref array[num4];
			reference = new Vector3(ovrAvatarMeshVertex2.x, ovrAvatarMeshVertex2.y, 0f - ovrAvatarMeshVertex2.z);
			ref Vector3 reference2 = ref array2[num4];
			reference2 = new Vector3(ovrAvatarMeshVertex2.nx, ovrAvatarMeshVertex2.ny, 0f - ovrAvatarMeshVertex2.nz);
			ref Vector4 reference3 = ref array3[num4];
			reference3 = new Vector4(ovrAvatarMeshVertex2.tx, ovrAvatarMeshVertex2.ty, 0f - ovrAvatarMeshVertex2.tz, ovrAvatarMeshVertex2.tw);
			ref Vector2 reference4 = ref array4[num4];
			reference4 = new Vector2(ovrAvatarMeshVertex2.u, ovrAvatarMeshVertex2.v);
			array5[num4].boneIndex0 = ovrAvatarMeshVertex2.blendIndices[0];
			array5[num4].boneIndex1 = ovrAvatarMeshVertex2.blendIndices[1];
			array5[num4].boneIndex2 = ovrAvatarMeshVertex2.blendIndices[2];
			array5[num4].boneIndex3 = ovrAvatarMeshVertex2.blendIndices[3];
			array5[num4].weight0 = ovrAvatarMeshVertex2.blendWeights[0];
			array5[num4].weight1 = ovrAvatarMeshVertex2.blendWeights[1];
			array5[num4].weight2 = ovrAvatarMeshVertex2.blendWeights[2];
			array5[num4].weight3 = ovrAvatarMeshVertex2.blendWeights[3];
		}
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array4;
		mesh.tangents = array3;
		mesh.boneWeights = array5;
		skinnedBindPose = ovrAvatarMeshAssetData2.skinnedBindPose;
		ulong num6 = ovrAvatarMeshAssetData2.indexCount;
		short[] array6 = new short[num6];
		IntPtr indexBuffer = ovrAvatarMeshAssetData2.indexBuffer;
		Marshal.Copy(indexBuffer, array6, 0, (int)num6);
		int[] array7 = new int[num6];
		for (ulong num7 = 0uL; num7 < num6; num7 += 3)
		{
			array7[num7 + 2] = array6[num7];
			array7[num7 + 1] = array6[num7 + 1];
			array7[num7] = array6[num7 + 2];
		}
		mesh.triangles = array7;
		int jointCount = skinnedBindPose.jointCount;
		jointNames = new string[jointCount];
		for (int i = 0; i < jointCount; i++)
		{
			jointNames[i] = Marshal.PtrToStringAnsi(skinnedBindPose.jointNames[i]);
		}
	}

	public SkinnedMeshRenderer CreateSkinnedMeshRendererOnObject(GameObject target)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = target.AddComponent<SkinnedMeshRenderer>();
		skinnedMeshRenderer.sharedMesh = mesh;
		mesh.name = "AvatarMesh_" + assetID;
		int jointCount = skinnedBindPose.jointCount;
		GameObject[] array = new GameObject[jointCount];
		Transform[] array2 = new Transform[jointCount];
		Matrix4x4[] array3 = new Matrix4x4[jointCount];
		for (int i = 0; i < jointCount; i++)
		{
			array[i] = new GameObject();
			array2[i] = array[i].transform;
			array[i].name = jointNames[i];
			int num = skinnedBindPose.jointParents[i];
			if (num == -1)
			{
				array[i].transform.parent = skinnedMeshRenderer.transform;
				skinnedMeshRenderer.rootBone = array[i].transform;
			}
			else
			{
				array[i].transform.parent = array[num].transform;
			}
			Vector3 position = skinnedBindPose.jointTransform[i].position;
			position.z = 0f - position.z;
			array[i].transform.localPosition = position;
			Quaternion orientation = skinnedBindPose.jointTransform[i].orientation;
			orientation.x = 0f - orientation.x;
			orientation.y = 0f - orientation.y;
			array[i].transform.localRotation = orientation;
			array[i].transform.localScale = skinnedBindPose.jointTransform[i].scale;
			ref Matrix4x4 reference = ref array3[i];
			reference = array[i].transform.worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
		}
		skinnedMeshRenderer.bones = array2;
		mesh.bindposes = array3;
		return skinnedMeshRenderer;
	}
}
