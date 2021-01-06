using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandScroll : MonoBehaviour
{
    public MenuClass menuClass;

    public GameObject[] islandsObj;
    public GameObject[] playersObj;
    public GameObject cam;
    public GameObject IslandControlObj,CitizenObj;
    public GameObject LevelInfoObj,FindNewIslandObj;
    public static int curIsland;
    public static bool tap, swipeUp, swipeDown;
    public static bool moveToIsland;
    private bool isDraging = false;
    public bool started = true;
    public int speed = 5;
    public float moveTimer;
   
    public float[] localStartScales = new float[6];
    public float scaleMultiplier;
    public float scaleMultiplierD;
public int ActiveIslandNumber=1;
    public float startColor = 166;
    public float endColor = 255;
    
    public float moveToIslandTimer;
    private Vector2 startTouch, swipeDelta;
    Vector3 startIslandPos;
    
    void Start(){
        startIslandPos = islandsObj[0].transform.position;

        for(int i = 0; i < ActiveIslandNumber; i++){
            localStartScales[i] = islandsObj[i].transform.localScale.x;
        }

        for(int i = 0; i < ActiveIslandNumber; i++){
            islandsObj[i].transform.localScale = new Vector3(localStartScales[i]*scaleMultiplier,localStartScales[i]*scaleMultiplier,localStartScales[i]*scaleMultiplier);
        }
        islandsObj[0].transform.localScale = new Vector3(localStartScales[0],localStartScales[0],localStartScales[0]);
    }

    void Update()
    {
        tap = swipeUp = swipeDown = false;
        if(Input.GetMouseButtonDown(0)){
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;
        }else if(Input.GetMouseButtonUp(0)){
            isDraging = false;
            Reset();
        }

        if(Input.touches.Length > 0){
            if(Input.touches[0].phase == TouchPhase.Began){
                tap = true;
                isDraging = true;
                startTouch = Input.touches[0].position;
            }else if(Input.touches[0].phase == TouchPhase.Ended){
                isDraging = false;
                Reset(); 
            }
        }

        swipeDelta = Vector2.zero;
        if(isDraging){
            if(Input.touches.Length < 0){
                swipeDelta = Input.touches[0].position - startTouch;
            }else if(Input.GetMouseButton(0)){
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        if(!moveToIsland && !started){
            if(swipeDelta.magnitude > 125){
                float x = swipeDelta.x;
                float y = swipeDelta.y;
                if(Mathf.Abs(x) < Mathf.Abs(y)){
                    if(y < 0){
                        swipeDown = true;
                        SwipeUp();
                    }else{
                        swipeUp = true;
                        SwipeDown();
                    }
                }
                Reset();
            }
        }
    }

 

    void FixedUpdate(){
        if(started){
            moveTimer += Time.deltaTime;
            if(moveTimer > 1.0f){
                started = false;
                moveTimer = 0;
            }
        }
        if(moveToIsland){
            moveTimer += Time.deltaTime;
            if(moveTimer < 1.0f){
                float colorUp =  Mathf.MoveTowards(startColor, endColor, Time.deltaTime);
                float colorDown =  Mathf.MoveTowards(endColor, startColor, Time.deltaTime);
                //islandsObj[prevIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255,255,255,colorDown);
                //islandsObj[curIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255,255,255,colorUp);

                float islandsObjcale =  Mathf.MoveTowards(islandsObj[curIsland].transform.localScale.x, localStartScales[curIsland]*scaleMultiplierD, Time.deltaTime);
                float islandsObjcaleD =  Mathf.MoveTowards(islandsObj[prevIsland].transform.localScale.x, localStartScales[prevIsland]*scaleMultiplier, Time.deltaTime);

                islandsObj[curIsland].transform.localScale = new Vector3(0.6097f, 0.6097f, 0.6097f);
                islandsObj[prevIsland].transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

                //islandsObj[curIsland].transform.localScale += new Vector3(islandsObjcale, islandsObjcale, islandsObjcale);
                //islandsObj[prevIsland].transform.localScale -= new Vector3(islandsObjcaleD, islandsObjcaleD, islandsObjcaleD);
            }
            if(cam.transform.position.x != islandsObj[curIsland].transform.position.x){
                cam.transform.position = 
                    Vector2.MoveTowards(cam.transform.position, islandsObj[curIsland].transform.position,
                    Time.deltaTime * speed);
            }else{
                moveToIsland = false;
                menuClass.EnableBuying();
                moveTimer = 0;
            }
        }
    }

    public static int prevIsland;

    void SwipeUp(){
        Debug.Log(islandsObj.Length);
        if(curIsland < ActiveIslandNumber-1){
            prevIsland = curIsland;
            curIsland++;
            ChooseIsland();
        }
    }

    void SwipeDown(){
        if(curIsland > 0){
            prevIsland = curIsland;
            curIsland--;
            ChooseIsland();
        }
    }

    public void ChooseIsland(){
        moveToIsland = true;
        islandsObj[prevIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255f/255f,255f/255f,255f/255f,166f/255f);
        
        islandsObj[curIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255f/255f,255f/255f,255f/255f,255f/255f);
       
    }
public void ActiveIsland()
{
    islandsObj[curIsland].SetActive(true);
    ActiveIslandNumber++;
    LevelInfoObj.SetActive(false);
    FindNewIslandObj.SetActive(false);
    IslandControlObj.SetActive(true);
    CitizenObj.SetActive(false);
}
    void Reset(){
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}
