using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringGun : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private int playerNum;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        StartCoroutine(ShootBullet());

    }

    IEnumerator ShootBullet() {
        while(true) {
            GameObject bull = Instantiate(bullet, transform.position + transform.TransformDirection(Vector3.right * 2), transform.rotation);
            bull.GetComponent<Bullet>().playerNum = playerNum;
            bull.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.right * 20);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
