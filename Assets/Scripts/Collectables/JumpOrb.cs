using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpOrb : MonoBehaviour
{

    private StarterAssets.FirstPersonController Player;
    private bool isActive = true;
    public float rotation = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<StarterAssets.FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotation);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(Player.AirBoost == 0 && isActive)
            {
                Player.AirBoost++;
                isActive = false;
                this.gameObject.SetActive(false);
                Invoke(nameof(Reactivate), 2);
            }

        }
    }
    
    private void Reactivate()
    {
        this.gameObject.SetActive(true);
        isActive = true;
    }

}