using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    
    private MeshFilter _myMeshFilter;
    private MeshRenderer _myMeshRenderer;
    private BoxCollider _myBoxCollider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Combine();
    }

    public void Combine()
    {

        Quaternion myRotation = Quaternion.identity;
        Vector3 myPosition = Vector3.zero;

        MeshFilter[] meshFilter = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstance = new CombineInstance[meshFilter.Length];

        Material[] myMats = GetComponentInChildren<MeshRenderer>().sharedMaterials;

        for (int i = 0; i < meshFilter.Length; i++)
        {
            combineInstance[i].subMeshIndex = 0;
            combineInstance[i].mesh = meshFilter[i].sharedMesh;
            combineInstance[i].transform = meshFilter[i].transform.localToWorldMatrix;
            

        }

        Mesh newMesh = new Mesh() { name = "CombinedMesh" };
        newMesh.CombineMeshes(combineInstance);

        _myMeshFilter = gameObject.AddComponent<MeshFilter>();

        _myMeshFilter.sharedMesh = newMesh;

        _myMeshRenderer = gameObject.AddComponent<MeshRenderer>();

        _myBoxCollider = gameObject.AddComponent<BoxCollider>();

        foreach (var mat in myMats)
        {
            _myMeshRenderer.material = mat;
        }

        transform.rotation = myRotation;
        transform.position = myPosition;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        gameObject.isStatic = true;
    }
}