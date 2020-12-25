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

        if(!moveToIsland){
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
            }else if(swipeDelta.magnitude > 15){

            }
        }
    }

    void FixedUpdate(){
        if(moveToIsland){
            /*
            if(Vector3.Distance(cam.transform.position, islands[curIsland].transform.position) > 0.3f){
                moveToIslandTimer+=Time.deltaTime;
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, islands[curIsland].transform.position, Time.deltaTime * 10);
                cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, islands[curIsland].transform.rotation, Time.deltaTime * 10);
            }else{
                moveToIsland = false;
                moveToIslandTimer = 0;
                menuClass.EnableBuying();
            }
            */


                if(Vector3.Distance(center.position, islands[curIsland].transform.position) > 0.003f ||
                Vector3.Distance(botLeft.position, islands[prevIsland].transform.position) > 0.003f){
                    if(up){
                        islands[prevIsland].transform.position = Vector3.MoveTowards(islands[prevIsland].transform.position, botLeft.position, Time.deltaTime * 15);
                        islands[curIsland].transform.position = Vector3.MoveTowards(islands[curIsland].transform.position, center.position, Time.deltaTime * 15);
                    }
                }else if(Vector3.Distance(center.position, islands[curIsland].transform.position) > 0.003f ||
                Vector3.Distance(topRight.position, islands[prevIsland].transform.position) > 0.003f){
                    if(down){
                        islands[prevIsland].transform.position = Vector3.MoveTowards(islands[prevIsland].transform.position, topRight.position, Time.deltaTime * 15);
                        islands[curIsland].transform.position = Vector3.MoveTowards(islands[curIsland].transform.position, center.position, Time.deltaTime * 15);
                    }
                }else{
                    //123
                    moveToIsland = false;
                    moveToIslandTimer = 0;
                    down=false;
                    up=false;
                }

        }else if(transformBeforeMoving){
            moveToIslandTimer+=Time.deltaTime;
            if(moveToIslandTimer < 1){
                if(down){
                    islands[prevIsland].transform.position = Vector3.MoveTowards(islands[prevIsland].transform.position, topRight.position, Time.deltaTime * swipeDelta.magnitude);
                    islands[curIsland].transform.position = Vector3.MoveTowards(islands[curIsland].transform.position, center.position, Time.deltaTime * swipeDelta.magnitude);
                }else if(up){
                    islands[prevIsland].transform.position = Vector3.MoveTowards(islands[prevIsland].transform.position, botLeft.position, Time.deltaTime * swipeDelta.magnitude);
                    islands[curIsland].transform.position = Vector3.MoveTowards(islands[curIsland].transform.position, center.position, Time.deltaTime * swipeDelta.magnitude);
                }
            }else{
                moveToIslandTimer = 0;
                transformBeforeMoving = false;
            }
        }
    }

    public bool transformBeforeMoving;
    public int prevIsland;
    public bool down;
    public bool up;

    public Transform topRight;
    public Transform botLeft;
    public Transform center;

    void SwipeUp(){
        if(curIsland < islands.Length-1){
            prevIsland = curIsland;
            curIsland++;
            up = true;
            ChooseIsland();
        }
    }

    void SwipeDown(){
        if(curIsland > 0){
            prevIsland = curIsland;
            curIsland--;
            down = true;
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
