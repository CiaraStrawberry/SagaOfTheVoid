using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script points the object towords the camera.
/// </summary>
public class BillBoardMe : MonoBehaviour
{
    public Camera m_Camera;
    void Awake()
    {
        if (!m_Camera)
            m_Camera = Camera.main;
    }

    void Update()
    {
        if (m_Camera)
        {
            var camForward = Quaternion.LookRotation(m_Camera.transform.forward).eulerAngles;
            transform.rotation = Quaternion.Euler(camForward.x, camForward.y, 0);
        }
    }
}
