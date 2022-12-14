using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCamTrigger : MonoBehaviour
{
    // Code that makes a trigger cause the camera to afix to a certain position.
    private CameraSwitchScript cam;
    [SerializeField] private int viewCam = 0;
    void Start()
    {
        cam = GameObject.Find("StateDrivenCamera").GetComponent<CameraSwitchScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        cam.PauseTurn(viewCam);
    }

    private void OnTriggerExit(Collider other)
    {
        cam.UnpauseTurn();
    }
}