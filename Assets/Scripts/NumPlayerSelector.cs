using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NumPlayerSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numPlayerTxt;
    [SerializeField] private Slider numPlayerSlider;
    public static int NumPlayer = 2;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        numPlayerTxt.text = NumPlayer.ToString();
        numPlayerSlider.value = (float)NumPlayer;
    }

    public void SetNumPlayer()
    {
        NumPlayer = (int)numPlayerSlider.value;
        numPlayerTxt.text = NumPlayer.ToString();
    }
}
