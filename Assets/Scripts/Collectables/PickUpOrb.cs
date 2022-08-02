using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpOrb : MonoBehaviour
{

    public float rotation = -0.3f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotation);
    }


    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {

            this.gameObject.SetActive(false);

            this.transform.parent.SendMessage("OnPickUp");

        }
    }

}
