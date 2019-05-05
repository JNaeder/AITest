using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGenerator : MonoBehaviour
{
    public Collider mainFloor;
    public int foodAmount;
    public GameObject foodPrefab;

    float xRange, zRange;
    // Start is called before the first frame update
    void Start()
    {
        xRange = mainFloor.bounds.size.x / 2;
        zRange = mainFloor.bounds.size.z / 2;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateFood() {
        for (int i = 0; i < foodAmount; i++)
        {
            Vector3 newPos = new Vector3(Random.Range(-xRange, xRange),0, Random.Range(-zRange, zRange));
            GameObject newFood = Instantiate(foodPrefab, newPos, Quaternion.identity);
            newFood.name = "Food_" + Random.Range(0, 9999);

        }


    }
}
