using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
  public GameObject[] items;

  public float x_min;
  public float x_max;
  public float y;
  public float z;
  public float time_interval = 10;

  private float spawn_count;

  // Start is called before the first frame update
  void Start()
  {
    spawn_count = time_interval;
  }

  // Update is called once per frame
  void Update()
  {
    spawn_count -= Time.deltaTime;
    if (spawn_count <= 0) {
      int index = Random.Range(0, items.Length);
      float item_x = Random.Range(x_min, x_max);
      Instantiate(items[index], new Vector3(item_x, y, z), Quaternion.identity);
      spawn_count = time_interval;
    }
  }
}
