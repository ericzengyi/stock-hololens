using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MydataType = UnityEngine.Vector4;

public class ParticleDisplay : MonoBehaviour {
    private string logname = "testlog.txt";
    public Camera camera;                     // Camera to use for rotation
    public int maxCubes = 1000;
    public int spaceRange = 1000;
    public bool dynamicColor = false;         // Use color defined in particle
    public Material material;                 // Material for particles
    public const int pointsPerCube = 24;
    private int numOfStocks = 55;//65520 / 24;
    public float defaultParticleSize = 1.0f;
    public Color defaultParticleColor = Color.white;

    public MeshShape shape = new MeshSphere();

    private void log(string fun, string content) {
        //System.IO.File.AppendAllText(logname, fun + "," + content + "\r\n");
        
    }

    public struct Particle {
        public float angularVelocity;
        public Color color;
        public float lifetime;
        public Vector3 position;
        public float rotation;
        public float size;
        public Vector3 velocity;
    }

    public Particle[] particles;
    public Shader shaders;
    private GameObject[] goMeshes;

    private Transform camTrans;
    //private Transform oldTrans;
    //private int[] Counts;   // size of each stock
    void Awake() {
        if (camera == null)
            camera = Camera.main;

        if (camera != null)
            camTrans = camera.transform;

        CreateMeshes();
       // oldTrans = transform;
    }
    public Vector3 GetVertexWorldPosition(Vector3 vertex, Transform owner) {
        return owner.localToWorldMatrix.MultiplyPoint3x4(vertex);
    } 
    
    bool first = true;
    int frameCount = 0;
    int notReadyCount = 0;
    GameObject go = null;
    Mesh mesh=null;
    private bool move2Next = false; // if true, means we need to restart timer

    public void UpdateMeshes(int nth_snapshot) {
        // list3DS = MarketDataParser.runningList;
        List<MydataType> all3DS = MarketDataParser.runningList;
        int max_number_of_cubes = System.Math.Min(all3DS.Count, numOfStocks);//stockmidList[0].Coun
        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var normals = mesh.normals;
        var colors = mesh.colors;
        var uv = mesh.uv;
        int[] sizes = shape.getSize();

        //                    var colors = new Color32[sizes.Item1 * numOfStocks];
        int vertex_group = sizes[MeshShape.NVertices];// int vertex_group = MeshCube.NCubeVertices;
        List<Color> allColor = MarketDataParser.runningColorList;
        for (int k = 0; k < numOfStocks; ++k) {
            for (int j = 0; j < vertex_group; j++) {
                colors[k * vertex_group + j] = MarketDataParser.runningColorList[nth_snapshot * numOfStocks + k];
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.RecalculateBounds();
        ;
    }

    void CreateMeshes() {
            MarketDataParser.Load3ds();
            List<MydataType> all3DS = MarketDataParser.runningList;
            numOfStocks = MarketDataParser.NStocks;

            int stocksToRender = System.Math.Min(MarketDataParser.NStocks, numOfStocks);
            print("stockCount=" + stocksToRender + ", cubersPerMesh=" + numOfStocks);
            log("newcreatemid", "cubsPerMesh=" + numOfStocks);
            //Tuple<int, int, int, int> sizes = shape.getSizes();
            int[] sizes = shape.getSize();
            log("CreateMeshes", "V:" + sizes[MeshShape.NVertices]+" N:" + sizes[MeshShape.NNormals]
                + " U:" + sizes[MeshShape.NUVs] + " T:" + sizes[MeshShape.NTriangles]+","+sizes[0]+sizes[1]+sizes[3]+sizes[3]);
                

            var colors = new Color32[sizes[MeshShape.NVertices] * numOfStocks];
            int vertex_group = sizes[MeshShape.NVertices];
            List<Color> allColor = MarketDataParser.runningColorList;
            for (int k = 0; k < numOfStocks; ++k) {
                for (int j = 0; j < vertex_group; j++) {
                    colors[k * vertex_group + j] = allColor[k];
                }
            }

        go = new GameObject("Mesh 1");//GameObject.CreatePrimitive(PrimitiveType.Cube); //
        go.transform.parent = transform;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mesh = go.GetComponent<MeshFilter>().mesh;

        Renderer rend = go.AddComponent<MeshRenderer>();
        log("NewCreate", "before shader find");
        rend.material = material;
        log("NewCreate", "post shader find");


            mesh.vertices   = new Vector3[ sizes[MeshShape.NVertices] * numOfStocks];
            mesh.normals    = new Vector3[ sizes[MeshShape.NNormals] * numOfStocks];
            mesh.uv         = new Vector2[ sizes[MeshShape.NUVs] * numOfStocks];
            mesh.triangles  = new int[ sizes[MeshShape.NTriangles] * numOfStocks];

            log("Sizes", "V:" + mesh.vertices.Length + "N:" + mesh.normals.Length + "U:" + mesh.uv.Length + "T:" + mesh.triangles.Length);

            mesh.MarkDynamic();

        var vertices = mesh.vertices;
        var triangles = mesh.triangles;
        var normals = mesh.normals;
        var uv = mesh.uv;

        for (int stocki = 0; stocki < numOfStocks; stocki++) {
            Vector3 newLocation = new Vector3(
				// No sizing here, all sizing controlled by matlab codes 
                all3DS[stocki].x * 1,
                all3DS[stocki].y * 1,
                all3DS[stocki].z * 1);

            Vector3 worldPt = transform.TransformPoint(newLocation);

                shape.AddShape(ref vertices, ref triangles, ref normals, ref uv, stocki, //which
                                        newLocation,//worldPt,// GetVertexWorldPosition(newLocation, transform), 
                                        defaultParticleSize * all3DS[stocki].w);
                //log("create", "done with " + stocki);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.colors32 = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        ;
        print("Meshes created");
    }
	
	public void Clear () {
		print ("Clearing");
		for (int i = 0; i < particles.Length; i++) 
			particles[i].lifetime = -1.0f;
	}
	
	public void Play() {
		print ("Playing");
		//foreach (GameObject go in goMeshes) go.GetComponent<Renderer>().enabled = true;
	}
	
	public void Stop () {
		print ("Stopping");
		//foreach (GameObject go in goMeshes) go.GetComponent<Renderer>().enabled = false;
	}
	
	void LateUpdate() {
	}
}

