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
    public IslandClass[] islandsObj;
    public AudioClass[] audioClasses; 
    public AudioSource audioSource;    
    public AudioClip curTrack;
    public string[] islandNames;
    public int[] islandPeopleAmount;
    public int[] islandCosts;
    public int coins;  
    private int medNum;
    private int curIsland;
    private float timer30m = -1;   
    public float counting;
    public bool[] availableIslands; 
    public bool wereStarted; 
    public bool count;
    public Text[] islandInfo;  
    public Text showLeftTimeText;   
    public Text fullTimeText;
    public GameObject islandInfoObj;
    public GameObject takeWithBoostObj;  
    public GameObject prizeObj;
    public GameObject boostButtonObj;
    public GameObject pauseButtonObj;
    public GameObject playButtonObj;
    public GameObject savedPauseButtonObj;
    public GameObject acceptPrizeObj;
    public GameObject startMeditationObj;
    public GameObject findNewIslandObj;
    public GameObject downloadingObj;   
    public GameObject meditationMenuObj;   
    public GameObject courseMenuObj;
    public GameObject playMenuObj;
    public Image loading;
   
  
 
   
  
  
    void Start(){
        availableIslands = new bool[islandCosts.Length];
    }

    public void BuyNewIsland(Button but){
        curIsland = IslandScroll.curIsland;
        coins -= islandCosts[curIsland];
        but.gameObject.transform.parent.gameObject.SetActive(false);
        islandInfoObj.SetActive(true);
        boostButtonObj.SetActive(true);
        islandInfo[0].text = islandNames[curIsland];
        islandInfo[1].text = "ЖИТЕЛЕЙ: " + islandPeopleAmount[curIsland];
        timer30m = 0;
    }



    public void SpeedUpTimer(){
        takeWithBoostObj.SetActive(true);
    }

    public void CloseBooster(){
        takeWithBoostObj.SetActive(false);
    }
  

    
    public void TakeBoost(){
        availableIslands[IslandScroll.curIsland] = true;
        takeWithBoostObj.SetActive(false);
        // Минус затраченные очки
        Debug.Log("+++");
    }

    public void TakePrize(){
        acceptPrizeObj.SetActive(true);
        islandInfoObj.SetActive(false);
    }

    public void GetPrize(){
        acceptPrizeObj.SetActive(false);
        boostButtonObj.SetActive(true);
        prizeObj.SetActive(false);
        islandInfoObj.SetActive(false);
        startMeditationObj.SetActive(true);
        coins+=100;
        Debug.Log("++");
        // И прописываю значения нашим валютам
    }

 
    public void GoToMeditation(){
        //startMeditation.SetActive(false);
        meditationMenuObj.SetActive(true);
       
    }

   

 
  

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
            downloadingObj.SetActive(false);
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



 

    public void OpenCourseMenu(){
        courseMenuObj.SetActive(true);
        meditationMenuObj.SetActive(false);
    }

    

    public void PlayTrack(Button but){
        medNum = int.Parse(but.name + "");
        if(!wereStarted){
            downloadingObj.SetActive(true);
            wereStarted = true;
            courseMenuObj.SetActive(false);
            playMenuObj.SetActive(true);
            playButtonObj = but.gameObject;

            Button butPause = Instantiate(pauseButtonObj).GetComponent<Button>();
            savedPauseButtonObj = butPause.gameObject;
            butPause.gameObject.transform.SetParent(playButtonObj.transform.parent);
            butPause.gameObject.transform.position = playButtonObj.transform.position;
            butPause.gameObject.transform.localScale = playButtonObj.transform.localScale;
            butPause.onClick.AddListener(() => PauseSound());

            StartCoroutine(DownloadTrack(audioClasses[medNum].url));
            //audioSource.PlayOneShot(islands[curIsland].audios[medNum]);

            but.gameObject.SetActive(false);
        }else{
            audioSource.UnPause();
            count = true;
            savedPauseButtonObj.SetActive(true);
            playButtonObj.SetActive(false);
        }
    }

    public void PauseSound()
    {
        if(playButtonObj.activeSelf == false){
            count = false;
            audioSource.Pause();
            savedPauseButtonObj.SetActive(false);
            playButtonObj.SetActive(true);
        }
    }

    public void CloseMeditation(){
        PauseSound();
        meditationMenuObj.SetActive(false);
    }

   
    public void EnableBuying(){
        findNewIslandObj.SetActive(true);
        findNewIslandObj.transform.GetComponentInChildren<Text>().text = islandCosts[IslandScroll.curIsland] + "";
    }

    string strMin1;
    string strSec1;
 


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

            showLeftTimeText.text = strMin + ":" + strSec;
            fullTimeText.text = strMin1 + ":" + strSec1;

            if(counting > curTrack.length){
                count = false;
                counting = 0;
                wereStarted = false;
                savedPauseButtonObj.SetActive(false);
                playButtonObj.SetActive(true);
                playMenuObj.SetActive(false);
                startMeditationObj.SetActive(false);
                IslandScroll.prevIsland = curIsland;
                curIsland++;
                IslandScroll.curIsland++;
                islandScroll.ChooseIsland();
                  islandScroll.ActiveIsland();
            }
        }
        if(timer30m >= 0){
            timer30m += Time.deltaTime;
            if(!availableIslands[IslandScroll.curIsland])
                loading.fillAmount+=Time.deltaTime/1800;
            else
                loading.fillAmount+=Time.deltaTime;

            if(loading.fillAmount == 1){
                prizeObj.SetActive(true);
                boostButtonObj.SetActive(false);
                timer30m = -1;
                loading.fillAmount = 0;
            }
        }

        if(timer30m > 1800){
            prizeObj.SetActive(true);
            boostButtonObj.SetActive(false);
            timer30m = -1;
        }
    }
}
