using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.LWRP;

public class Guy_Movement : MonoBehaviour
{

    NavMeshAgent guyAgent;
    ControllerScript cS;
    NavMeshPath path;


    public enum State {searching, feeding, cannibal, horny, beingEaten}
    [Header("Status")]
    public State switchState;
    public FoodScript currentFood;
    public Guy_Movement currentGuyFood;
    public Guy_Movement currentPartner;
   

    [Header("Set Up")]
    public GameObject spawnPrefab;
    public LayerMask foodLayer;
    public LayerMask cannibalLayer;
    public LayerMask hornyLayer;
    public GameObject healthBarGreen, extraBarBlue;


    [Header("Main Atrributes")]
    public float strength = 5f;
    public float searchRange = 10f;
    public float speed = 5f;
    public float foodSearchRadius = 2f;
    public float health = 100f;
    public float maxExtraFood = 20f;
    public float extraFood = 0;
    public float buffer = 1f;
    public float mutationRange = 0.25f;
    public string age;
    public int childrenHad;


    float startHealth;
    float foodRadius;
    float birthTime;
    Vector3 targetDest;

    // Start is called before the first frame update
    void Start()
    {

        //Assign
        cS = FindObjectOfType<ControllerScript>();
        guyAgent = GetComponent<NavMeshAgent>();

        // Set Up
        path =  new NavMeshPath();
        startHealth = health;
        guyAgent.speed = speed;
        birthTime = cS.globalTime;


        GoToRandomPostion();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateBars();
        age = cS.GetTime(cS.globalTime - birthTime);



        switch (switchState) {
            case State.searching:
                Seeking();
                break;
            case State.feeding:
                Feeding();
                break;
            case State.cannibal:
                Cannibal();
                break;
            case State.horny:
                Horny();
                break;
            case State.beingEaten:
                BeingEaten();
                break;
        }
    }


   

    


    void Seeking() {

        health -= Time.deltaTime * 0.25f;
        guyAgent.speed = speed;
        guyAgent.isStopped = false;

        // Go to random place and check for only food
        CheckForFood();
        MoveToRandomValidPosition();

        // If Dead
        if (health <= 0) {
            ControllerScript.numberOfGuys--;
            Destroy(gameObject);
            return;
        }

        // If is hungry, will become cannibal
        if(health <= startHealth * 0.25f) {
            switchState = State.cannibal;
            return;
           }

        // If full extra food and more than half health, look for mate
        if(extraFood >= maxExtraFood && health > startHealth * 0.5f) {
            switchState = State.horny;
            return;
          }


       



    }

    void Feeding() {
        // Has Enough Food, stop feeding
        if (extraFood >= maxExtraFood ) {
            switchState = State.horny;
            GoToRandomPostion();
            return;

        }

        // Ran out of food, stop feeding
        if (currentFood == null && currentGuyFood == null)
        {

            currentFood = null;
            currentGuyFood = null;
            switchState = State.searching;
            MoveToRandomValidPosition();
            return;

        }
        if (currentFood != null && currentGuyFood == null)
        {
            EatFood();

        }
        else if (currentGuyFood != null && currentFood == null)
        {
            EatGuy();
        }



    }

    void Cannibal() {
        health -= Time.deltaTime * 0.1f;
        guyAgent.speed = speed + 2;
        guyAgent.isStopped = false;

        // Go To random place and check for food or another guy
        CheckForAnything();
        MoveToRandomValidPosition();


        // If Dead
        if (health <= 0)
        {
            ControllerScript.numberOfGuys--;
            Destroy(gameObject);
            return;
        }

        if (health > startHealth * 0.75f) {
            switchState = State.searching;
            return;
          }




    }

    void Horny() {
        //Debug.Log(name + " is horny");

        health -= Time.deltaTime * 0.25f;
        guyAgent.speed = speed;
        guyAgent.isStopped = false;
        currentFood = null;

        CheckForPartner();
        MoveToRandomValidPosition();

        if (health <= startHealth * 0.5f)
        {
            switchState = State.searching;
            return;
        }
        // Makew new person
        if (currentPartner) {
            CreateSpawn();
            extraFood = 0;
            currentPartner.extraFood = 0;
            currentPartner.currentPartner = null;
            switchState = State.searching;
            currentPartner.switchState = State.searching;
            currentPartner = null;
            GoToRandomPostion();
        }
    }


    void BeingEaten() {
        // If Dead
        if (health <= 0)
        {
            ControllerScript.numberOfGuys--;
            Destroy(gameObject);
            return;
        }

    }


    void CheckForFood() {

        FoodScript closestFood = null;
        float closestFoodDist = Mathf.Infinity;

        // Check for all the food colliders it's touching, find the closest one
        Collider[] colls = Physics.OverlapSphere(transform.position, foodSearchRadius, foodLayer);
        if (colls.Length > 0) {
            foreach (Collider c in colls) {
                float dist = Vector3.Distance(transform.position, c.gameObject.transform.position);
                if (dist < closestFoodDist) {
                    closestFood = c.GetComponent<FoodScript>();
                    closestFoodDist = dist;
                }
            }

            currentFood = closestFood;
            switchState = State.feeding;
        }
    }

    void CheckForAnything() {
        GameObject closestThing = null;
        float closestThingDist = Mathf.Infinity;

        Collider[] collsFood = Physics.OverlapSphere(transform.position, foodSearchRadius,foodLayer);
        if(collsFood.Length > 0) {
            foreach(Collider c in collsFood) {
                float dist = Vector3.Distance(transform.position, c.gameObject.transform.position);
                if (dist < closestThingDist)
                {
                    closestThing = c.gameObject;
                    closestThingDist = dist;
                }
            }
            }

        if(closestThing != null) {
            currentFood = closestThing.GetComponent<FoodScript>();
            return;
     }      


        // Check for all the food colliders it's touching, find the closest one
        Collider[] colls = Physics.OverlapSphere(transform.position, foodSearchRadius, cannibalLayer);
        if (colls.Length > 0)
        {
            foreach (Collider c in colls)
            {
                if (c.gameObject != gameObject)
                {
                    //Debug.Log("Some Other Collider! " + this.gameObject);
                    float dist = Vector3.Distance(transform.position, c.gameObject.transform.position);
                    if (dist < closestThingDist)
                    {
                        closestThing = c.gameObject;
                        closestThingDist = dist;
                    }
                }
            }

            if(closestThing == null) {
                return;
              }

            if (closestThing.GetComponent<FoodScript>() != null) {
                currentFood = closestThing.GetComponent<FoodScript>();
                currentGuyFood = null;

               } else if(closestThing.GetComponent<Guy_Movement>() != null) {
                currentGuyFood = closestThing.GetComponent<Guy_Movement>();
                currentFood = null;
            }
            switchState = State.feeding;
        }

    }


    void CheckForPartner() {
        Guy_Movement closestGuy = null;
        float closestGuyDist = Mathf.Infinity;

        Collider[] colls = Physics.OverlapSphere(transform.position, foodSearchRadius, hornyLayer);
        if(colls.Length > 0) {
            foreach (Collider c in colls)
            {
                if (c.gameObject != gameObject)
                {
                    //Debug.Log("Some Other Collider! " + this.gameObject);
                    float dist = Vector3.Distance(transform.position, c.gameObject.transform.position);
                    if (dist < closestGuyDist)
                    {
                        closestGuy = c.GetComponent<Guy_Movement>();
                        closestGuyDist = dist;
                    }
                }
            }
        }

        if(closestGuy != null) {
            currentPartner = closestGuy;
 }

    }


    void UpdateBars() {
        //Update the Health UI Bar
        Vector3 healthScale = healthBarGreen.transform.localScale;
        float healthPerc = health / startHealth;
        healthScale.x = healthPerc;
        healthBarGreen.transform.localScale = healthScale;

        //Update Extra Food UI Bar
        Vector3 extraScale = extraBarBlue.transform.localScale;
        float extraPerc = extraFood / maxExtraFood;
        extraScale.x = extraPerc;
        extraBarBlue.transform.localScale = extraScale;

    }


    //Seeking Stuff!
    Vector3 ChooseRandomSeekPos() {
        Vector3 newPos = new Vector3(Random.Range(-searchRange,searchRange), 0, Random.Range(-searchRange, searchRange));
        newPos += transform.position;
        return newPos;
    }
    void GoToRandomPostion() {
        targetDest = ChooseRandomSeekPos();
        SetDest(targetDest);
    }
    void SetDest(Vector3 pos) {
        guyAgent.SetDestination(pos);
    }
    void MoveToRandomValidPosition() {

        //If alive then check to see if path is valid by calcutlating path
        guyAgent.CalculatePath(targetDest, path);


        // if path is not valid, choose another one
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            GoToRandomPostion();
            return;

        }
        else if (path.status == NavMeshPathStatus.PathComplete)
        {
            //path is valid but no food found
            if (Vector3.Distance(transform.position, targetDest) < buffer && currentFood == null)
            {

                GoToRandomPostion();
            }
        }


    }


    void EatFood() {
        //Set food as target;
        targetDest = currentFood.transform.position;
        SetDest(targetDest);


        if (Vector3.Distance(transform.position, targetDest) < 2f)
        {

            currentFood.foodAmount -= Time.deltaTime;

            if (health <= startHealth)
            {
                health += Time.deltaTime;
            }
            else if (health >= startHealth)
            {
                extraFood += Time.deltaTime;
            }
            guyAgent.isStopped = true;
        }
        else
        {

            guyAgent.isStopped = false;
        }

    }


    void EatGuy() {
        //Set food as target;
        targetDest = currentGuyFood.transform.position;
        SetDest(targetDest);


        if(health <= 0) {
            currentGuyFood.switchState = State.searching;
 }


        if (Vector3.Distance(transform.position, targetDest) < 2f)
        {

            currentGuyFood.health -= Time.deltaTime;
            currentGuyFood.switchState = State.beingEaten;

            if (health <= startHealth)
            {
                health += Time.deltaTime * 0.5f;
            } else {
                switchState = State.searching;
                currentGuyFood.switchState = State.searching;
 }
            guyAgent.isStopped = true;
        }
        else
        {
            guyAgent.isStopped = false;
        }


    }

    void CreateSpawn() {
        GameObject newGuy = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        newGuy.name = "Guy_" + cS.GetTime(cS.globalTime);
        Guy_Movement guyScript = newGuy.GetComponent<Guy_Movement>();

        // Set new values based on mutations
        guyScript.speed = speed + Random.Range(-mutationRange, mutationRange);
        guyScript.searchRange = searchRange + Random.Range(-mutationRange, mutationRange);
        guyScript.foodSearchRadius = foodSearchRadius + Random.Range(-mutationRange, mutationRange);
        guyScript.health = health + Random.Range(-mutationRange, mutationRange);

        guyScript.extraFood = 0;
        guyScript.switchState = State.searching;
        ControllerScript.numberOfGuys++;

    }




    // My Gizmos!
    private void OnDrawGizmos()
    { 
        if (currentFood != null)
        {
            Gizmos.color = Color.green;
        }
        else {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawSphere(targetDest, 0.25f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, foodSearchRadius);
    }
}
