using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class just returns the object to world root on start.
/// </summary>
public class KillParentOnStartScript : MonoBehaviour {
	void Start () {
        transform.parent = null;
	}
}
