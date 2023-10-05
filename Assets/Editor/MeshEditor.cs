using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MeshGenerator))]
public class MeshEditor : Editor
{
    private MeshGenerator meshGenerator;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();
        if (GUILayout.Button ("Generate Mesh")) {
            meshGenerator.ConstructMesh();
        }

    }
    
    void OnEnable () {
        meshGenerator = (MeshGenerator) target;
        Tools.hidden = true;
    }

    void OnDisable () {
        Tools.hidden = false;
    }
    
}