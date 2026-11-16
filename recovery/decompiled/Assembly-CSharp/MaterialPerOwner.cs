using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MaterialPerOwner : Photon.MonoBehaviour
{
	private int assignedColorForUserId;

	private Renderer m_Renderer;

	private void Start()
	{
		m_Renderer = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (photonView.ownerId != assignedColorForUserId)
		{
			m_Renderer.material = PlayerVariables.GetMaterial(m_Renderer.material, photonView.ownerId);
			assignedColorForUserId = photonView.ownerId;
		}
	}
}
