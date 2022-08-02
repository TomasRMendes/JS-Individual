using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript1 : MonoBehaviour
{

    public float damage = 50;
    public float fireRate = 1;
    public float actionTime = 1.5f;
    public float secondaryCooldown = 6;
    public int delayMultiplier = 5;

    private StarterAssets.FirstPersonController Player;
    private GameObject CameraReference;
    private WeaponScript ws;
    public AudioSource SpinSfx;
    public AudioSource DelaySfx;


    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<StarterAssets.FirstPersonController>();
        CameraReference = GameObject.FindGameObjectWithTag("MainCamera");
        ws = this.GetComponent<WeaponScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SecondaryFire()
    {
        SpinSfx.Play();
        //playAnimation

        Invoke(nameof(DelayedShot), actionTime - 0.05f);

    }

    public void DelayedShot()
    {
        Ray bullet = new Ray(CameraReference.transform.position, CameraReference.transform.forward);
        Physics.Raycast(bullet, out RaycastHit hit, 1000f, 7);

        ws.showBullet();

        DelaySfx.Play();

        if (hit.collider.CompareTag("HitBox"))
        {
            if (hit.collider.transform.parent.CompareTag("Enemy"))
            {
                damage *= delayMultiplier;
                bool killConfirm = hit.collider.GetComponentInParent<EnemyBehaviour>().OnHit(damage);

                Player.OnHitEffects(damage);
                if (killConfirm)
                {
                    Player.SecondaryCooldown = 0;
                }
            }
        }
    }


}
