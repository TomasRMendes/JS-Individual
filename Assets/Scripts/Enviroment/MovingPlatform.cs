using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public GameObject Platform;
    public GameObject[] Positions;
    public float Speed = 1;
    public int start = 0;
    public bool WarpToStart = false;

    private float Progress = 0;
    private int next;

    // Start is called before the first frame update
    void Start()
    {
        next = (start + 1) % Positions.Length;
    }

    // Update is called once per frame
    void Update()
    {

        Progress += Time.deltaTime / 100 * Speed;

        if(Progress >= Mathf.PI)
        {
            Progress %= Mathf.PI;
            start++;
            next++;
            start %= Positions.Length;
            next %= Positions.Length;
            if(WarpToStart && next == 0)
            {
                start = 0;
                next++;
            }
        }

        Platform.transform.position = Vector3.Lerp(Positions[start].transform.position, Positions[next].transform.position, -Mathf.Cos(Progress) /2 + 0.5f);

    }



}
