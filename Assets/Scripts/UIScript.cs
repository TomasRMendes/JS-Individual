using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Slider Health;
    public Slider Rally;
    public Slider CD;
    public GameObject[] Bombs;
    private int nBombs = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject i in Bombs)
        {
            i.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(float health)
    {
        Health.value = health;
    }
    public void UpdateRally(float rally)
    {
        Rally.value = rally;
    }

    public void UpdateCD(float cd)
    {
        CD.value = cd;
    }


    public void AddBomb()
    {
        if (nBombs >= 4) return;
        Bombs[nBombs++].SetActive(true);
    }




}
