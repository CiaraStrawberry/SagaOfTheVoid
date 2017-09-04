using UnityEngine;
using System.Collections;

public class Example_Menu_1 : aMenuState {

    // Transitioning from
    public override void OnTransitionFrom() {

    }

    // Menu is now active
    public override void OnActive() {
        this.gameObject.SetActive(true);
    }

    // Menu has been transitioned to
    public override void OnTransitionTo() {
    }

    // Remove the object
    public override void OnDeactivate() {
        this.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

}
