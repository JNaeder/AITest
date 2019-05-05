using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.LWRP;

public class Guy_Movement : MonoBehaviour
{

    NavMeshAgent guyAgent;
    ControllerScript cS;

    public Vector3 targetDest;

    public enum State {searching, feeding}
    public State switchState;

    NavMeshPath path;
    public FoodScript currentFood;
    public GameObject spawnPrefab;
    public LayerMask foodLayer;

    public float buffer = 1f;
    public float mutationRange = 0.25f;

    [Header("Main Atrributes")]
    public float searchRange = 10f;
    public float speed = 5f;
    public float foodSearchRadius = 2f;

    public float health = 100f;
    public float maxExtraFood = 20f;
    public float extraFood = 0;
    public GameObject healthBarGreen, extraBarBlue;

    float startHealth;
    float foodRadius;
    

    // Start is called before the first frame update
    void Start()
    {


        cS = FindObjectOfType<ControllerScript>();
        guyAgent = GetComponent<NavMeshAgent>();
        
        path =  new NavMeshPath();

        startHealth = health;

        
        targetDest = ChooseRandomSeekPos();
        guyAgent.speed = speed;
        SetDest(targetDest);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(switchState);

        
        UpdateHealthBar();


        switch (switchState) {
            case State.searching:
                Seeking();
                break;
            case State.feeding:
                Feeding();
                break;


        }
        

    }


   

    


    void Seeking() {

        health -= Time.deltaTime * 0.25f;


        guyAgent.speed = speed;
        guyAgent.isStopped = false;

        CheckForFood();

        if (health <= 0) {
            //Debug.Log(name + " has died at " + cS.globalTime);
            ControllerScript.numberOfGuys--;
            Destroy(gameObject);
            return;
        }

        guyAgent.CalculatePath(targetDest, path);

        // if path is not valid, choose another one
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            targetDest = ChooseRandomSeekPos();
            SetDest(targetDest);
            return;

        }
        else if (path.status == NavMeshPathStatus.PathComplete) {
            //path is valid but no food found
            if (Vector3.Distance(transform.position, targetDest) < buffer && currentFood == null)
            {
                
                targetDest = ChooseRandomSeekPos();
                SetDest(targetDest);
            }
        } 



    }

    void Feeding() {

        if (extraFood >= maxExtraFood ) {
            //Debug.Log(name + " Fisnihed Eating");
            CreateSpawn();
            extraFood = 0;
            currentFood = null;
            switchState = State.searching;
            targetDest = ChooseRandomSeekPos();
            SetDest(targetDest);
            return;

        }
        if (currentFood == null)
        {
            //Debug.Log(name + " Ran Out of Food");

            currentFood = null;
            switchState = State.searching;
            targetDest = ChooseRandomSeekPos();
            SetDest(targetDest);
            return;

        }
        else {
            targetDest = currentFood.transform.position;
            SetDest(targetDest);
            //Debug.Log(Vector3.Distance(transform.position, targetDest) + name);
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
            else {
                guyAgent.isStopped = false;
            }

        }


    }



    void CheckForFood() {

        FoodScript closestFood = null;
        float closestFoodDist = Mathf.Infinity;

        Collider[] colls = Physics.OverlapSphere(transform.position, foodSearchRadius, foodLayer);
        if (colls.Length > 0) {
            foreach (Collider c in colls) {
                float dist = Vector3.Distance(transform.position, c.gameObject.transform.position);
                if (dist < closestFoodDist) {
                    closestFood = c.GetComponent<FoodScript>();
                }
            }

            currentFood = closestFood;
            switchState = State.feeding;
            switchState = State.feeding;
        }
    }

    void UpdateHealthBar() {
        Vector3 healthScale = healthBarGreen.transform.localScale;
        float healthPerc = health / startHealth;
        healthScale.x = healthPerc;
        healthBarGreen.transform.localScale = healthScale;

        Vector3 extraScale = extraBarBlue.transform.localScale;
        float extraPerc = extraFood / maxExtraFood;
        extraScale.x = extraPerc;
        extraBarBlue.transform.localScale = extraScale;

    }

    Vector3 ChooseRandomSeekPos() {
        Vector3 newPos = new Vector3(Random.Range(-searchRange,searchRange), 0, Random.Range(-searchRange, searchRange));
        newPos += transform.position;
        return newPos;
    }

    
     void SetDest(Vector3 pos) {
        guyAgent.SetDestination(pos);

    }
    

    void CreateSpawn() {
        GameObject newGuy = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        newGuy.name = "Guy_" + cS.GetTime(cS.globalTime);
        Guy_Movement guyScript = newGuy.GetComponent<Guy_Movement>();
        guyScript.speed = speed + Random.Range(-mutationRange, mutationRange);
        guyScript.searchRange = searchRange + Random.Range(-mutationRange, mutationRange);
        guyScript.foodSearchRadius = foodSearchRadius + Random.Range(-mutationRange, mutationRange);




        guyScript.extraFood = 0;
       // Debug.Log(name + " spawned " + newGuy.name);
        ControllerScript.numberOfGuys++;

    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Bloob") {
            //Debug.Log("Has started eating " + other.gameObject.name);

            switchState = State.feeding;
            currentFood = other.GetComponent<FoodScript>();
            switchState = State.feeding;
            foodRadius = currentFood.foodModel.GetComponent<Collider>().bounds.size.x;
        } 
    }





    private void OnDrawGizmos()
    {

        //Gizmos.color = Color.red;
       // Gizmos.DrawWireSphere(transform.position, searchRange);
        //Gizmos.color = Color.blue;
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
