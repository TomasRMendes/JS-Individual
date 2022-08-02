using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformGrab : MonoBehaviour
{

    public GameObject Platform;

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
        if (other.CompareTag("Player")) Debug.Log("test");
       // other.transform.parent.SetParent(Platform.transform, true);
    }

    public void OnTriggerExit(Collider other)
    {
        //other.transform.parent.SetParent(null, true);
    }


}
