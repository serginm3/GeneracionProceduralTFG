using MarchingCubes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpine : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myPrefab;
    public GameObject marching;


    public void CreateSpine()
    {
        MarchingLogic marchingScript = marching.GetComponent<MarchingLogic>();
        Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity, marching.transform);
        marchingScript.GetChilds();
        marchingScript.needsRecalculation = true;
    }


}
