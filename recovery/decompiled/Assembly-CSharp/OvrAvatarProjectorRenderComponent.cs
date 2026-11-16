using System.Collections.Generic;
using UnityEngine;

public class OvrAvatarProjectorRenderComponent : OvrAvatarRenderComponent
{
	private Material material;

	internal void InitializeProjectorRender(ovrAvatarRenderPart_ProjectorRender render, OvrAvatarRenderComponent target)
	{
		material = CreateAvatarMaterial(gameObject.name + "_projector", physicallyBasedShader: false, selfOccluding: false);
		material.EnableKeyword("PROJECTOR_ON");
		Renderer component = target.GetComponent<Renderer>();
		if (component != null)
		{
			List<Material> list = new List<Material>(component.sharedMaterials);
			list.Add(material);
			component.sharedMaterials = list.ToArray();
		}
	}

	internal void UpdateProjectorRender(ovrAvatarRenderPart_ProjectorRender render)
	{
		OvrAvatar.ConvertTransform(render.localTransform, transform);
		material.SetMatrix("_ProjectorWorldToLocal", transform.worldToLocalMatrix);
		UpdateAvatarMaterial(material, render.materialState);
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-1f, -1f, -1f));
		Vector3 vector2 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(1f, -1f, -1f));
		Vector3 vector3 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-1f, 1f, -1f));
		Vector3 vector4 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(1f, 1f, -1f));
		Vector3 vector5 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-1f, -1f, 1f));
		Vector3 vector6 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(1f, -1f, 1f));
		Vector3 vector7 = transform.localToWorldMatrix.MultiplyPoint(new Vector3(-1f, 1f, 1f));
		Vector3 to = transform.localToWorldMatrix.MultiplyPoint(new Vector3(1f, 1f, 1f));
		Gizmos.color = Color.gray;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, to);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector5, vector7);
		Gizmos.DrawLine(vector7, to);
		Gizmos.DrawLine(vector6, to);
	}
}
