using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    /// <summary>
    /// Triggers on a scale event start
    /// </summary>
    public System.Action<float> OnScaleStart { get; set; }

    /// <summary>
    /// Triggers during a scale event, multiple times
    /// </summary>
    public System.Action<float, float> OnScale { get; set; }

    /// <summary>
    /// Triggers when a scale event ends
    /// </summary>
    public System.Action<float> OnScaleEnd { get; set; }

    /// <summary>
    /// Triggers on a pan event start
    /// </summary>
    public System.Action<Vector3> OnStartPan { get; set; }

    /// <summary>
    /// Triggers during a pan event, multiple times
    /// </summary>
    public System.Action<Vector3> OnPan { get; set; }

    /// <summary>
    /// Triggers when a pan event ends
    /// </summary>
    public System.Action<Vector3> OnEndPan { get; set; }

    /// <summary>
    /// Returns the left paddle
    /// </summary>
    public GameObject GetLeftPaddle {
        get
        {
            return InputImplementation.GetLeftPaddle();
        }
    }

    /// <summary>
    /// Returns the right paddle
    /// </summary>
    public GameObject GetRightPaddle {
        get
        {
            return InputImplementation.GetrightPaddle();
        }
    }

    public aInput InputImplementation;

}
