using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Food_Guy_Movement : MonoBehaviour
{

    NavMeshAgent guyAgent;
    NavMeshPath path;
    ControllerScript cS;

    public Vector3 targetDest;
    public GameObject foodPrefab;
    public float searchRange = 10f;
    public float buffer = 1f;
    public float speed = 5f;
    public float dropRate = 10f;

    // Start is called before the first frame update
    void Start()
    {
        cS = FindObjectOfType<ControllerScript>();
        guyAgent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();


        targetDest = ChooseRandomSeekPos();
        guyAgent.speed = speed;
        SetDest(targetDest);

        InvokeRepeating("PlaceAFood", 3f, dropRate);
    }

    // Update is called once per frame
    void Update()
    {
        Roaming();
    }


    Vector3 ChooseRandomSeekPos()
    {
        Vector3 newPos = new Vector3(Random.Range(-searchRange, searchRange), 0, Random.Range(-searchRange, searchRange));
        newPos += transform.position;
        return newPos;
    }


    void SetDest(Vector3 pos)
    {
        guyAgent.SetDestination(pos);

    }


    void Roaming() {
        guyAgent.speed = speed;
        guyAgent.isStopped = false;

        guyAgent.CalculatePath(targetDest, path);

        // if path is not valid, choose another one
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            targetDest = ChooseRandomSeekPos();
            SetDest(targetDest);
            return;

        }
        else if (path.status == NavMeshPathStatus.PathComplete)
        {
            //path is valid but no food found
            if (Vector3.Distance(transform.position, targetDest) < buffer)
            {

                targetDest = ChooseRandomSeekPos();
                SetDest(targetDest);
            }
        }
    }

    void PlaceAFood() {

        GameObject newFood = Instantiate(foodPrefab, transform.position, Quaternion.identity);
        newFood.name = "Food";

    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetDest, 0.25f);
    }
}
