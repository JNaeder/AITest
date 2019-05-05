using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerScript : MonoBehaviour
{
    Camera cam;
    
    public GameObject guy, food, food_Guy;

    public Text leftText, timeText;

    public float globalTime;
    public float timeSclaeNum = 1f;

    enum State {isPlacingGuys, isPlacingFoodGuys,isNothing}
    State switchState;


    public static int numberOfGuys;


    int min = 0;
    int hour = 0;

    List<Guy_Movement> guyList;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        guyList = new List<Guy_Movement>();
        switchState = State.isNothing;
    }

    // Update is called once per frame
    void Update()
    {
        globalTime = Time.time;
        Time.timeScale = timeSclaeNum;

        leftText.text = numberOfGuys.ToString();
        timeText.text = GetTime(globalTime);


        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            switch (switchState) {
                case State.isPlacingGuys:
                    PlaceGuys();
                    break;
                case State.isPlacingFoodGuys:
                    PlaceFoodguys();
                    break;


            }

        }
    }


    void PlaceGuys() {

       GameObject newGuy = Instantiate(guy, GetWorldPosiition(), Quaternion.identity);
        newGuy.name = "Guy_" + globalTime.ToString("F2");
        Guy_Movement guyScript = newGuy.GetComponent<Guy_Movement>();
       guyList.Add(guyScript);
        numberOfGuys++;


    }

    void PlaceFoodguys() {
        GameObject newGuy = Instantiate(food_Guy, GetWorldPosiition(), Quaternion.identity);
        newGuy.name = "Food Guy_" + GetTime(globalTime);

    }

    void PlaceFood()
    {
        Vector3 newFoodPos = GetWorldPosiition();
        newFoodPos.y = 0;
        GameObject newFood = Instantiate(food, newFoodPos, Quaternion.identity);
        newFood.name = "Food_" + Random.Range(0, 99999);
        FoodScript foodScript = newFood.GetComponent<FoodScript>();


    }


    public string GetTime(float floatTime) {
        float sec = floatTime - ((min * 60) + (hour * 3600));
        if (sec >= 60f) {
            min++;
            if (min >= 60) {
                min = 0;
                hour++;

            }

        }




        return (" Hour: " + hour + " Min: " + min + " Sec: " + Mathf.Floor(sec));
    }


    


    public Vector3 GetWorldPosiition() {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mousePos, out RaycastHit hit))
        {
            return hit.point;

        }

        else return Vector3.zero;
    }



    





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
