using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool moving = false;

    public float distance = 15f;    // Distancia de la cámara al objetivo
    public float speedX = 240f;     // Velocidad de rotación horizontal
    public float speedY = 240f;     // Velocidad de rotación vertical
    public float minYAngle = -80f;  // Límite mínimo del ángulo en el eje X (ángulo de elevación)
    public float maxYAngle = 80f;   // Límite máximo del ángulo en el eje X (ángulo de elevación)

    private float currentX = 0f;    // Ángulo actual en el eje X
    private float currentY = 0f;    // Ángulo actual en el eje Y


    public GameObject Object;
    void Start()
    {
        
        targetPosition = target.transform.position;
        speedMove = 50.0f;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        // Actualizamos la posición y rotación de la cámara
        transform.position = position;
        transform.LookAt(target);

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

        if (Input.GetKeyUp(KeyCode.M))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log(hit.transform.name);
                Instantiate(Object, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        
    }

    public void changeTarget(Transform newTarget)
    {
        Vector3 direction = newTarget.position - target.position;
        position = Camera.main.transform.position + direction;
        targetPosition = target.position + direction;
        moving = true;
    }

    public void changeSameTarget(Vector3 direction)
    {
        targetPosition = target.position + direction;
        moving = true;
    }

    void RotateCamera()
    {
        
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            // Actualizamos los ángulos de rotación basados en la entrada del mouse
            currentX += mouseX * speedX * Time.deltaTime;
            currentY += mouseY * speedY * Time.deltaTime;

            // Limitamos el ángulo de rotación en el eje X entre -80 y 80 grados
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

            // Aplicamos la rotación alrededor del objetivo
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
            Vector3 position = target.position - (rotation * Vector3.forward * distance);

            // Actualizamos la posición y rotación de la cámara
            transform.position = position;
            transform.LookAt(target);



        }

        

            // -------------------Code for Zooming Out------------
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= 125)
            {
                distance += +0.5f;
                //Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
                Vector3 position = target.position - (rotation * Vector3.forward * distance);

                // Actualizamos la posición y rotación de la cámara
                transform.position = position;
                transform.LookAt(target);
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
                distance += -0.5f;
                //Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
                Vector3 position = target.position - (rotation * Vector3.forward * distance);

                // Actualizamos la posición y rotación de la cámara
                transform.position = position;
                transform.LookAt(target);
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

        if (Input.GetKeyUp(KeyCode.F))
        {
            transform.RotateAround(target.position,
                                            new Vector3(1, 0, 0),
                                            90);
            transform.rotation = Quaternion.LookRotation(target.position);
            
            

        }

    }
}
