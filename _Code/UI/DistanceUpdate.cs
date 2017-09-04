using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceUpdate : MonoBehaviour {

    public int DistanceMultiplier = 250;
    Camera m_Camera;
    Text m_Text;

	// Use this for initialization
	void Start () {
        // Get main camera
        // if no camera referenced, grab the main camera
        if (!m_Camera)
            m_Camera = Camera.main;

        if (!m_Text)
            m_Text = GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {

        // Correct distance as needed (for now)
     //   double distance = Vector3.Distance(m_Camera.transform.position, transform.position) * DistanceMultiplier;

        // Update on ship
      //  m_Text.text = distance.ToString("N2") + "m";

	}
}
