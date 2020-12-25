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

    public static bool moveToIsland;
    public float moveToIslandTimer;

    Vector3 startIslandPos;
    
    void Start(){
        startIslandPos = islands[0].transform.position;
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

    public int speed = 5;
    public float moveTimer;
    public bool started = true;

    void FixedUpdate(){
        if(started){
            moveTimer += Time.deltaTime;
            for(int i = 1; i < islands.Length; i++){
                islands[i].transform.localScale -= new Vector3(0.008f, 0.008f, 0.008f);
            }
            if(moveTimer > 1.0f){
                started = false;
                moveTimer = 0;
            }
        }
        if(moveToIsland){
            moveTimer += Time.deltaTime;
            if(moveTimer < 1.0f){
                islands[prevIsland].GetComponentInChildren<SpriteRenderer>().color -= new Color(0,0,0,0.007f);
                islands[curIsland].GetComponentInChildren<SpriteRenderer>().color += new Color(0,0,0,0.007f);
                islands[curIsland].transform.localScale += new Vector3(0.008f, 0.008f, 0.008f);
                islands[prevIsland].transform.localScale -= new Vector3(0.008f, 0.008f, 0.008f);
            }
            if(cam.transform.position.x != islands[curIsland].transform.position.x){
                cam.transform.position = 
                    Vector2.MoveTowards(cam.transform.position, islands[curIsland].transform.position,
                    Time.deltaTime * speed);
            }else{
                moveToIsland = false;
                menuClass.EnableBuying();
                moveTimer = 0;
            }
        }
    }

    public int prevIsland;

    void SwipeUp(){
        if(curIsland < islands.Length-1){
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

    void ChooseIsland(){
        moveToIsland = true;
    }

    void Reset(){
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}
