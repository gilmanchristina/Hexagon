using UnityEngine;
using System.Collections;
 
public class Hex : MonoBehaviour
{
    [HideInInspector]
    public Vector3 gridposition;
    [HideInInspector]
    public GameObject tileHex;

    public void hexMesh(Vector3 gridposition, Material hex_Material)
    {
        tileHex = new GameObject("Hex");
        tileHex.isStatic = true;
        tileHex.tag = "Hex";
        tileHex.AddComponent<MeshFilter>();
        tileHex.AddComponent<MeshRenderer>();
        tileHex.AddComponent<MeshCollider>();
        tileHex.AddComponent<ClickableTile>();
        tileHex.transform.position = gridposition;
        Vector2 center = Vector2.zero;
        float size = 0.4330127f / 0.75f;

        float[] angle_deg = { 0, 0, 0, 0, 0, 0 };
        float[] angle_rad = { 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < 6; i++){
            angle_deg[i] = 60 * i + 90;
            angle_rad[i] = Mathf.PI / 180 * angle_deg[i];
        }
        float floorLevel = 0;
        //vertices
        #region verts
        Vector3[] vertices = new Vector3[7]{
            /*0*/new Vector3(center.x, floorLevel, center.y),
            /*1*/new Vector3(center.x + size * Mathf.Cos(angle_rad[0]), floorLevel, center.y + size * Mathf.Sin(angle_rad[0])),
            /*2*/new Vector3(center.x + size * Mathf.Cos(angle_rad[1]), floorLevel, center.y + size * Mathf.Sin(angle_rad[1])),
            /*3*/new Vector3(center.x + size * Mathf.Cos(angle_rad[2]), floorLevel, center.y + size * Mathf.Sin(angle_rad[2])),
            /*4*/new Vector3(center.x + size * Mathf.Cos(angle_rad[3]), floorLevel, center.y + size * Mathf.Sin(angle_rad[3])),
			/*5*/new Vector3(center.x + size * Mathf.Cos(angle_rad[4]), floorLevel, center.y + size * Mathf.Sin(angle_rad[4])),
            /*6*/new Vector3(center.x + size * Mathf.Cos(angle_rad[5]), floorLevel, center.y + size * Mathf.Sin(angle_rad[5]))
        };
        #endregion
        //triangles
        #region triangles
        int[] triangles = new int[18];

        triangles[0] = 0; triangles[1] = 2; triangles[2] = 1;
        triangles[3] = 0; triangles[4] = 3; triangles[5] = 2;
        triangles[6] = 0; triangles[7] = 4; triangles[8] = 3;
        triangles[9] = 0; triangles[10] = 5; triangles[11] = 4;
        triangles[12] = 0; triangles[13] = 6; triangles[14] = 5;
        triangles[15] = 0; triangles[16] = 1; triangles[17] = 6;

        #endregion
        //normals
        #region normals
        Vector3[] normals = new Vector3[7];

        normals[0] = Vector3.up;
        normals[1] = Vector3.up;
        normals[2] = Vector3.up;
        normals[3] = Vector3.up;
        normals[4] = Vector3.up;
        normals[5] = Vector3.up;
        normals[6] = Vector3.up;

        #endregion
        //uvs
        #region uv
        Vector2[] uv = new Vector2[7];

        uv[0] = new Vector2(0.5f, 0.5f);
        uv[1] = new Vector2(0.5f, 1f);
        uv[2] = new Vector2(0, (0.5f + size * Mathf.Sin(30 * Mathf.PI / 180)));
        uv[3] = new Vector2(0, (0.5f - size * Mathf.Sin(30 * Mathf.PI / 180)));
        uv[4] = new Vector2(0.5f, 0);
        uv[5] = new Vector2(1, (0.5f - size * Mathf.Sin(30 * Mathf.PI / 180)));
        uv[6] = new Vector2(1, (0.5f + size * Mathf.Sin(30 * Mathf.PI / 180)));

        #endregion

        #region finalize
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        tileHex.GetComponent<MeshFilter>().mesh = mesh;
        tileHex.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        tileHex.GetComponent<MeshCollider>().sharedMesh = mesh;
        tileHex.GetComponent<MeshRenderer>().material = hex_Material;

        #endregion
    }
}