using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise_cubes_clean : MonoBehaviour
{

    [SerializeField]
    public GameObject blockPrefab;

    private static int chunkSizeBlocks = 7;//MarchingCubes3Clean.chunkDim; //if a chunk has 3 blocks on an axis, there are 4 verticies on that axis.

    private static int chunkSizeVerts = chunkSizeBlocks + 1;//MarchingCubes3Clean.chunkDim + 1; //max mesh verticies is 65536. 65536/8 (for each cube), and the cube root of that is blocks/chunk, which is MAX 20.

    private float[,,] ogChunkData = new float[chunkSizeBlocks, chunkSizeBlocks, chunkSizeBlocks];

    private float[,,] hollowedChunkData = new float[chunkSizeBlocks, chunkSizeBlocks, chunkSizeBlocks];

    private float threshold = 0.5f;

    public float noiseScale = 0.05f;

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
                        ogChunkData[chunkSizeBlocks - x - 1, chunkSizeBlocks - y - 1, z] = 1;

                    }
                    else
                    {
                        ogChunkData[chunkSizeBlocks - x - 1, chunkSizeBlocks - y - 1, z] = 0;//idk if it instantiates to all 0's
                    }
                }
            }
        }

        hollowedChunkData = hollowOut(ogChunkData);




        CombineInstance[] wholeMesh = new CombineInstance[(int)Mathf.Pow(chunkSizeBlocks, 3)];

        MeshFilter meshFilter = blockPrefab.GetComponent<MeshFilter>(); //instantiates every element in array by default, like C#

        //wholeMesh[0].mesh = meshFilter.sharedMesh;
        wholeMesh[0].transform = meshFilter.transform.localToWorldMatrix;

        int index = 1; //blockNum is a bad idea for indexes, if (x,y,z) is (1,0,0) or (0,1,0), maps to the same index!

        //per chunk
        for (int x = 0; x < chunkSizeBlocks; x++) //or x < chunkSizeBlocks
        {
            for (int y = 0; y < chunkSizeBlocks; y++)
            {
                for (int z = 0; z < chunkSizeBlocks; z++)
                {
                    /*if (x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }*/

                    if ((int)hollowedChunkData[x, y, z] > 0)
                    {
                        //set blockNum somehow
                        //int blockNum = (int)(z + y * (Mathf.Pow(chunkSizeBlocks, 1)) + x * (Mathf.Pow(chunkSizeBlocks, 2)));
                        //percube
                        //if combineInstance has too many verts: //IMPLEMENT THIS LOGIC LATER, MAYBE IF COMBINING CHUNKS???
                        //save the current combineInstance to a mesh
                        //make a new one

                        wholeMesh[index].mesh = meshFilter.sharedMesh;
                        wholeMesh[index].transform = Matrix4x4.TRS(new Vector3(x, y, z), meshFilter.transform.localToWorldMatrix.rotation, new Vector3(1, 1, 1));//meshFilter.transform.localToWorldMatrix; //the missing link

                        index++;
                    }
                }
            }
        }


        MeshFilter thisMesh = transform.GetComponent<MeshFilter>();
        thisMesh.mesh = new Mesh();
        thisMesh.mesh.CombineMeshes(wholeMesh);

        thisMesh.mesh.RecalculateBounds();
        thisMesh.mesh.RecalculateNormals();
        thisMesh.mesh.Optimize();
    }

    float[,,] hollowOut(float[,,] arr) //i think arr is passed as a reference so im pretty sure modifying it in this method as a void would be fine but im being careful. Test this later, making this a void and just modifying arr!!!!!!!!
    {
        List<Vector3> modPoints = new List<Vector3>(); //all the points we change, any point in this list was hollowed so set it to 0
        for (int x = 0; x < chunkSizeBlocks; x++)
        {
            for (int y = 0; y < chunkSizeBlocks; y++)
            {
                for (int z = 0; z < chunkSizeBlocks; z++)
                {
                    int startPosX = (x - 1 < 0) ? x : x - 1;
                    int startPosY = (y - 1 < 0) ? y : y - 1;
                    int startPosZ = (z - 1 < 0) ? z : z - 1;

                    int endPosX = (x + 1 > chunkSizeBlocks - 1) ? x : x + 1;
                    int endPosY = (y + 1 > chunkSizeBlocks - 1) ? y : y + 1;
                    int endPosZ = (z + 1 > chunkSizeBlocks - 1) ? z : z + 1;

                    int numAlive = 0;
                    int numAround = 6;//(endPosX - startPosX) + (endPosY - startPosY) + (endPosZ - startPosZ);

                    if (startPosX == x || startPosY == y || startPosZ == z || endPosX == x || endPosY == y || endPosZ == z) //ignore edge cases entirely
                    {
                        continue;
                    }

                    /*for (int i = startPosX; i < endPosX + 1; i++)
                        if (arr[i, y, z] == 1 && i != x)
                            numAlive++;

                    for (int i = startPosX; i < endPosX + 1; i++)
                        if (arr[x, i, z] == 1 && i != y)
                            numAlive++;

                    for (int i = startPosX; i < endPosX + 1; i++)
                        if (arr[x, y, i] == 1 && i != z)
                            numAlive++;*/

                    if (arr[x + 1, y, z] == 1)
                        numAlive++;
                    if (arr[x - 1, y, z] == 1)
                        numAlive++;
                    if (arr[x, y + 1, z] == 1)
                        numAlive++;
                    if (arr[x, y - 1, z] == 1)
                        numAlive++;
                    if (arr[x, y, z + 1] == 1)
                        numAlive++;
                    if (arr[x, y, z - 1] == 1)
                        numAlive++;

                    if (numAround > 0 && numAlive == numAround)
                        modPoints.Add(new Vector3(x, y, z));
                }
            }
        }

        foreach (Vector3 v in modPoints)
            arr[(int)v.x, (int)v.y, (int)v.z] = 0;

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
