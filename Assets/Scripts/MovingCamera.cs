using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Camera panning horizontally in the City game scene 
 */
public class MovingCamera : MonoBehaviour
{
    public float moveSpeed;
    private bool isRightDir;

    // Start is called before the first frame update
    void Start()
    {
        isRightDir = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRightDir) {
            transform.position += Vector3.right * Time.deltaTime * moveSpeed;
        } else { // move left 
            transform.position += Vector3.right * Time.deltaTime * -moveSpeed;
        }

        // camera move range is set to 50 
        if(transform.position.x >= 50) {
            isRightDir = false;
        } else if(transform.position.x < 0) {
            isRightDir = true;
        }
    }
}
