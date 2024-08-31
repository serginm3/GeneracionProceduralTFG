using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class IUController : MonoBehaviour
{
    // Start is called before the first frame update
    public SpineObject spine = null;
    public Slider[] sliders;
    public string status;
    void Start()
    {
        status = "scale";
        sliders = new Slider[4];
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            sliders[i] = this.transform.GetChild(i).gameObject.GetComponent<Slider>();
            Debug.Log(this.transform.GetChild(i).gameObject.GetComponent<Slider>());
        }
            
            
    }

    // Update is called once per frame
    public void ChangeSpine()
    {
        if (spine != null)
        {
            
            if (status == "scale")
            {
                spine.radiousX = sliders[0].value;
                spine.radiousY = sliders[1].value;
                spine.radiousZ = sliders[2].value;
                spine.scale = sliders[3].value;
            } else if (status == "rotation")
            {
                spine.rotationX = sliders[0].value;
                spine.rotationY = sliders[1].value;
                spine.rotationZ = sliders[2].value;
                spine.scale = sliders[3].value;
            }
            
        }
       

    }
    public void ChangeStatus()
    {
        if (status == "scale")
        {
            status = "rotation";

           
            sliders[0].maxValue = 360;
            sliders[1].maxValue = 360;
            sliders[2].maxValue = 360;

            sliders[0].SetValueWithoutNotify(spine.rotationX);
            sliders[1].SetValueWithoutNotify(spine.rotationY);
            sliders[2].SetValueWithoutNotify(spine.rotationZ);
            sliders[3].SetValueWithoutNotify(spine.scale);


        }
        else if (status == "rotation")
        {
            status = "scale";

            sliders[0].SetValueWithoutNotify(spine.radiousX);
            sliders[1].SetValueWithoutNotify(spine.radiousY);
            sliders[2].SetValueWithoutNotify(spine.radiousZ);
            sliders[3].SetValueWithoutNotify(spine.scale);

            sliders[0].maxValue = 15;
            sliders[1].maxValue = 15;
            sliders[2].maxValue = 15;

            
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                spine = hit.transform.GetComponentInParent<SpineObject>();
                if (status == "scale")
                {
                    sliders[0].SetValueWithoutNotify(spine.radiousX);
                    sliders[1].SetValueWithoutNotify(spine.radiousY);
                    sliders[2].SetValueWithoutNotify(spine.radiousZ);
                    sliders[3].SetValueWithoutNotify(spine.scale);
                }
                else if (status == "rotation")
                {
                    sliders[0].SetValueWithoutNotify(spine.rotationX);
                    sliders[1].SetValueWithoutNotify(spine.rotationY);
                    sliders[2].SetValueWithoutNotify(spine.rotationZ);
                    sliders[3].SetValueWithoutNotify(spine.scale);
                }
                
               

            }
        }
    }
    
}
