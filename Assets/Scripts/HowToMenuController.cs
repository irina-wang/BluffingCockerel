using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class HowToMenuController : MonoBehaviour
{

    public static bool MenuIsActive = false;
    public GameObject menuUI;
    public AudioMixer mixer;
    public static float volumeLevel = 1.0f;
    private Slider sliderVolumeCtrl;

    void Awake (){
        SetLevel (volumeLevel);
        GameObject sliderTemp = GameObject.FindWithTag("PauseMenuSlider");
        if (sliderTemp != null) {
            sliderVolumeCtrl = sliderTemp.GetComponent<Slider>();
            sliderVolumeCtrl.value = volumeLevel;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        menuUI.SetActive(false);
        MenuIsActive = false;
    }

    public void OpenMenu() {
        menuUI.SetActive(true);
        MenuIsActive = true;
    }

    public void CloseMenu() {
        menuUI.SetActive(false);
        MenuIsActive = false;
    }

    public void SetLevel (float sliderValue){
        mixer.SetFloat("MusicVolume", Mathf.Log10 (sliderValue) * 20);
        volumeLevel = sliderValue;
    }
}
