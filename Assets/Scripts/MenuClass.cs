using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuClass : MonoBehaviour
{
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

    public void PlayTrack(Button but){
        medNum = int.Parse(but.name + "");
        count = true;
        if(!wereStarted){
            wereStarted = true;
            playButton = but.gameObject;

            Button butPause = Instantiate(pauseButton).GetComponent<Button>();
            savedPauseButton = butPause.gameObject;
            butPause.gameObject.transform.SetParent(playButton.transform.parent);
            butPause.gameObject.transform.position = playButton.transform.position;
            butPause.gameObject.transform.localScale = playButton.transform.localScale;
            butPause.onClick.AddListener(() => PauseSound());

            audioSource.PlayOneShot(islands[curIsland].audios[medNum]);

            but.gameObject.SetActive(false);
        }else{
            audioSource.UnPause();
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

    void Update(){
        if(count){
            counting += Time.deltaTime;
            showLeftTime.text = (int)counting + " / 15 sec.";

            if(counting > islands[curIsland].audios[medNum].length){
                count = false;
                counting = 0;
                wereStarted = false;
                savedPauseButton.SetActive(false);
                playButton.SetActive(true);
                meditationMenu.SetActive(false);
                startMeditation.SetActive(false);
                curIsland++;
                IslandScroll.curIsland++;
                IslandScroll.moveToIsland = true;
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
