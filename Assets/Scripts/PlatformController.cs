using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private GameObject[] platforms;
    [SerializeField] private float time_interval = 3;
    private List<Animation> animations;
    private float time_elapsed = 0;

    void Start()
    {
        animations = new List<Animation>();
        for (int i = 0; i < platforms.Length; i++) {
            animations.Add(platforms[i].GetComponent<Animation>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        time_elapsed += Time.deltaTime;
        if (time_elapsed > time_interval) {
            List<int> static_platforms = new List<int>();
            for (int i = 0; i < animations.Count; i++) {
                if (!animations[i].IsPlaying("platform_animation" + (i+1).ToString())) {
                    static_platforms.Add(i);
                }
            }
            if (static_platforms.Count > 1) {
                int index = static_platforms[Random.Range(0, static_platforms.Count)];
                animations[index].Play("platform_animation" + (index+1).ToString());
            }
        time_elapsed = 0;
        }
    }
}
