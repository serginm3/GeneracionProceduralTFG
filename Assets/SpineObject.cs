using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SpineObject : MonoBehaviour
{
    public string type = "Spine";
    public float radiousX = 1f;
    public float radiousY = 1f;
    public float radiousZ = 1f;
    public float scale = 2f;
    public float rotationX = 0f;
    public float rotationY = 0f;
    public float rotationZ = 0f;
    public GameObject partOf;
    public bool updateRadious = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updateRadious)
        {
            radiousX = this.transform.localScale.x;
            radiousY = this.transform.localScale.y;
            radiousZ = this.transform.localScale.z;
            updateRadious = false;
        }
        
        Vector3 eulerRotation = this.transform.eulerAngles;
        rotationX = eulerRotation.x;
        rotationY = eulerRotation.y;
        rotationZ = eulerRotation.z;
    }

    public void changeRadiousNeighbors()
    {
        float changeX = this.transform.localScale.x - radiousX;
        float changeY = this.transform.localScale.y - radiousY;
        float changeZ = this.transform.localScale.z - radiousZ;
        if (partOf != null)
        {
            GameObject[] spinesArray = partOf.GetComponent<SpineController>().spines;
            int index = -1;

            // Encuentra el índice del objeto actual en el arreglo de spines
            for (int i = 0; i < spinesArray.Length; i++)
            {
                if (spinesArray[i] == this.gameObject)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                int firstIndex = index - 4;
                int lastIndex = index + 4;

                // Aseguramos que los índices estén dentro de los límites del array
                firstIndex = Mathf.Max(firstIndex, 0);
                lastIndex = Mathf.Min(lastIndex, spinesArray.Length);

                // Aplicamos cambios a las spines vecinas
                for (int i = firstIndex; i < index; i++)
                {
                    
                    Transform spineTrans = spinesArray[i].transform;
                    Debug.Log(i);

                    // Escala objetivo basada en el radiousX, radiousY y radiousZ
                    Vector3 targetScale = new Vector3(spineTrans.localScale.x + (changeX*0.25f*(i+1)), spineTrans.localScale.y + (changeY * 0.25f * (i + 1)), spineTrans.localScale.z + (changeZ * 0.25f * (i + 1)));

                    spineTrans.localScale = targetScale;
                    spineTrans.GetComponent<SpineObject>().updateRadious = true;
                    
                }
                for (int i = index + 1; i < lastIndex; i++)
                {
                    
                    Transform spineTrans = spinesArray[i].transform;
                    Debug.Log(i);

                    // Escala objetivo basada en el radiousX, radiousY y radiousZ
                    Vector3 targetScale = new Vector3(spineTrans.localScale.x + (changeX * 0.25f * (lastIndex - i)), spineTrans.localScale.y + (changeY * 0.25f * (lastIndex - i)), spineTrans.localScale.z + (changeZ * 0.25f * (lastIndex - i)));

                    spineTrans.localScale = targetScale;
                    spineTrans.GetComponent<SpineObject>().updateRadious = true;
                    
                }
            }


        }
        updateRadious = true;
    }

}

