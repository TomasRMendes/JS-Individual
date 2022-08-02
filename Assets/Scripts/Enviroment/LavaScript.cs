using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{

    public float delay = 1.5f;
    public float damage = 40;
    public bool isRegenining = false;
    private float initialDelay;

    // Start is called before the first frame update
    void Start()
    {
        initialDelay = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRegenining)
        {
            delay += Time.deltaTime;

            if(delay > initialDelay)
            {
                delay = initialDelay;
                isRegenining = false;
            }
        }


    }


    public void OnTriggerEnter(Collider other)
    {
        isRegenining = false;


    }

    public void OnTriggerStay(Collider other)
    {
        
        if(delay > 0)
        {
            delay -= Time.deltaTime;

        }
        else
        {
            if (other.CompareTag("Player"))
            {
                StarterAssets.FirstPersonController player = other.GetComponent<StarterAssets.FirstPersonController>();
                player.Damage(damage * Time.deltaTime);
            }


        }
    }


    public void OnTriggerExit(Collider other)
    {
        isRegenining = true;
    }



}
