using UnityEngine;
using System.Collections;

public class WorldScale : MonoBehaviour {

    public InputController InputController;

    public Vector3 MinScale = new Vector3(0.07f, 0.07f, 0.07f);
    public Vector3 MaxScale = new Vector3(10f, 10f, 10f);

    private double CurrentDistance;

    public void Awake() {
    }

    // Use this for initialization
    void Start() {
        /*
        GameObject.Find("JoinMultiplayer").GetComponent<JoinMultiplayerGame>().spawnallships();
        // Capture events for world scaling
        InputController.OnScaleStart += OnScaleStart;
        InputController.OnScale += OnScale;
        InputController.OnScaleEnd += OnScaleEnd;
        */

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnScaleStart(float distance) {
        // Keep scaling
        Debug.Log("Start: " + distance);
    }

    private void OnScale(float distance, float direction) {
        // Keep lerping between scales
        Debug.Log("Scale: " + distance);

        gameObject.transform.localScale += new Vector3(direction, direction, direction);

        if (gameObject.transform.localScale.x < 0.07f) {
            gameObject.transform.localScale = MinScale;
        } else if (gameObject.transform.localScale.x > 10f) {
            gameObject.transform.localScale = MaxScale;
        }

    }

    private void OnScaleEnd(float distance) {
        // Stop scaling
        Debug.Log("End: " + distance);
    }

}
