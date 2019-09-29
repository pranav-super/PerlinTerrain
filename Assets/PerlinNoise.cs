using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{

    [SerializeField]
    //private GameObject blockPrefab;

    private static int chunkSizeVerts = MarchingCubes3Clean.chunkDim + 1; //max mesh verticies is 65536. 65536/8 (for each cube), and the cube root of that is blocks/chunk, which is MAX 20.

    private static int chunkSizeBlocks = MarchingCubes3Clean.chunkDim; //if a chunk has 3 blocks on an axis, there are 4 verticies on that axis.

    private float[,,] chunkData = new float[chunkSizeVerts, chunkSizeVerts, chunkSizeVerts];

    private float threshold = 0.5f;

    public float noiseScale = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        //also from youtube
        for (int x = 0; x < chunkSizeVerts; x++)
        {
            for (int y = 0; y < chunkSizeVerts; y++)
            {
                for (int z = 0; z < chunkSizeVerts; z++)
                {
                    float noise = Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale);
                    if (noise >= threshold) //here we pass floats instead so that Perlin behaves
                    {
                        //sampling unity's perlin noise with an integer returns the same value, which was less than the threshold
                        //Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);//not rotated is what quaternion.identity says
                        //^^ this is if you treat each point as a cube. I will treat each point as a vertex instead.
                        /*bool xSafe = false;
                        bool ySafe = false;
                        bool zSafe = false;
                        if (x == chunkSize - 1)
                        {
                            //check edges (x - 1)
                        }
                        else
                        {
                            //check edges (x + 1)
                        }
                        if (y == chunkSize - 1)
                        {
                            //check y edges accordingly
                        }
                        else
                        {
                            //check edges (y + 1)
                        }
                        if (z == chunkSize - 1)
                        {
                            //check z edges accordingly
                        }
                        else
                        {
                            //check edges (z + 1)
                        }
                        if (xSafe && ySafe && zSafe) { //make sure that we have checked edges, if all edges are filled, then it is safe to set to 0; we are checking if a cube is like covered or not
                            chunkData[chunkSize - x - 1, chunkSize - y - 1, z] = 0;
                        }
                        else
                        {
                            chunkData[chunkSize - x - 1, chunkSize - y - 1, z] = 1;
                        }*/
                        chunkData[chunkSizeVerts - x - 1, chunkSizeVerts - y - 1, z] = 1;

                    }
                    else
                    {
                        chunkData[chunkSizeVerts - x - 1, chunkSizeVerts - y - 1, z] = 0;//idk if it instantiates to all 0's
                    }
                }
            }
        }

        chunkData = hollowOut(chunkData);

        //now that we have our verticies, lets march some cubes?

        //some for loop logic to get to one cube
            //now that we are at a cube:
            //check each of the 8 verticies to see if it is on or off
            //index = 0;
            //for all of the 8
                //index += (vertex value [1 or 0]) * 2^(vertex index)
            //lookup, then make triangle
        
        CombineInstance[] wholeMesh = new CombineInstance[(int) Mathf.Pow(chunkSizeBlocks, 3)];

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        int[] initialVoxels = { (int)chunkData[0, 0, 0], (int)chunkData[1, 0, 0], (int)chunkData[1, 0, 1], (int)chunkData[0, 0, 1], (int)chunkData[0, 1, 0], (int)chunkData[1, 1, 0], (int)chunkData[1, 1, 1], (int)chunkData[0, 1, 1] };

        Mesh temp = MarchingCubes3.perCube(new Vector3(0, 0, 0), initialVoxels, 0);

        //foreach (int i in initialVoxels)
        //    Debug.Log(i);

        //mesh.triangles = temp.triangles;
        //mesh.vertices = temp.vertices;

        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

        wholeMesh[0] = new CombineInstance();
        wholeMesh[0].mesh = temp;

        //per chunk
        for (int x = 0; x < chunkSizeVerts-1; x++) //or x < chunkSizeBlocks
        {
            //Debug.Log("slatt x");
            for (int y = 0; y < chunkSizeVerts-1; y++)
            {
                //Debug.Log("slatt y");
                for (int z = 0; z < chunkSizeVerts-1; z++)
                {
                    //Debug.Log("slatt z");
                    if (x==0 && y == 0 && z == 0)
                    {
                        continue;
                    }
                    //set blockNum somehow
                    int blockNum = (int) (z + y * (Mathf.Pow(chunkSizeBlocks, 1)) + x * (Mathf.Pow(chunkSizeBlocks, 2)));
                    //percube
                    int[] voxels = { (int)chunkData[x, y, z], (int)chunkData[x + 1, y, z], (int)chunkData[x + 1, y, z + 1], (int)chunkData[x, y, z + 1], (int)chunkData[x, y + 1, z], (int)chunkData[x + 1, y + 1, z], (int)chunkData[x + 1, y + 1, z + 1], (int)chunkData[x, y + 1, z + 1] }; //this is a line worth checking, I made it match up with MarchingCubes3.localVerticies
                    Mesh m = MarchingCubes3Clean.perCube(new Vector3(0, 0, 0), voxels, blockNum);
                    //^^ this causes unity to freeze up

                    //if combineInstance has too many verts: //IMPLEMENT THIS LOGIC LATER, MAYBE IF COMBINING CHUNKS???
                    //save the current combineInstance to a mesh
                    //make a new one
                    //Debug.Log("slatt");
                    //Debug.Log(wholeMesh[blockNum]);
                    wholeMesh[blockNum] = new CombineInstance();
                    wholeMesh[blockNum].mesh = m;
                    //Debug.Log(wholeMesh[blockNum]);
                    //wholeMesh[blockNum].transform. = MarchingCubes3Clean.beginningCoordinates(new Vector3(0, 0, 0), blockNum);//comes with the mesh???
                    //merge meshes
                }
            }
        }

        /*mesh.Clear();

        mesh.CombineMeshes(wholeMesh);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();*/

        //Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh = GetComponent<MeshFilter>().mesh; //mesh is invisible now.

        mesh.Clear();

        mesh.CombineMeshes(wholeMesh);

        //mesh.Optimize();

        //MeshCollider meshCollider = GetComponent<MeshCollider>();

        //meshCollider.sharedMesh = GetComponent<MeshFilter>().mesh; //this won't work but i want it to :(, also it shows up as black and unlit???
        //I think the mesh is centered but idk...

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


    }

    float[,,] hollowOut(float[,,] arr) //i think arr is passed as a reference so im pretty sure modifying it in this method as a void would be fine but im being careful. Test this later, making this a void and just modifying arr!!!!!!!!
    {
        List<Vector3> modPoints = new List<Vector3>(); //all the points we change, any point in this list was hollowed so set it to 0
        for (int x = 0; x < chunkSizeVerts; x++)
        {
            for (int y = 0; y < chunkSizeVerts; y++)
            {
                for (int z = 0; z < chunkSizeVerts; z++)
                {
                    int startPosX = (x - 1 < 0) ? x : x - 1;
                    int startPosY = (y - 1 < 0) ? y : y - 1;
                    int startPosZ = (z - 1 < 0) ? z : z - 1;

                    int endPosX = (x + 1 > chunkSizeVerts - 1) ? x : x + 1;
                    int endPosY = (y + 1 > chunkSizeVerts - 1) ? y : y + 1;
                    int endPosZ = (z + 1 > chunkSizeVerts - 1) ? z : z + 1;

                    int numAlive = 0;
                    int numAround = (endPosX - startPosX) + (endPosY - startPosY) + (endPosZ - startPosZ);

                    for (int i = startPosX; i < endPosX+1; i++)
                        if (arr[i, y, z] == 1 && i != x)
                        numAlive++;

                    for (int i = startPosX; i < endPosX + 1; i++)
                        if (arr[x, i, z] == 1 && i != y)
                            numAlive++;

                    for (int i = startPosX; i < endPosX + 1; i++)
                        if (arr[x, y, i] == 1 && i != z)
                            numAlive++;

                    if (numAround > 0 && numAlive == numAround)
                        modPoints.Add(new Vector3(x, y, z));
                }
            }
        }

        foreach (Vector3 v in modPoints)
            arr[(int) v.x, (int) v.y, (int) v.z] = 0;

        return arr;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //borrowed from Nova840's solution on youtube, i think this is available online
    public static float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;//no idea how this works, but im pretty sure this isnt the montecarlo approach lol
        return abc / 6f;
    }
}
