using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    public GameObject[] playerObjs;
    public GameObject[] playerStats;
    public TextMeshProUGUI[] scoreTxts;
    public TextMeshProUGUI timerTxt;
    public Sprite[] playerImgs;
    public GameObject resultOverlay;
    public Image winnerImg;
    public float height = -5;
    public float gameRound = 20.0f;
    public bool isMovingLevel = false;
    private static int numPlayer;
    private static int numActive;
    private static int[] scores;
    private static bool turnEnd;
    private float time;

    // for pause menu
    public static bool GameisPaused = false;
    public static bool HowToMenuIsActive = false;
    public GameObject pauseMenuUI;
    public GameObject howToMenuUI;
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
        // pause menu
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        howToMenuUI.SetActive(false);
        GameisPaused = false;

        numPlayer = NumPlayerSelector.NumPlayer;
        numActive = numPlayer;
        scores = new int[numPlayer];
        for (int i = 0; i < numPlayer; i++) {
        scoreTxts[i].text = scores[i].ToString();
        }
        for (int i = numPlayer; i < 4; i++) {
        playerObjs[i].SetActive(false);
        playerStats[i].SetActive(false);
        }
        turnEnd = false;
        resultOverlay.SetActive(false);
        time = gameRound;
        timerTxt.text = Mathf.Floor(time).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // pause menu
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (GameisPaused && !HowToMenuIsActive){
                Resume();
            } else if (!GameisPaused && !HowToMenuIsActive) {
                Pause();
            }
        }

        if (!turnEnd) {
            for (int i = 0; i < numPlayer; i++) {
                if (playerObjs[i].activeSelf && playerObjs[i].transform.position.y < height)
                {
                    PlayerGG(i);
                }else if(isMovingLevel && Mathf.Abs(playerObjs[i].transform.position.x - Camera.main.transform.position.x) > 17) {
                    PlayerGG(i);
                }
            }
            // if (numActive <= 1) {
            //     // GG Reset
            //     StartCoroutine(GG());
            // }

            time -= Time.deltaTime;
            timerTxt.text = Mathf.Floor(time).ToString();

            if(time <= 0) {
                StartCoroutine(GG());
            }
        }
    }

    private IEnumerator GG() {
        for (int i = 0; i < numPlayer; i++) {
            if (playerObjs[i].activeSelf) {
                // scores[i]++;
                // scoreTxts[i].text = scores[i].ToString();
                if(scores[i] == scores.Max()) {
                    winnerImg.sprite = playerImgs[i];

                }
            }
        }
        time = gameRound;
        turnEnd = true;
        resultOverlay.SetActive(true);
        yield return new WaitForSeconds(3);
        resultOverlay.SetActive(false);
        turnEnd = false;

        for (int i = 0; i < numPlayer; i++) {
            playerObjs[i].SetActive(true);
            playerObjs[i].GetComponent<PlayerController1>().ResetPlayer(isMovingLevel);
        }
        numActive = numPlayer;
    }

    public void PlayerGG(int num) {
        int killingPlayer = playerObjs[num].GetComponent<PlayerController1>().hitBy;
        if(killingPlayer == -1) {
            scores[num]--;
            scoreTxts[num].text = scores[num].ToString();
        }else {
            scores[killingPlayer]++;
            scoreTxts[killingPlayer].text = scores[killingPlayer].ToString();
        }
        playerObjs[num].GetComponent<PlayerController1>().ResetPlayer(isMovingLevel);
        // numActive--;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameisPaused = true;
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameisPaused = false;
    }

    public void SetLevel (float sliderValue){
        mixer.SetFloat("MusicVolume", Mathf.Log10 (sliderValue) * 20);
        volumeLevel = sliderValue;
    }

    public void RestartGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenControlsMenu() {
        HowToMenuIsActive = true;
        pauseMenuUI.SetActive(false);
        howToMenuUI.SetActive(true);
    }

    public void CloseControlsMenu() {
        HowToMenuIsActive = false;
        pauseMenuUI.SetActive(true);
        howToMenuUI.SetActive(false);
    }
}
