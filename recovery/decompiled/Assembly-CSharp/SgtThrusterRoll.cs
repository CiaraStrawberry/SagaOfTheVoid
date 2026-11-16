using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Thruster Roll")]
public class SgtThrusterRoll : MonoBehaviour
{
	public class CameraState : SgtCameraState
	{
		public Quaternion LocalRotation;
	}

	[Tooltip("The rotation offset in degrees")]
	public Vector3 Rotation = new Vector3(0f, 90f, 90f);

	[NonSerialized]
	private List<CameraState> cameraStates;

	protected virtual void OnEnable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
	}

	protected virtual void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(CameraPreCull));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(CameraPreRender));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(CameraPostRender));
	}

	private void CameraPreCull(Camera camera)
	{
		Revert();
		Vector3 forward = transform.forward;
		Vector3 rhs = transform.position - camera.transform.position;
		Vector3 vector = Vector3.Cross(forward, rhs);
		if (vector != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation(vector, forward) * Quaternion.Euler(Rotation);
		}
		Save(camera);
	}

	private void CameraPreRender(Camera camera)
	{
		Restore(camera);
	}

	private void CameraPostRender(Camera camera)
	{
		Revert();
	}

	public void Save(Camera camera)
	{
		CameraState cameraState = SgtCameraState.Save(ref cameraStates, camera);
		cameraState.LocalRotation = transform.localRotation;
	}

	private void Restore(Camera camera)
	{
		CameraState cameraState = SgtCameraState.Restore(cameraStates, camera);
		if (cameraState != null)
		{
			transform.localRotation = cameraState.LocalRotation;
		}
	}

	public void Revert()
	{
		transform.localRotation = Quaternion.identity;
	}
}
