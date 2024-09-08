using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        radiousX = this.transform.localScale.x;
        radiousY = this.transform.localScale.y;
        radiousZ = this.transform.localScale.z;
        Vector3 eulerRotation = this.transform.eulerAngles;
        rotationX = eulerRotation.x;
        rotationY = eulerRotation.y;
        rotationZ = eulerRotation.z;
    }
}
