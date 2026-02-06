using System.Collections;
using System.Collections.Generic;
using TransformGizmos;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class IUController : MonoBehaviour
{
    // Start is called before the first frame update
    public SpineObject spine = null;

    public string status;
    public GameObject segmentPrefab;
    public GizmoController gizmo;
    public LayerMask Mask; // (or public Layermask Mask)
    void Start()
    {
        status = "scale";
        
            
            
    }

    // Update is called once per frame
  

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity,Mask))
            {
                Debug.Log(hit.transform.gameObject.layer);
                if (hit.transform.tag == "Selectable") {
                    CameraLogic cameraScript = Camera.main.GetComponent<CameraLogic>();
                    SpineObject spine = hit.transform.parent.GetComponent<SpineObject>();
                    if (spine != null)
                    {
                        Debug.Log(spine.partOf);
                        if (spine.partOf != null)
                        {
                            if (gizmo.m_targetObject == spine.partOf)
                            {
                                gizmo.changeTarget(hit.transform.parent.gameObject);
                                cameraScript.changeTarget(hit.transform.parent.transform);
                            } else
                            {
                                if (spine.partOf == gizmo.m_targetObject.GetComponent<SpineObject>().partOf)
                                {
                                    gizmo.changeTarget(hit.transform.parent.gameObject);
                                    cameraScript.changeTarget(hit.transform.parent.transform);
                                }
                                else
                                {
                                    gizmo.changeTarget(spine.partOf);
                                    cameraScript.changeTarget(spine.partOf.transform);
                                }
                                
                            }
                            
                        } else
                        {
                            gizmo.changeTarget(hit.transform.parent.gameObject);
                            cameraScript.changeTarget(hit.transform.parent.transform);
                        }
                    } else
                    {
                        gizmo.changeTarget(hit.transform.parent.gameObject);
                        cameraScript.changeTarget(hit.transform.parent.transform);
                    }
                    
                    spine = hit.transform.GetComponentInParent<SpineObject>();
                    cameraScript.changeTarget(hit.transform);
                    
                } else if (hit.transform.parent.name == "AddNextSeg")
                {
                    Transform segmentParent = hit.transform.parent.parent;

                    SpineController controllerHit = segmentParent.GetComponent<SpineController>();
                    Vector3 position = segmentParent.GetChild(1).position;
                    GameObject newSegment = Instantiate(segmentPrefab, segmentParent.position + new Vector3(0, 7, 0), Quaternion.identity, segmentParent.parent);
                    SpineController controllerNew = newSegment.GetComponent<SpineController>();
                    controllerNew.changePositionOfSpine(false, position);
                    controllerNew.previousSegment = controllerHit;
                    controllerHit.nextSegment = controllerNew;

                } else if (hit.transform.parent.name == "AddPrevSeg")
                {
                    Transform segmentParent = hit.transform.parent.parent;

                    SpineController controllerHit = segmentParent.GetComponent<SpineController>();
                    Vector3 position = segmentParent.GetChild(4).position;
                    GameObject newSegment = Instantiate(segmentPrefab, segmentParent.position + new Vector3(0, -7, 0), Quaternion.identity, segmentParent.parent);
                    SpineController controllerNew = newSegment.GetComponent<SpineController>();
                    controllerNew.changePositionOfSpine(true, position);
                    controllerNew.nextSegment = controllerHit;
                    controllerHit.previousSegment = controllerNew;
                }





            }
        }
    }
    
}
