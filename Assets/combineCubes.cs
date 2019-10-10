using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combineCubes : MonoBehaviour
{
    public GameObject prefab;
    int chunkSizeBlocks = 2;
    // Start is called before the first frame update
    void Start()
    {


        /* https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
        MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[1];

        combine[0].mesh = meshFilter.sharedMesh;
        combine[0].transform = meshFilter.transform.localToWorldMatrix;
        meshFilter.gameObject.SetActive(false);

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
        */

        MeshFilter meshFilter = prefab.GetComponent<MeshFilter>(); //instantiates every element in array by default, like C#
        CombineInstance[] wholeMesh = new CombineInstance[2];

        wholeMesh[0].mesh = meshFilter.sharedMesh;
        wholeMesh[0].transform = meshFilter.transform.localToWorldMatrix;

        wholeMesh[1].mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        int x = 1;
        int y = 0;
        int z = 0;
        int blockNum = 1;
        //for (int i = 0; i < 24; i++)
        //{
            //wholeMesh[1].mesh.vertices[i] = new Vector3(wholeMesh[blockNum].mesh.vertices[i].x + x, wholeMesh[blockNum].mesh.vertices[i].y + y, wholeMesh[blockNum].mesh.vertices[i].z + z);
        wholeMesh[1].mesh = meshFilter.sharedMesh;
        /*for(int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                switch(c)
                {
                    case 0:
                        Debug.Log(meshFilter.transform.localToWorldMatrix.GetRow(r).w);
                        break;
                    case 1:
                        Debug.Log(meshFilter.transform.localToWorldMatrix.GetRow(r).x);
                        break;
                    case 2:
                        Debug.Log(meshFilter.transform.localToWorldMatrix.GetRow(r).y);
                        break;
                    case 3:
                        Debug.Log(meshFilter.transform.localToWorldMatrix.GetRow(r).z);
                        break;
                    default:
                        break;
                }
            }
            Debug.Log(" ");
        }*/
        wholeMesh[1].transform = Matrix4x4.TRS(new Vector3(1, 1, 1), meshFilter.transform.localToWorldMatrix.rotation, new Vector3(1, 1, 1));//meshFilter.transform.localToWorldMatrix; //the missing link
        //}

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(wholeMesh);

        transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        transform.GetComponent<MeshFilter>().mesh.Optimize();









        /*Mesh m = prefab.GetComponent<MeshFilter>().sharedMesh;

        //Mesh mesh = GetComponent<MeshFilter>().mesh;

        //mesh.Clear();

        CombineInstance[] wholeMesh = new CombineInstance[1];

        wholeMesh[0] = new CombineInstance();
        wholeMesh[0].mesh = m;


        /*wholeMesh[1] = new CombineInstance();
        wholeMesh[1].mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        //Debug.Log(prefab.GetComponent<MeshFilter>().mesh.vertices.Length);
        //ArrayList listComprehension = new ArrayList();
        int x = 1;
        int y = 0;
        int z = 0;
        int blockNum = 1;
        for (int i = 0; i < 24; i++)
        {
            wholeMesh[1].mesh.vertices[i] = new Vector3(wholeMesh[blockNum].mesh.vertices[i].x + x, wholeMesh[blockNum].mesh.vertices[i].y + y, wholeMesh[blockNum].mesh.vertices[i].z + z);
        }

        //wholeMesh[1].mesh.vertices = listComprehension.ToArray();
        wholeMesh[1].mesh.RecalculateBounds();
        wholeMesh[1].mesh.RecalculateNormals();*

        /*mesh = GetComponent<MeshFilter>().mesh; //mesh is invisible now.

        //mesh.Clear();

        mesh.CombineMeshes(wholeMesh, true);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();*

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(wholeMesh);
        transform.gameObject.SetActive(true);

        /*wholeMesh[1].mesh.vertices = new Vector3[] { new Vector3(wholeMesh[1].mesh.vertices[0].x + 1, wholeMesh[blockNum].mesh.vertices[0].y + y, wholeMesh[blockNum].mesh.vertices[0].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[1].x + x, wholeMesh[blockNum].mesh.vertices[1].y + y, wholeMesh[blockNum].mesh.vertices[1].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[2].x + x, wholeMesh[blockNum].mesh.vertices[2].y + y, wholeMesh[blockNum].mesh.vertices[2].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[3].x + x, wholeMesh[blockNum].mesh.vertices[3].y + y, wholeMesh[blockNum].mesh.vertices[3].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[4].x + x, wholeMesh[blockNum].mesh.vertices[4].y + y, wholeMesh[blockNum].mesh.vertices[4].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[5].x + x, wholeMesh[blockNum].mesh.vertices[5].y + y, wholeMesh[blockNum].mesh.vertices[5].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[7].x + x, wholeMesh[blockNum].mesh.vertices[7].y + y, wholeMesh[blockNum].mesh.vertices[7].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[8].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[9].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[10].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[11].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[12].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[13].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[14].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[15].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[16].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[17].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[18].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[19].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[20].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[21].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[22].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                     new Vector3(wholeMesh[1].mesh.vertices[23].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z)};*/

        /*for(int i = 0; i < m.vertexCount; i++)
        {
            Debug.Log(i + " " + m.vertices[i]);
        }*/



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
