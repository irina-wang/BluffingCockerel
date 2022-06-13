using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] NumPlayerSelector numPlayerSelector;

    public void LaunchGameScene() {
        SceneManager.LoadScene("PrototypeLevel");
    }

    public void LaunchLevelTwo() {
        SceneManager.LoadScene("LevelTwo");
    }

    public void LaunchLevelThree() {
        SceneManager.LoadScene("LevelThree");
    }

    public void LaunchLevelSelection() {
        SceneManager.LoadScene("LevelSelection");
    }

    public void LaunchMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetNumPlayer() {
        numPlayerSelector.SetNumPlayer();
    }

    // actual levels
    public void LaunchLavaLevel() {
        SceneManager.LoadScene("Lava Level");
    }

    public void LaunchMovingLevel() {
        SceneManager.LoadScene("MovingLevel");
    }

    public void LaunchRotatingLevel() {
        SceneManager.LoadScene("RotatingLevel");
    }

    public void LaunchWaterWheelLevel() {
        SceneManager.LoadScene("WaterWheelLevel");
    }
}
