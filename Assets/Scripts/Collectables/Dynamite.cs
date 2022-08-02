using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPickUp()
    {

        GameObject Player = FindObjectOfType<StarterAssets.FirstPersonController>().gameObject;

        Player.GetComponent<StarterAssets.FirstPersonController>().GrabBomb();

        this.gameObject.SetActive(false);

    }


}
