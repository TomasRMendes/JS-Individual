using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroBox : MonoBehaviour
{
    public EnemyBehaviour eb;


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
            eb.Aggro = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            eb.Aggro = false;
        }
    }

}
