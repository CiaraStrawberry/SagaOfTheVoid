using UnityEngine;

namespace simple;

public class SerializedMonoBehaviour : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	public string _serializedGuid;
}
