using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{

    public WeaponScript1 w1;
    public WeaponScript1 w2;
    public WeaponScript1 w3;
    public int ScriptToUse = 1;

    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float fireRate;
    [HideInInspector]
    public float actionTime;
    [HideInInspector]
    public float secondaryCooldown;

    public AudioSource sfx;


    private GameObject hand;
    private StarterAssets.FirstPersonController Player;
    private GameObject CameraReference;
    public GameObject Bullet;


    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<StarterAssets.FirstPersonController>();
        hand = GameObject.FindGameObjectWithTag("Hand");
        CameraReference = GameObject.FindGameObjectWithTag("MainCamera");

        Bullet.SetActive(false);

        if(ScriptToUse == 1)
        {
            damage = w1.damage;
            fireRate = w1.fireRate;
            actionTime = w1.actionTime;
            secondaryCooldown = w1.secondaryCooldown;
        }
        else if(ScriptToUse == 2)
        {
            damage = w2.damage;
            fireRate = w2.fireRate;
            actionTime = w2.actionTime;
            secondaryCooldown = w2.secondaryCooldown;
        }
        else if(ScriptToUse == 3)
        {
            damage = w3.damage;
            fireRate = w3.fireRate;
            actionTime = w3.actionTime;
            secondaryCooldown = w3.secondaryCooldown;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnPickUp()
    {
        transform.SetParent(hand.transform);
        transform.SetPositionAndRotation(hand.transform.position, hand.transform.parent.rotation);
        transform.localPosition = new Vector3(0, 0.5f, 4.5f);
        transform.Rotate(0.0f, -90.0f, 0.0f, Space.Self);

        Player.SendMessage("ChangeWeapon");

    }


    public void PrimaryFire()
    {
        PrimaryFire(1);
    }


    public void PrimaryFire(float mult)
    {
        Ray bullet = new Ray(CameraReference.transform.position, CameraReference.transform.forward);
        Physics.Raycast(bullet, out RaycastHit hit, 1000f, 7);
        sfx.Play();

        showBullet();

        if(hit.collider.CompareTag("HitBox"))
        {
            if (hit.collider.transform.parent.CompareTag("Enemy"))
            {
                damage *= mult;
                bool killConfirm = hit.collider.GetComponentInParent<EnemyBehaviour>().OnHit(damage);

                Player.OnHitEffects(damage);

            }
        }

    }

    public void SecondaryFire()
    {
        if (ScriptToUse == 1)
            w1.SecondaryFire();
        else if (ScriptToUse == 2)
            w2.SecondaryFire();
        else if (ScriptToUse == 3)
            w3.SecondaryFire();


    }


    public void showBullet()
    {
        Bullet.SetActive(true);
        Invoke(nameof(hideBullet), 0.5f);
    }

    public void hideBullet()
    {
        Bullet.SetActive(false);
    }


}
