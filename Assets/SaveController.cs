using System.Collections;
using System.Collections.Generic;
using System.IO;
using Torec;
using UnityEditor.VersionControl;
using UnityEditor;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;
using MarchingCubes;

public class SaveController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject marching;
    public GameObject creature;
    Mesh mesh;

    public void saveFinalMesh()
    {
        mesh = marching.GetComponent<MeshFilter>().mesh;
        ModelExporter.ExportObject("Assets/Saves/model.fbx", marching);
    }

    public void saveProject()
    {
        mesh = marching.GetComponent<MeshFilter>().mesh;
        PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/modelPrefab.prefab", creature);
    }

    public void loadProject()
    {
        Vector3 position = creature.transform.position;
        Destroy(creature);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/modelPrefab");
        GameObject newObj = Instantiate(prefab, position, Quaternion.identity);
        newObj.transform.GetChild(1).GetComponent<MarchingLogic>().UpdateMesh();
    }
}
