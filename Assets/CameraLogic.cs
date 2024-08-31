using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform target;
    public float speed = 2.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
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

    }
}
