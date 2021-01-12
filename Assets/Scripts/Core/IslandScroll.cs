using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandScroll : MonoBehaviour
{
    public MenuClass menuClass;
    public static int curIsland;
    public GameObject[] islands;
    public GameObject[] players;
    public GameObject cam;

    public static bool tap, swipeUp, swipeDown;
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta;

    public bool moveToIsland;
    public float moveToIslandTimer;

    Vector3 startIslandPos;
    
    void Start(){
        startIslandPos = islands[0].transform.position;

        for(int i = 0; i < islands.Length; i++){
            localStartScales[i] = islands[i].transform.localScale.x;
        }

        for(int i = 0; i < islands.Length; i++){
            islands[i].transform.localScale = new Vector3(localStartScales[i]*scaleMultiplier,localStartScales[i]*scaleMultiplier,localStartScales[i]*scaleMultiplier);
        }
        islands[0].transform.localScale = new Vector3(localStartScales[0],localStartScales[0],localStartScales[0]);
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
        if(isDraging && menuClass.scrolling){
            if(Input.touches.Length < 0){
                swipeDelta = Input.touches[0].position - startTouch;
            }else if(Input.GetMouseButton(0)){
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }

        if(!moveToIsland && !started){
            if(swipeDelta.magnitude > 125){
                menuClass.toIslandButton.SetActive(false);
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
            }else{
                menuClass.toIslandButton.SetActive(true);
            }
        }
    }

    public int speed = 5;
    public float moveTimer;
    public bool started = true;
    public float[] localStartScales = new float[6];
    public float scaleMultiplier;
    public float scaleMultiplierD;

    public float startColor = 166;
    public float endColor = 255;

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
                //islands[prevIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255,255,255,colorDown);
                //islands[curIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255,255,255,colorUp);

                float islandScale =  Mathf.MoveTowards(islands[curIsland].transform.localScale.x, localStartScales[curIsland]*scaleMultiplierD, Time.deltaTime);
                float islandScaleD =  Mathf.MoveTowards(islands[prevIsland].transform.localScale.x, localStartScales[prevIsland]*scaleMultiplier, Time.deltaTime);

                islands[curIsland].transform.localScale = new Vector3(islandScale, islandScale, islandScale);
                islands[prevIsland].transform.localScale = new Vector3(islandScaleD, islandScaleD, islandScaleD);

                //islands[curIsland].transform.localScale += new Vector3(islandScale, islandScale, islandScale);
                //islands[prevIsland].transform.localScale -= new Vector3(islandScaleD, islandScaleD, islandScaleD);
            }
            if(cam.transform.position.x != islands[curIsland].transform.position.x){
                cam.transform.position = 
                    Vector2.MoveTowards(cam.transform.position, islands[curIsland].transform.position,
                    Time.deltaTime * speed);
            }else{
                moveToIsland = false;
                menuClass.ChangeIslandInfo();
                menuClass.toIslandButton.SetActive(true);
                moveTimer = 0;
            }
        }
    }

    public static int prevIsland;

    void SwipeUp(){
        if(curIsland < islands.Length-1  && MenuClass.curIsland > curIsland){
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
        if(islands[curIsland].activeSelf == false){
            islands[curIsland].SetActive(true);
        }
        islands[prevIsland].transform.GetChild(1).gameObject.SetActive(false);
        islands[prevIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255f/255f,255f/255f,255f/255f,166f/255f);
        islands[curIsland].GetComponentInChildren<SpriteRenderer>().color = new Color(255f/255f,255f/255f,255f/255f,255f/255f);
    }

    void Reset(){
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}
