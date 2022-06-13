using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int playerNum;

    private float timealive;
    // Start is called before the first frame update
    void Start()
    {
        playerNum = -1;
        timealive = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timealive += Time.deltaTime;

        if(timealive > 0.5f) {
            Destroy(gameObject);
        }
        
    }

}
