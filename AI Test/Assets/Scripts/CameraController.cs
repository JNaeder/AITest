using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Speeds")]
    public float movementSpeed = 5f;
    public float scrollSpeed = 5f;
    public float rotationSpeed = 5f;
    [Header("Set Up")]
    public Transform cam;

    Vector3 centerPoint;

   

    // Update is called once per frame
    void Update()
    {
        WASDMovement();
        ScrollZoom();
        CameraRotate();
    }


    void WASDMovement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, 0, v) * Time.deltaTime * movementSpeed, Space.Self);
       }


    void ScrollZoom() {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(new Vector3(0,-scrollWheel, 0) * Time.deltaTime * scrollSpeed, Space.World);
        }

    void CameraRotate() {
        float rot = Input.GetAxis("Rotate");
        FindCenterPoint();
        transform.RotateAround(centerPoint, Vector3.up, rot * Time.deltaTime * rotationSpeed);
    }


    void FindCenterPoint() {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit))
        {
            centerPoint = hit.point;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(centerPoint, 1f);
    }
}
