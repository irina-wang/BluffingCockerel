using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPlayer : MonoBehaviour
{
    public GameObject Player;
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject == Player){
            Player.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject == Player){
            Player.transform.parent = null;
        }
    }
}
