using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour
{

    public enum State { isPlacingGuys, isPlacingFoodGuys, isNothing }
    public State switchState;
    [Header("Prefabs")]
    public GameObject guy;
    public GameObject food;
    public GameObject food_Guy;
    [Header("UI Stuff")]
    public Text leftText;
    public Text timeText;
    [Header("Time Stuff")]
    public float globalTime;
    public float timeSclaeNum = 1f;



    public static int numberOfGuys;

    Camera cam;

    float min = 0;
    float hour = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        switchState = State.isNothing;
    }

    // Update is called once per frame
    void Update()
    {
        TimeStuff();
        UpdateUI();
        InputControl();
    }

    // Mouse Clicks
    void InputControl()
    {
        // left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // check to see if it's using UI element or not
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // Check to see if it hit a collider
            Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mousePos, out RaycastHit hit))
            {
                return;
            }

            // Place Something depending on the state
            switch (switchState)
            {
                case State.isPlacingGuys:
                    PlaceGuys();
                    break;
                case State.isPlacingFoodGuys:
                    PlaceFoodguys();
                    break;
            }
        }
    }

    // place normal guy and name him
    void PlaceGuys() {
        GameObject newGuy = Instantiate(guy, GetWorldPosiition(), Quaternion.identity);
        newGuy.name = "Guy_" + GetTime(globalTime);
        numberOfGuys++;
    }


    void PlaceFoodguys() {
        GameObject newGuy = Instantiate(food_Guy, GetWorldPosiition(), Quaternion.identity);
        newGuy.name = "Food Guy_" + GetTime(globalTime);
    }


    void UpdateUI() {
        leftText.text = numberOfGuys.ToString();
        timeText.text = GetTime(globalTime);
    }

    void TimeStuff() {
        globalTime = Time.time;
        Time.timeScale = timeSclaeNum;
    }


    // make time into a string
    public string GetTime(float floatTime) {
        min = Mathf.Floor((floatTime - (hour * 3600)) / 60);
        hour = Mathf.Floor(floatTime / 3600);
        float sec = floatTime - ((min * 60) + (hour * 3600));
        return (" Hour: " + hour + " Min: " + min + " Sec: " + Mathf.Floor(sec));
    }


    

    // Get the mouseclick position in world units
    public Vector3 GetWorldPosiition() {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mousePos, out RaycastHit hit))
        {
            return hit.point;
        }
        else return Vector3.zero;
    }



    



    // button functions

    public void IsPlacingGuys() {
        switchState = State.isPlacingGuys;
    }
    public void IsPlacingFoodGuys() {
        switchState = State.isPlacingFoodGuys;
    }
    public void SetTimeScale(float newTimescale) {
       timeSclaeNum = newTimescale;
    }

    
}
