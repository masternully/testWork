using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float moveToIslandTimer;
    
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

        if(swipeDelta.magnitude > 125 && !moveToIsland){
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

        if(moveToIsland){
            if(Vector3.Distance(cam.transform.position, islands[curIsland].transform.position) > 0.3f){
                moveToIslandTimer+=Time.deltaTime;
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, islands[curIsland].transform.position, Time.deltaTime * 10);
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, islands[curIsland].transform.rotation, Time.deltaTime * 10);
            }else{
                moveToIsland = false;
                moveToIslandTimer = 0;
                menuClass.EnableBuying();
            }
        }
    }

    void SwipeUp(){
        if(curIsland < islands.Length-1){
            curIsland++;
            ChooseIsland();
        }
    }

    void SwipeDown(){
        if(curIsland > 0){
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
