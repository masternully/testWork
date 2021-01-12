using System.Collections;
using System;
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
    DateTime currentDate;
    DateTime oldDate;

    public IslandScroll islandScroll;
    public AudioClass[] audioClasses;
    public string[] islandNames;
    public int[] islandPeopleAmount;
    public int[] islandCosts;

    public int coins;
    public int diamonds;
    public bool passiveMoney;

    public bool[] availableIslands;
    public Text[] islandInfo;
    public GameObject islandInfoObj;
    public static int curIsland;
    private float timer30m = -1;
    public Image loading;
    public GameObject toIslandButton;

    void Start(){
        availableIslands = new bool[islandCosts.Length];
        availableIslands[0] = true;
        curIsland = PlayerPrefs.GetInt("curIsland");
        if(curIsland == 0){
            BackToScroll();
        }
        //PlayerPrefs.SetInt("coins", 10000);
        //PlayerPrefs.SetInt("diamonds", 10000);
        if(PlayerPrefs.GetInt("coins") == 0 && PlayerPrefs.GetInt("diamonds") == 0){
            coins = 2000;
            diamonds = 300;
        }else{
            coins = PlayerPrefs.GetInt("coins");
            diamonds = PlayerPrefs.GetInt("diamonds");
        }
        currentDate = System.DateTime.Now;
 
         //Grab the old time from the player prefs as a long
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));
 
         //Convert the old time from binary to a DataTime variable
        DateTime oldDate = DateTime.FromBinary(temp);
 
         //Use the Subtract method and store the result as a timespan variable
        TimeSpan difference = currentDate.Subtract(oldDate);
        Debug.Log(difference.TotalSeconds);
        int min = (int) difference.TotalSeconds / 60;
        for(int i = 0; i < curIsland; i++){
            for(int z = 0; z < min; z++){
                coins += 1000;
            }
        }
        Debug.Log(curIsland);
        Debug.Log(min);
        RefreshUI();
    }

    public void BuyNewIsland(Button but){
        if(coins >= islandCosts[curIsland]){
        backToScroll.SetActive(false);
        curIsland++;
        PlayerPrefs.SetInt("curIsland", curIsland);
        coins -= islandCosts[curIsland];
        RefreshUI();
        but.gameObject.transform.parent.gameObject.SetActive(false);
        islandInfoObj.SetActive(true);
        boostButton.SetActive(true);
        timer30m = 0;
        }
    }

    public void ChangeIslandInfo(){
        islandInfo[0].text = islandNames[IslandScroll.curIsland];
        islandInfo[1].text = "ЖИТЕЛЕЙ: " + islandPeopleAmount[IslandScroll.curIsland];
        findNewIsland.transform.GetComponentInChildren<Text>().text = islandCosts[IslandScroll.curIsland] + "";
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
        takeWithBoost.SetActive(false);
        boosted = true;
        diamonds-=150;
        RefreshUI();
        // Минус затраченные очки
    }

    public void TakePrize(){
        acceptPrize.SetActive(true);
        islandInfoObj.SetActive(false);
    }

    public void GetPrize(){
        acceptPrize.SetActive(false);
        boostButton.SetActive(false);
        prize.SetActive(false);
        islandInfoObj.SetActive(false);
        startMeditation.SetActive(true);
        coins+=100;
        RefreshUI();
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

    public bool scrolling;

    public void BackToScroll(){
        scrolling = true;
        findNewIsland.SetActive(false);
        toIslandButton.SetActive(true);
        backToScroll.SetActive(false);
        islandScroll.islands[IslandScroll.curIsland].transform.GetChild(1).gameObject.SetActive(false);
    }

    public GameObject findNewIsland;
    public GameObject backToScroll;

    public void EnableBuying(){
        scrolling = false;
        islandScroll.islands[IslandScroll.curIsland].transform.GetChild(1).gameObject.SetActive(true);
        toIslandButton.SetActive(false);
        islandInfoObj.SetActive(true);
        backToScroll.SetActive(true);

        if(availableIslands[IslandScroll.curIsland] == false){
            findNewIsland.SetActive(true);
        }
    }

    string strMin1;
    string strSec1;
    public Text fullTime;
    public GameObject downloading;
    public bool boosted;

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

            if(counting > 5){ //curTrack.length
                count = false;
                counting = 0;
                scrolling = true;
                wereStarted = false;
                savedPauseButton.SetActive(false);
                playButton.SetActive(true);
                playMenu.SetActive(false);
                startMeditation.SetActive(false);
                availableIslands[curIsland] = true;
                IslandScroll.prevIsland = curIsland;
                IslandScroll.curIsland++;
                curIsland++;
                islandScroll.ChooseIsland();
            }
        }
        if(timer30m >= 0){
            timer30m += Time.deltaTime;
            if(!boosted)
                loading.fillAmount+=Time.deltaTime/1800;
            else
                loading.fillAmount+=Time.deltaTime;

            if(loading.fillAmount == 1){
                boosted = false;
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

            passiveMoneyTimer += Time.deltaTime;
            if(passiveMoneyTimer > 60){
                passiveMoneyTimer = 0;
                for(int i = 0; i < curIsland; i++){
                    coins += 1000;
                }
                RefreshUI();
            }
    }

    public void RefreshUI(){
        coinsText.text = coins + "";
        PlayerPrefs.SetInt("coins", coins);
        diamondText.text = diamonds + "";
        PlayerPrefs.SetInt("diamonds", diamonds);
    }

    public float passiveMoneyTimer;
    public Text coinsText;
    public Text diamondText;

    void OnApplicationQuit()
    {
         //Savee the current system time as a string in the player prefs class
        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
 
        print("Saving this date to prefs: " + System.DateTime.Now);
    }
}
