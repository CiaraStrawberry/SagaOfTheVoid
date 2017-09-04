using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valve.VR;

public class ViveInputHandler : aInput {

    private enum eButtonStates {
        Pressed,
        Released
    }

    public SteamVR_ControllerManager SteamController;
    private GameObject _LeftPaddle;
    private GameObject _RightPaddle;

    private VRTK.VRTK_ControllerEvents _LeftDeviceEvents;
    private VRTK.VRTK_ControllerEvents _RightDeviceEvents;

    private InputController _InputController;

    eButtonStates ScaleButtonPressed = eButtonStates.Released;

    private float _PreviousDistance = 0;

    /// <summary>
    /// Gets the left paddle
    /// </summary>
    /// <returns></returns>
    public override GameObject GetLeftPaddle()
    {
        return _LeftPaddle;
    }

    /// <summary>
    /// Gets the right paddle
    /// </summary>
    /// <returns></returns>
    public override GameObject GetrightPaddle()
    {
        return _RightPaddle;
    }

    // Use this for initialization
    void Start() {

        Debug.Log("Start");

        // Get the input controller
        _InputController = gameObject.GetComponent<InputController>();

        _LeftPaddle = SteamController.left;
        _RightPaddle = SteamController.right;

        Debug.Log("Left: " + _LeftPaddle);
        Debug.Log("Right: " + _RightPaddle);

        _LeftDeviceEvents = _LeftPaddle.GetComponent<VRTK.VRTK_ControllerEvents>();
        _RightDeviceEvents = _RightPaddle.GetComponent<VRTK.VRTK_ControllerEvents>();

    }

    // Update is called once per frame
    void Update() {

        #region Scale Handing

        // State is unpressed, and two triggers pressed
        if (ScaleButtonPressed == eButtonStates.Released
            && _LeftDeviceEvents.triggerClicked
            && _RightDeviceEvents.triggerClicked) {

            // Switch state to pressed
            ScaleButtonPressed = eButtonStates.Pressed;

            Debug.Log(_LeftPaddle);
            Debug.Log(_RightPaddle);

            // Get distance
            float dist = Vector3.Distance(_LeftPaddle.transform.position, _RightPaddle.transform.position);

            _PreviousDistance = dist;

            // Send pressed event
            if (_InputController.OnScaleStart != null) { _InputController.OnScaleStart(dist); }

        }

        // State is pressed, send event
        else if (ScaleButtonPressed == eButtonStates.Pressed
            && _LeftDeviceEvents.triggerClicked
            && _RightDeviceEvents.triggerClicked) {

            float dir = 0;

            // Get distance
            float dist = Vector3.Distance(_LeftPaddle.transform.position, _RightPaddle.transform.position);

            // Determine direction
            dir = (dist < _PreviousDistance) ? -0.03f : 0.03f;

            // Set the previous
            _PreviousDistance = dist;

            // Send scale event
            if (_InputController.OnScaleStart != null) { _InputController.OnScale(dist, dir); }

        }

        // State is releasing (if either trigger releases, the state is done
        else if (ScaleButtonPressed == eButtonStates.Pressed
            && (!_LeftDeviceEvents.triggerClicked
            || !_RightDeviceEvents.triggerClicked)) {

            // Release the event
            ScaleButtonPressed = eButtonStates.Released;

            // Get distance
            float dist = Vector3.Distance(_LeftPaddle.transform.position, _RightPaddle.transform.position);

            _PreviousDistance = 0;

            // Send scale event
            if (_InputController.OnScaleStart != null) { _InputController.OnScaleEnd(dist); }

        }

        #endregion

    }
    
}

