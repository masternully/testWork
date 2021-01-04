using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Serializable]
public class AudioClass {
    public string url;
    public string name;
}

public class MenuClass : MonoBehaviour
{
    public IslandScroll islandScroll;
    public AudioClass[] audioClasses;
    public string[] islandNames;
    public int[] islandPeopleAmount;
    public int[] islandCosts;

    public int coins;

    public bool[] availableIslands;
    public Text[] islandInfo;
    public GameObject islandInfoObj;
    private int curIsland;
    private float timer30m = -1;
    public Image loading;

    void Start(){
        availableIslands = new bool[islandCosts.Length];
    }

    public void BuyNewIsland(Button but){
        curIsland = IslandScroll.curIsland;
        coins -= islandCosts[curIsland];
        but.gameObject.transform.parent.gameObject.SetActive(false);
        islandInfoObj.SetActive(true);
        boostButton.SetActive(true);
        islandInfo[0].text = islandNames[curIsland];
        islandInfo[1].text = "ЖИТЕЛЕЙ: " + islandPeopleAmount[curIsland];
        timer30m = 0;
    }


    public GameObject takeWithBoost;
    public void SpeedUpTimer(){
        takeWithBoost.SetActive(true);
    }

    public void CloseBooster(){
        takeWithBoost.SetActive(false);
    }

    public GameObject prize;
    public GameObject boostButton;

    public GameObject acceptPrize;
    public GameObject startMeditation;
    public void TakeBoost(){
        availableIslands[IslandScroll.curIsland] = true;
        takeWithBoost.SetActive(false);
        // Минус затраченные очки
    }

    public void TakePrize(){
        acceptPrize.SetActive(true);
        islandInfoObj.SetActive(false);
    }

    public void GetPrize(){
        acceptPrize.SetActive(false);
        boostButton.SetActive(true);
        prize.SetActive(false);
        islandInfoObj.SetActive(false);
        startMeditation.SetActive(true);
        coins+=100;
        // И прописываю значения нашим валютам
    }

    public GameObject meditationMenu;
    public void GoToMeditation(){
        //startMeditation.SetActive(false);
        meditationMenu.SetActive(true);
    }

    public IslandClass[] islands;
    public AudioSource audioSource;

    public GameObject pauseButton;
    public GameObject playButton;
    public GameObject savedPauseButton;
    public bool wereStarted;
    public float counting;
    public bool count;
    public Text showLeftTime;
    private int medNum;

    IEnumerator  DownloadTrack(string SoundURL){
        UnityWebRequest wwwSound = UnityWebRequestMultimedia.GetAudioClip(SoundURL, AudioType.MPEG);
        yield return wwwSound.SendWebRequest();
        if (wwwSound.isNetworkError) 
        {
            Debug.Log (wwwSound.error);
        }
        else 
        {
            Debug.Log ("Sound Received at: " + SoundURL);
            curTrack = ((DownloadHandlerAudioClip)wwwSound.downloadHandler).audioClip;
            downloading.SetActive(false);
            audioSource.PlayOneShot(curTrack);
            count = true;

            float minutes1 = Mathf.Floor(curTrack.length / 60);
            float seconds1 = Mathf.RoundToInt(curTrack.length%60);

            strMin1 = minutes1.ToString();
            if(minutes1 < 10) {
                strMin1 = "0" + minutes1.ToString();
            }
            strSec1 = Mathf.RoundToInt(seconds1).ToString();

            if(seconds1 < 10) {
                strSec1 = "0" + Mathf.RoundToInt(seconds1).ToString();
            }
        }
    }

    public AudioClip curTrack;

    public GameObject courseMenu;


    public void OpenCourseMenu(){
        courseMenu.SetActive(true);
        meditationMenu.SetActive(false);
    }

    public GameObject playMenu;

    public void PlayTrack(Button but){
        medNum = int.Parse(but.name + "");
        if(!wereStarted){
            downloading.SetActive(true);
            wereStarted = true;
            courseMenu.SetActive(false);
            playMenu.SetActive(true);
            playButton = but.gameObject;

            Button butPause = Instantiate(pauseButton).GetComponent<Button>();
            savedPauseButton = butPause.gameObject;
            butPause.gameObject.transform.SetParent(playButton.transform.parent);
            butPause.gameObject.transform.position = playButton.transform.position;
            butPause.gameObject.transform.localScale = playButton.transform.localScale;
            butPause.onClick.AddListener(() => PauseSound());

            StartCoroutine(DownloadTrack(audioClasses[medNum].url));
            //audioSource.PlayOneShot(islands[curIsland].audios[medNum]);

            but.gameObject.SetActive(false);
        }else{
            audioSource.UnPause();
            count = true;
            savedPauseButton.SetActive(true);
            playButton.SetActive(false);
        }
    }

    public void PauseSound()
    {
        if(playButton.activeSelf == false){
            count = false;
            audioSource.Pause();
            savedPauseButton.SetActive(false);
            playButton.SetActive(true);
        }
    }

    public void CloseMeditation(){
        PauseSound();
        meditationMenu.SetActive(false);
    }

    public GameObject findNewIsland;

    public void EnableBuying(){
        findNewIsland.SetActive(true);
        findNewIsland.transform.GetComponentInChildren<Text>().text = islandCosts[IslandScroll.curIsland] + "";
    }

    string strMin1;
    string strSec1;
    public Text fullTime;
    public GameObject downloading;

    void Update(){
        if(count){
            counting += Time.deltaTime;

            float minutes = Mathf.Floor(counting / 60);
            float seconds = Mathf.RoundToInt(counting%60);
            string strMin = "";
            string strSec = "";

            strMin = minutes.ToString();
            if(minutes < 10) {
                strMin = "0" + minutes.ToString();
            }
            strSec = Mathf.RoundToInt(seconds).ToString();
            
            if(seconds < 10) {
                strSec = "0" + Mathf.RoundToInt(seconds).ToString();
            }

            showLeftTime.text = strMin + ":" + strSec;
            fullTime.text = strMin1 + ":" + strSec1;

            if(counting > curTrack.length){
                count = false;
                counting = 0;
                wereStarted = false;
                savedPauseButton.SetActive(false);
                playButton.SetActive(true);
                playMenu.SetActive(false);
                startMeditation.SetActive(false);
                IslandScroll.prevIsland = curIsland;
                curIsland++;
                IslandScroll.curIsland++;
                islandScroll.ChooseIsland();
            }
        }
        if(timer30m >= 0){
            timer30m += Time.deltaTime;
            if(!availableIslands[IslandScroll.curIsland])
                loading.fillAmount+=Time.deltaTime/1800;
            else
                loading.fillAmount+=Time.deltaTime;

            if(loading.fillAmount == 1){
                prize.SetActive(true);
                boostButton.SetActive(false);
                timer30m = -1;
                loading.fillAmount = 0;
            }
        }

        if(timer30m > 1800){
            prize.SetActive(true);
            boostButton.SetActive(false);
            timer30m = -1;
        }
    }
}
