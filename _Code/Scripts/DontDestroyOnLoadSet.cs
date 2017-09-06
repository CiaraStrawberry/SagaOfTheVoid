using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class just sets the object it is attached to to not destroy on load.
/// </summary>
public class DontDestroyOnLoadSet : MonoBehaviour {
	void Start () {
        DontDestroyOnLoad(this.gameObject);
	}
}
