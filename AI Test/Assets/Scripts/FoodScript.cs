using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{

    public float foodAmount;

    float minFoodAmount = 5;
    float maxFoodAmount = 20f;

    public Transform foodModel;

    SphereCollider coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<SphereCollider>();

        foodAmount = Random.Range(minFoodAmount, maxFoodAmount);
        float remappedSize = Remap(foodAmount, 5f, 20f, 1f, 3f);
        Vector3 foodScale = new Vector3(remappedSize, remappedSize, remappedSize);
        foodModel.localScale = foodScale;

    }

    // Update is called once per frame
    void Update()
    {
        float remappedSize = Remap(foodAmount, 5f, 20f, 1f, 3f);
        Vector3 foodScale = new Vector3(remappedSize, remappedSize, remappedSize);
        foodModel.localScale = foodScale;

        if (foodAmount <= 0) {
            Destroy(gameObject);
        }
    }

    float Remap(float thisNum, float fromA, float toA, float fromB, float toB) {

        return (thisNum - fromA) / (toA - fromA) * (toB - fromB) + fromB;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        coll = GetComponent<SphereCollider>();
        Gizmos.DrawWireSphere(transform.position, coll.radius);
    }

}
