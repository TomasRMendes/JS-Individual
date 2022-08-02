using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarricadeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            StarterAssets.FirstPersonController player = other.GetComponent<StarterAssets.FirstPersonController>();
            if (player.bombs >= 4) Destroy(transform.parent.gameObject);

        }

    }

}
