using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public float delay;
    public float blastRadius;
    public float blastForce;

    private float countdown;

    public bool hasExploded = false;
    PickupItem pickupScript;
    public Transform respawnPoint;

    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject timer;

    void Start()
    {
        timer.transform.parent.gameObject.GetComponent<Canvas>().worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
    }

    void OnEnable()
    {
        countdown = delay;
        pickupScript = GetComponent<PickupItem>();
        timer.SetActive(false);
    }

    void Update()
    {
        // To make camera look towards 
        Camera camera = Camera.main ;
        timer.transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);

        if (pickupScript.hasBeenThrown)
        {
            timer.SetActive(true);
            countdown -= Time.deltaTime;
            timer.GetComponent<Image>().fillAmount -= 1.0f / delay * Time.deltaTime;
        }

        // IF want to make timer change colour, can set through same way fillAmount changes; timer.GetComponent<Image>().

        if (countdown <= 0f && !hasExploded)
        {
            hasExploded = true;
            Explode();
        }
    }

    void Explode()
    {
        //this is for whenever we add an explosion effect
        // Instantiate(explosionEffect (this is a GameObject), transform.position, transform.rotation);

        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in collidersToDestroy)
        {
            Destructible dest = nearbyObject.GetComponent<Destructible>();
            if (dest != null)
            {
                dest.Destroy();
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Instantiate(explosion, transform.position, transform.rotation).GetComponent<Explosion>().bombRadious = blastRadius;
                rb.AddExplosionForce(blastForce, transform.position, blastRadius);
            }
        }

        // Removing bomb and resetting
        gameObject.SetActive(false);
        transform.position = respawnPoint.position;
        gameObject.SetActive(true);
        hasExploded = false;
        timer.GetComponent<Image>().fillAmount = 1;
    }
}
