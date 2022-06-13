using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotate : MonoBehaviour
{
    private bool isGoingRight;
    // Start is called before the first frame update
    void Start()
    {
        isGoingRight = true;
        StartCoroutine(ChangeDirection());
    }

    // Update is called once per frame
    void Update()
    {
        if(isGoingRight) {
            transform.Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
        }else {
            transform.Rotate (new Vector3 (0, 0, -45) * Time.deltaTime);
        }
    }

    IEnumerator ChangeDirection() {
        while(true) {
            yield return new WaitForSeconds(7.0f);
            isGoingRight = !isGoingRight;
        }

    }
}
