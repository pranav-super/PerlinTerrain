using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise_cubes : MonoBehaviour
{

    [SerializeField]
    //private GameObject blockPrefab;

    private static int chunkSizeBlocks = 7;//MarchingCubes3Clean.chunkDim; //if a chunk has 3 blocks on an axis, there are 4 verticies on that axis.

    private static int chunkSizeVerts = chunkSizeBlocks + 1;//MarchingCubes3Clean.chunkDim + 1; //max mesh verticies is 65536. 65536/8 (for each cube), and the cube root of that is blocks/chunk, which is MAX 20.

    private float[,,] ogChunkData = new float[chunkSizeBlocks, chunkSizeBlocks, chunkSizeBlocks];

    private float[,,] hollowedChunkData = new float[chunkSizeBlocks, chunkSizeBlocks, chunkSizeBlocks];

    private float threshold = 0.5f;

    public float noiseScale = 0.05f;

    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        //also from youtube
        for (int x = 0; x < chunkSizeBlocks; x++)
        {
            for (int y = 0; y < chunkSizeBlocks; y++)
            {
                for (int z = 0; z < chunkSizeBlocks; z++)
                {
                    float noise = Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale);
                    if (noise >= threshold) //here we pass floats instead so that Perlin behaves
                    {
                        //sampling unity's perlin noise with an integer returns the same value, which was less than the threshold
                        //Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);//not rotated is what quaternion.identity says
                        ogChunkData[chunkSizeBlocks- x - 1, chunkSizeBlocks- y - 1, z] = 1;

                    }
                    else
                    {
                        ogChunkData[chunkSizeBlocks - x - 1, chunkSizeBlocks- y - 1, z] = 0;//idk if it instantiates to all 0's
                    }
                }
            }
        }

        hollowedChunkData = hollowOut(ogChunkData);

        //now that we have our verticies, lets march some cubes?

        //some for loop logic to get to one cube
        //now that we are at a cube:
        //check each of the 8 verticies to see if it is on or off
        //index = 0;
        //for all of the 8
        //index += (vertex value [1 or 0]) * 2^(vertex index)
        //lookup, then make triangle

        CombineInstance[] wholeMesh = new CombineInstance[(int)Mathf.Pow(chunkSizeBlocks, 3)];

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        //int[] initialVoxels = { (int) hollowedChunkData[0, 0, 0], (int) hollowedChunkData[1, 0, 0], (int) hollowedChunkData[1, 0, 1], (int) hollowedChunkData[0, 0, 1], (int) hollowedChunkData[0, 1, 0], (int) hollowedChunkData[1, 1, 0], (int) hollowedChunkData[1, 1, 1], (int) hollowedChunkData[0, 1, 1] };

        Mesh temp = prefab.GetComponent<MeshFilter>().sharedMesh;//MarchingCubes3.perCube(new Vector3(0, 0, 0), initialVoxels, 0);

        ////foreach (int i in initialVoxels)
        ////    Debug.Log(i);

        ////mesh.triangles = temp.triangles;
        ////mesh.vertices = temp.vertices;

        ////mesh.RecalculateBounds();
        ////mesh.RecalculateNormals();

        wholeMesh[0] = new CombineInstance();
        wholeMesh[0].mesh = temp;

        //Debug.Log(chunkSizeBlocks);

        //per chunk
        for (int x = 0; x < chunkSizeBlocks; x++)
        {
            //Debug.Log("slatt x");
            for (int y = 0; y < chunkSizeBlocks; y++)
            {
                //Debug.Log("slatt y");
                for (int z = 0; z < chunkSizeBlocks; z++)
                {
                    //Debug.Log("slatt z");
                    /*if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }*/
                    /*//set blockNum somehow
                    int blockNum = (int)(z + y * (Mathf.Pow(chunkSizeBlocks, 1)) + x * (Mathf.Pow(chunkSizeBlocks, 2)));
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
                    //merge meshes*/

                    int blockNum = (int)(z + y * (Mathf.Pow(chunkSizeBlocks, 1)) + x * (Mathf.Pow(chunkSizeBlocks, 2)));
                    //int voxel = (int)chunkData[x, y, z];
                    //Debug.Log("Block #:" + blockNum + ", isOn: " + chunkData[x, y, z]);
                    if ((int)hollowedChunkData[x, y, z] > 0) {
                        wholeMesh[blockNum] = new CombineInstance();
                        wholeMesh[blockNum].mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
                        Debug.Log(prefab.GetComponent<MeshFilter>().mesh.vertices.Length);
                        wholeMesh[blockNum].mesh.vertices = new Vector3[] { new Vector3(wholeMesh[blockNum].mesh.vertices[0].x + x, wholeMesh[blockNum].mesh.vertices[0].y + y, wholeMesh[blockNum].mesh.vertices[0].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[1].x + x, wholeMesh[blockNum].mesh.vertices[1].y + y, wholeMesh[blockNum].mesh.vertices[1].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[2].x + x, wholeMesh[blockNum].mesh.vertices[2].y + y, wholeMesh[blockNum].mesh.vertices[2].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[3].x + x, wholeMesh[blockNum].mesh.vertices[3].y + y, wholeMesh[blockNum].mesh.vertices[3].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[4].x + x, wholeMesh[blockNum].mesh.vertices[4].y + y, wholeMesh[blockNum].mesh.vertices[4].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[5].x + x, wholeMesh[blockNum].mesh.vertices[5].y + y, wholeMesh[blockNum].mesh.vertices[5].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),
                                                                            new Vector3(wholeMesh[blockNum].mesh.vertices[7].x + x, wholeMesh[blockNum].mesh.vertices[7].y + y, wholeMesh[blockNum].mesh.vertices[7].z + z),
                         new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z), new Vector3(wholeMesh[blockNum].mesh.vertices[6].x + x, wholeMesh[blockNum].mesh.vertices[6].y + y, wholeMesh[blockNum].mesh.vertices[6].z + z),};
                        
                        //Instantiate(prefab, MarchingCubes3Clean.beginningCoordinates(new Vector3(0, 0, 0), blockNum, chunkSizeBlocks), Quaternion.identity);
                    }
                }
            }
        }


        mesh = GetComponent<MeshFilter>().mesh; //mesh is invisible now.

        mesh.Clear();

        mesh.CombineMeshes(wholeMesh);

        //mesh.Optimize();

        //MeshCollider meshCollider = GetComponent<MeshCollider>();

        //meshCollider.sharedMesh = GetComponent<MeshFilter>().mesh; //this won't work but i want it to :(, also it shows up as black and unlit???
        //I think the mesh is centered but idk...

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        /*mesh.Clear();

        mesh.CombineMeshes(wholeMesh);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();*

        //Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh = GetComponent<MeshFilter>().mesh; //mesh is invisible now.

        mesh.Clear();

        mesh.CombineMeshes(wholeMesh);

        //mesh.Optimize();

        //MeshCollider meshCollider = GetComponent<MeshCollider>();

        //meshCollider.sharedMesh = GetComponent<MeshFilter>().mesh; //this won't work but i want it to :(, also it shows up as black and unlit???
        //I think the mesh is centered but idk...

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();*/


    }

    float[,,] hollowOut(float[,,] arr) //i think arr is passed as a reference so im pretty sure modifying it in this method as a void would be fine but im being careful. Test this later, making this a void and just modifying arr!!!!!!!!
    { //doesn't work
        List<Vector3> modPoints = new List<Vector3>(); //all the points we change, any point in this list was hollowed so set it to 0

        int hollowCount = 0;
        int totalCount = 0;

        for (int x = 0; x < chunkSizeBlocks; x++)
        {
            for (int y = 0; y < chunkSizeBlocks; y++)
            {
                for (int z = 0; z < chunkSizeBlocks; z++)
                {
                    if(arr[x,y,z] == 1)
                    {
                        totalCount++;
                    }
                    int startPosX = (x - 1 < 0) ? x : x - 1;
                    int startPosY = (y - 1 < 0) ? y : y - 1;
                    int startPosZ = (z - 1 < 0) ? z : z - 1;

                    int endPosX = (x + 1 > chunkSizeBlocks - 1) ? x : x + 1;
                    int endPosY = (y + 1 > chunkSizeBlocks - 1) ? y : y + 1;
                    int endPosZ = (z + 1 > chunkSizeBlocks - 1) ? z : z + 1;

                    int numAlive = 0;
                    int numAround = 6;//(endPosX - startPosX + 1)*(endPosY - startPosY + 1)*(endPosZ - startPosZ + 1) - 1; //if it's verticies, I think numaround will be 26, to be same, and I'll check probably using loops (or something exhaustive, like the if statements but like 26 of them for all combinations of x +/- 1, y +/- 1, and z +/- 1)

                    if(startPosX == x || startPosY == y || startPosZ == z || endPosX == x || endPosY == y || endPosZ == z) //ignore edge cases entirely
                    {
                        continue;
                    }

                    /*for (int i = startPosX; i <= endPosX; i++)
                        if (arr[i, y, z] == 1 && i != x)
                            numAlive++;

                    for (int i = startPosY; i <= endPosY; i++)
                        if (arr[x, i, z] == 1 && i != y)
                            numAlive++;

                    for (int i = startPosZ; i <= endPosZ; i++)
                        if (arr[x, y, i] == 1 && i != z)
                            numAlive++;*/
                    if (arr[x+1,y,z] == 1)
                        numAlive++;
                    if (arr[x-1, y, z] == 1)
                        numAlive++;
                    if (arr[x, y+1, z] == 1)
                        numAlive++;
                    if (arr[x, y-1, z] == 1)
                        numAlive++;
                    if (arr[x, y, z+1] == 1)
                        numAlive++;
                    if (arr[x, y, z-1] == 1)
                        numAlive++;

                    if (numAround > 0 && numAlive >= numAround)
                    {
                        //Debug.Log("here");
                        modPoints.Add(new Vector3(x, y, z));
                        hollowCount++;
                    }
                }
            }
        }

        foreach (Vector3 v in modPoints)
            arr[(int)v.x, (int)v.y, (int)v.z] = 0;

        //Debug.Log(hollowCount);
        //Debug.Log(totalCount);

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
