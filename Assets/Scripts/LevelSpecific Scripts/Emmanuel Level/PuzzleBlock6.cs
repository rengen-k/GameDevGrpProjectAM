using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBlock6 : Triggerable
{
    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject ring;
    [SerializeField] private GameObject[] oldButtons;
    [SerializeField] private GameObject[] newButtons;
    private bool isTrigger = true;
    private bool rotate = false;
    private Quaternion nextRotation;
    private Quaternion nextRotationRing2;
    private float degree = 0;
    private float degreeRing2 = 0;
    public string tag;
    private int turns = 0;
    private int turns2 = 250;


    void Start()
    {
        nextRotation = transform.rotation * Quaternion.Euler(0, 0, degree);
    }

    void Update()
    {
        if (turns >= 360)
        {
            turns -= 360;
        } else if (turns <= -360)
        {
            turns += 360;
        }

        if (turns2 >= 360)
        {
            turns2 -= 360;
        } else if (turns2 <= -360)
        {
            turns2 += 360;
        }

        if ((turns == -180) && (turns2 == 160))
        {
            cube.SetActive(true);
            foreach (GameObject i in oldButtons)
            {
                i.SetActive(false);
            }

            foreach (GameObject j in newButtons)
            {
                j.SetActive(true);
            }
        }

        if (rotate) {
            transform.rotation = Quaternion.Slerp(transform.rotation, nextRotation, Time.deltaTime * 8);
            ring.transform.rotation = Quaternion.Slerp(ring.transform.rotation, nextRotationRing2, Time.deltaTime * 8);

            if (transform.rotation == nextRotation) {
                rotate = false;
            }
        }
    }

    public override void triggerAct()
    {
        if (isTrigger) {
            isTrigger = false;
            if (tag == "IncreaseButton")
            {
                degree = -30;
                degreeRing2 = 15;
                turns -= 30;
                turns2 += 15;

                print(turns);
                print(turns2);
            } else if (tag == "DecreaseButton")
            {
                degree = -30;
                degreeRing2 = 15;
                turns -= 30;
                turns2 += 15;

                print(turns);
                print(turns2);
            }

            if (rotate) {
                nextRotation = nextRotation * Quaternion.Euler(0, 0, degree);
                nextRotationRing2 = nextRotationRing2 * Quaternion.Euler(0, 0, degreeRing2);
            } else {
                nextRotation = transform.rotation * Quaternion.Euler(0, 0, degree);
                nextRotationRing2 = ring.transform.rotation * Quaternion.Euler(0, 0, degreeRing2);
            }
            rotate = true;
        }
    }

    public override void triggerUnAct()
    {
        isTrigger = true;
    }
}