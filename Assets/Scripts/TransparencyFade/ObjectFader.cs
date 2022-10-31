using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{

    FindObjsToFade hitList;
    public bool faded;

    void Awake()
    {
        // Find Camera's list of collisions hit, 
        hitList = GameObject.FindWithTag("MainCamera").GetComponent<FindObjsToFade>();
        faded = false;


    }

    public void Fade()
    {
        //Debug.Log(transform.name + " should be faded");
        faded = true;

        Renderer objectRenderer = GetComponent<Renderer>();

        foreach (Material material in objectRenderer.materials)
        {
            MaterialObjectFade.MakeFade(material);

            Color newColor = material.color;
            newColor.a = 0.3f;
            material.color = newColor;
        }
    }

    void Update ()
    {
        // So long as we are faded, keep checking if we are still inside the list to be faded, if not, unfade. 
        if (faded)
        {
            bool stayFaded = false;
            int i = 0;
            int max = hitList.numOfHits;

            foreach (RaycastHit hit in hitList.hits)
            {
                
                if (transform == hit.transform)
                {
                    stayFaded = true;
                }
                
                if (i > max){
                    break;
                }
                if (hit.collider != null){
                    i++;
                }
            }
            
            faded = stayFaded;
            if (!stayFaded){
                Unfade();
            }
        }

        
    }

    private void Unfade()
    {
        //Debug.Log("Unfading " + transform.name);

        Renderer objectRenderer = GetComponent<Renderer>();
        foreach (Material material in objectRenderer.materials)
        {
            
            MaterialObjectFade.MakeOpaque(material);
   
            Color newColor = material.color;
            newColor.a = 1f;
            material.color = newColor;
        }
    }

    public bool IsFade()
    {
        return faded;
    }

}