using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform target;
    public float speed = 2.0f;
    public float speedMove;
    public Camera secondCamera;
    public Vector3 position;
    public Vector3 targetPosition;
    public bool moving;
    void Start()
    {
        position = transform.position;
        targetPosition = target.transform.position;
        speedMove = 50.0f;
        moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            var step = speedMove * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, position, step);
            target.transform.position = Vector3.MoveTowards(target.transform.position, targetPosition, step);
            if (transform.position == position)
            {
                moving = false;
            }
        } else
        {
            RotateCamera();
        }

    }

    public void changeTarget(Transform newTarget)
    {
        Vector3 direction = newTarget.position - target.position;
        position = Camera.main.transform.position + direction;
        targetPosition = target.position + direction;
        moving = true;
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(target.position,
                                            target.up,
                                            -Input.GetAxis("Mouse X")*speed);
            Vector3 crosspro = Vector3.Cross(target.up, target.position - transform.position);
            transform.RotateAround(target.position,
                                            crosspro,
                                            -Input.GetAxis("Mouse Y")*speed);
        }

        // -------------------Code for Zooming Out------------
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 125)
            {
                Camera.main.fieldOfView += 2;
                secondCamera.fieldOfView += 2;
            }
                
            if (Camera.main.orthographicSize <= 20)
            {
                Camera.main.orthographicSize += 0.5f;
                secondCamera.orthographicSize += 0.5f;
            }
                

        }
        // ---------------Code for Zooming In------------------------
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > 2)
            {
                Camera.main.fieldOfView -= 2;
                secondCamera.fieldOfView -= 2;
            }
                
            if (Camera.main.orthographicSize >= 1)
            {
                Camera.main.orthographicSize -= 0.5f;
                secondCamera.orthographicSize -= 0.5f;
            }
                
        }

        // -------Code to switch camera between Perspective and Orthographic--------
        if (Input.GetKeyUp(KeyCode.B))
        {
            if (Camera.main.orthographic == true)
            {
                Camera.main.orthographic = false;
                secondCamera.orthographic = false;
            }


            else
            {
                Camera.main.orthographic = true;
                secondCamera.orthographic = true;
            }
                
        }

    }
}
