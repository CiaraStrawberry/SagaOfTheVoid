using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[AddComponentMenu("Photon Networking/Photon Transform View")]
public class PhotonTransformView : MonoBehaviour, IPunObservable
{
	[SerializeField]
	private PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

	[SerializeField]
	private PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

	[SerializeField]
	private PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

	private PhotonTransformViewPositionControl m_PositionControl;

	private PhotonTransformViewRotationControl m_RotationControl;

	private PhotonTransformViewScaleControl m_ScaleControl;

	private PhotonView m_PhotonView;

	private bool m_ReceivedNetworkUpdate;

	private bool m_firstTake;

	public GameObject World;

	public Vector3 nextpos;

	private void Awake()
	{
		m_PhotonView = GetComponent<PhotonView>();
		m_PositionControl = new PhotonTransformViewPositionControl(m_PositionModel);
		m_RotationControl = new PhotonTransformViewRotationControl(m_RotationModel);
		m_ScaleControl = new PhotonTransformViewScaleControl(m_ScaleModel);
	}

	private void OnEnable()
	{
		m_firstTake = true;
	}

	private void Update()
	{
		if (!(m_PhotonView == null) && !m_PhotonView.isMine && PhotonNetwork.connected)
		{
			UpdatePosition();
			UpdateRotation();
			UpdateScale();
		}
	}

	private void UpdatePosition()
	{
		if (m_PositionModel.SynchronizeEnabled && m_ReceivedNetworkUpdate)
		{
			if (World == null)
			{
				World = GameObject.Find("Objects");
			}
			if (!m_PhotonView.ingame)
			{
				transform.localPosition = m_PositionControl.UpdatePosition(transform.localPosition);
			}
			else
			{
				transform.position = World.transform.TransformPoint(m_PositionControl.GetNetworkPosition());
			}
		}
	}

	private void UpdateRotation()
	{
		if (m_RotationModel.SynchronizeEnabled && m_ReceivedNetworkUpdate)
		{
			transform.localRotation = m_RotationControl.GetRotation(transform.localRotation);
		}
	}

	private Vector3 calculatelocalscale()
	{
		Debug.Log("recieved :" + m_ScaleControl.GetNetworkScale());
		return new Vector3(1f / m_ScaleControl.GetNetworkScale().x, 1f / m_ScaleControl.GetNetworkScale().y, 1f / m_ScaleControl.GetNetworkScale().z);
	}

	private void UpdateScale()
	{
		if (m_ScaleModel.SynchronizeEnabled && m_ReceivedNetworkUpdate)
		{
			if (!m_PhotonView.ingame)
			{
				transform.localScale = m_ScaleControl.GetScale(transform.localScale);
			}
			else
			{
				transform.localScale = calculatelocalscale();
			}
		}
	}

	public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
	{
		m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		m_PositionControl.OnPhotonSerializeView(transform.localPosition, stream, info);
		m_RotationControl.OnPhotonSerializeView(transform.localRotation, stream, info);
		m_ScaleControl.OnPhotonSerializeView(transform.localScale, stream, info);
		if (!m_PhotonView.isMine && m_PositionModel.DrawErrorGizmo)
		{
			DoDrawEstimatedPositionError();
		}
		if (!stream.isReading)
		{
			return;
		}
		m_ReceivedNetworkUpdate = true;
		if (m_firstTake)
		{
			m_firstTake = false;
			if (m_PositionModel.SynchronizeEnabled)
			{
				nextpos = m_PositionControl.GetNetworkPosition();
			}
			if (m_RotationModel.SynchronizeEnabled)
			{
				transform.localRotation = m_RotationControl.GetNetworkRotation();
			}
			if (m_ScaleModel.SynchronizeEnabled)
			{
				transform.localScale = m_ScaleControl.GetNetworkScale();
			}
		}
	}

	private void DoDrawEstimatedPositionError()
	{
		Vector3 vector = m_PositionControl.GetNetworkPosition();
		if (transform.parent != null)
		{
			vector = transform.parent.position + vector;
		}
		Debug.DrawLine(vector, transform.position, Color.red, 2f);
		Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green, 2f);
		Debug.DrawLine(vector, vector + Vector3.up, Color.red, 2f);
	}
}
