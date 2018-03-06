using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleDisplay : MonoBehaviour {
	public Camera camera;                     // Camera to use for rotation
	public int maxCubes = 1000;
	public int spaceRange = 1000;
	public bool dynamicColor = false;         // Use color defined in particle
	public Material material;                 // Material for particles
    public const int pointsPerCube = 24;
	private const int cubesPerMesh = 65520 / 24;
	public float defaultParticleSize = 1.0f;
	public Color defaultParticleColor = Color.white;
    private const int maxNShow = 55;

    private bool useMid = true;                 // if we dont use mid, we use stockList

    public struct Particle {
		public float angularVelocity;
		public Color color;
		public float lifetime;
		public Vector3 position;
		public float rotation;
		public float size;
		public Vector3 velocity;
	}
    private List<List<StockSample>> stockLists; // a list of stock-time waveforms [stock][time]
    private List<List<float>> stockmidList;    // [time][stock], at each stock, list all stocks mid point


    public Particle[] particles;
	
	private GameObject[] goMeshes;
	private Mesh[] meshes;
	private bool isRunning = false;
	
	private Transform camTrans;
    private int[] Counts;   // size of each stock
	void Awake() {
		if (camera == null) 
			camera = Camera.main;
		
		if (camera != null)
			camTrans = camera.transform;
		
		CreateMeshes();
	}
	
    private void ModifyStockListMeshes(int which) {
        int many = goMeshes.Length;
        for (int stocki = 0; stocki < many; stocki++)    // i iterates through all stocks
        {
            var samples = stockLists[stocki];
            if (which < Counts[stocki]) {
                var vertices = meshes[stocki].vertices;// new Vector3[24];
                var triangles = meshes[stocki].triangles;// new int[36];
                var normals = meshes[stocki].normals;// new Vector3[24];
                var uv = meshes[stocki].uv;// new Vector2[24];

                float init = samples[0].open;
                float open = samples[which].open;
                float y_return = (open - init) * 10000 / init; // in bps 
                MeshCube.AddCube(ref vertices, ref triangles, ref normals, ref uv, which,
                    new Vector3(which * spaceRange, y_return, stocki * spaceRange * 10), defaultParticleSize);
                /*
                for (int j = 0; j < 24; j++)  {
                    meshes[stocki].vertices[which * 24 + j] = vertices[j];
                    meshes[stocki].normals[which * 24 + j] = normals[j];
                    meshes[stocki].uv[which * 24 + j] = uv[j];
                }
                for (int j = 0; j < 36; j++)  {
                    meshes[stocki].triangles[which*36+j] = triangles[j];
                }
                */
                meshes[stocki].vertices = vertices;
                meshes[stocki].triangles = triangles;
                meshes[stocki].normals = normals;

                meshes[stocki].uv = uv;

                meshes[stocki].RecalculateBounds();
                meshes[stocki].Optimize();
            }

        }
    }
    // now use time as input variable, to change one mesh at this time
    private void ModifyMidMeshes(int which) {
        if (which>=stockmidList.Count)
            return;
        
        List<float> allMid = stockmidList[which];
        List<float> openMid = stockmidList[0];
        int many = allMid.Count;

        for (int stocki = 0; stocki < many; stocki++)    // iterates through all stocks
        {
            if (which < stockmidList.Count) {
                var vertices = meshes[which].vertices;// new Vector3[24];
                var triangles = meshes[which].triangles;// new int[36];
                var normals = meshes[which].normals;// new Vector3[24];
                var uv = meshes[which].uv;// new Vector2[24];

                float y_return = ( allMid[stocki] -  openMid[stocki] ) * 10000 / openMid[stocki]; // in bps 
                MeshCube.AddCube(ref vertices, ref triangles, ref normals, ref uv, stocki, //which
                    new Vector3(which * spaceRange, y_return, stocki * spaceRange * 10), defaultParticleSize);
                /*
                for (int j = 0; j < 24; j++)  {
                    meshes[stocki].vertices[which * 24 + j] = vertices[j];
                    meshes[stocki].normals[which * 24 + j] = normals[j];
                    meshes[stocki].uv[which * 24 + j] = uv[j];
                }
                for (int j = 0; j < 36; j++)  {
                    meshes[stocki].triangles[which*36+j] = triangles[j];
                }
                */
                meshes[which].vertices = vertices;
                meshes[which].triangles = triangles;
                meshes[which].normals = normals;

                meshes[which].uv = uv;

                meshes[which].RecalculateBounds();
                meshes[which].Optimize();
            }

        }
    }

    public void ModifyMeshes(int which)
    {
        if (useMid) 
            ModifyMidMeshes(which);
        else
            ModifyStockListMeshes(which);
    }

    void CreateStockListMeshes() {
        stockLists = MarketDataParser.LoadStocks();
        int maxSamples = 0;
        foreach (var list in stockLists) {
            if (maxSamples < list.Count)
                maxSamples = list.Count;
        }

        particles = new Particle[maxSamples];
        for (int i = 0; i < particles.Length; i++) {
            particles[i].size = defaultParticleSize;
            particles[i].color = defaultParticleColor;
        }

        int stocksToRender = stockLists.Count;
        print("stockCount=" + stockLists.Count + ", cubersPerMesh=" + cubesPerMesh);

        goMeshes = new GameObject[stockLists.Count];
        meshes = new Mesh[stockLists.Count];

        GaussianRandom gaussianRandom = new GaussianRandom();

        List<Color> mesh_colors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow };
        int color_choices = mesh_colors.Count;
        Counts = new int[stockLists.Count];
        for (int i = 0; i < stockLists.Count; i++) {
            GameObject go = new GameObject("Mesh " + i);
            go.transform.parent = transform;
            MeshFilter mf = go.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mf.mesh = mesh;
            Renderer rend = go.AddComponent<MeshRenderer>();
            rend.material = material;
            rend.material.color = mesh_colors[i % color_choices];

            var vertices = new Vector3[24 * cubesPerMesh];
            var triangles = new int[36 * cubesPerMesh];
            var normals = new Vector3[24 * cubesPerMesh];
            var colors = new Color[24 * cubesPerMesh];
            int vertex_group = 10 * 24;
            for (int c = 0; c < colors.Length / vertex_group; ++c) {
                var a_color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                for (int k = 0; k < vertex_group; ++k)
                    colors[vertex_group * c + k] = a_color;
            }
            var uv = new Vector2[24 * cubesPerMesh];

            float x_time = 0, y_return = 0;
            // TODO: cubesPerMesh might be smaller than the sample count 
            var samples = stockLists[i];
            Counts[i] = samples.Count;
            for (int j = 0; j < cubesPerMesh && j < samples.Count; j++) {
                // x=> screen x
                // y=> screen y
                // z=> depth 

                //x = (float)(gaussianRandom.NextGaussian() * spaceRange);      
                //z = (float)(gaussianRandom.NextGaussian() * spaceRange);
                //y = (float)(0.1*x + 0.2*z + gaussianRandom.NextGaussian() * 0.5 * spaceRange ); 

                x_time += spaceRange;
                float init = samples[0].open;
                float open = samples[j].open;
                y_return = (open - init) * 10000 / init; // in bps 

                print("Adding x_time=" + x_time
                    + ", y_price=" + y_return
                    + ", z_stock" + i);
                MeshCube.AddCube(ref vertices, ref triangles, ref normals, ref uv, j,
                    new Vector3(x_time, 0, i * spaceRange * 10), defaultParticleSize / 2);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.colors = colors;
            mesh.uv = uv;
            mesh.RecalculateBounds();
            mesh.Optimize();
            for (int k = 0; k < 5; k++) {
                print("k=" + k);
                print("vertices: " + mesh.vertices[k]);
                print("triangles: " + mesh.triangles[k]);
                print("normals: " + mesh.normals[k]);
                print("uv: " + mesh.uv[k]);
                print("colors: " + mesh.colors[k]);
            }

            goMeshes[i] = go;
            meshes[i] = mesh;
        }

        print("Meshes created");
    }

    void CreateMidMeshes() {
        stockmidList = MarketDataParser.LoadStocksMid();
        int maxSamples = stockmidList.Count;
        float x1 = stockmidList[0][0];
        float y1 = stockmidList[0][1];
        float x2 = stockmidList[1][0];
        float y2 = stockmidList[1][1];
        particles = new Particle[maxSamples];
        for (int i = 0; i < particles.Length; i++) {
            particles[i].size = defaultParticleSize;
            particles[i].color = defaultParticleColor;
        }

        int stocksToRender = System.Math.Min(stockmidList[0].Count, maxNShow);
        print("stockCount=" + stocksToRender + ", cubersPerMesh=" + cubesPerMesh);


        int NMeshes=100;
        goMeshes = new GameObject[NMeshes]; // maxSamples//[stocksToRender];
        meshes = new Mesh[NMeshes];// stocksToRender];

        GaussianRandom gaussianRandom = new GaussianRandom();

        List<Color> mesh_colors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow };
        int color_choices = mesh_colors.Count;
        Counts = new int[stocksToRender];
        for (int t = 0; t < cubesPerMesh && t < maxSamples; t++) {
        //
            GameObject go = new GameObject("Mesh " + t);
            go.transform.parent = transform;
            MeshFilter mf = go.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mf.mesh = mesh;
            Renderer rend = go.AddComponent<MeshRenderer>();
            rend.material = material;
            rend.material.color = mesh_colors[t % color_choices];

            var vertices = new Vector3[24 * cubesPerMesh];
            var triangles = new int[36 * cubesPerMesh];
            var normals = new Vector3[24 * cubesPerMesh];
            var colors = new Color[24 * cubesPerMesh];
            int vertex_group = 10 * 24;
            for (int c = 0; c < colors.Length / vertex_group; ++c) {
                var a_color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                for (int k = 0; k < vertex_group; ++k)
                    colors[vertex_group * c + k] = a_color;
            }
            var uv = new Vector2[24 * cubesPerMesh];

            float x_time = 0, y_return = 0;
            // TODO: cubesPerMesh might be smaller than the sample count 
            //for (int t = 0; t < cubesPerMesh && t < maxSamples; t++) {
            for (int iStock = 0; iStock < stocksToRender; iStock++) {
                // x=> screen x
                // y=> screen y
                // z=> depth 

                //x = (float)(gaussianRandom.NextGaussian() * spaceRange);      
                //z = (float)(gaussianRandom.NextGaussian() * spaceRange);
                //y = (float)(0.1*x + 0.2*z + gaussianRandom.NextGaussian() * 0.5 * spaceRange ); 

                x_time += spaceRange;
   
                print("Adding x_time=" + x_time
                    + ", y_price=" + y_return
                    + ", z_stock" + iStock);
                MeshCube.AddCube(ref vertices, ref triangles, ref normals, ref uv, t,
                    new Vector3(x_time, 0, iStock * spaceRange * 10), defaultParticleSize / 2);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.colors = colors;
            mesh.uv = uv;
            mesh.RecalculateBounds();
            mesh.Optimize();
            for (int k = 0; k < 5; k++) {
                print("k=" + k);
                print("vertices: " + mesh.vertices[k]);
                print("triangles: " + mesh.triangles[k]);
                print("normals: " + mesh.normals[k]);
                print("uv: " + mesh.uv[k]);
                print("colors: " + mesh.colors[k]);
            }

            goMeshes[t] = go;
            meshes[t] = mesh;
        }

        print("Meshes created");

    }


	void CreateMeshes() {
        if (useMid)
            CreateMidMeshes();
        else
            CreateStockListMeshes();
	}
	
	public void Clear () {
		print ("Clearing");
		for (int i = 0; i < particles.Length; i++) 
			particles[i].lifetime = -1.0f;
	}
	
	public void Play() {
		print ("Playing");
		isRunning = true;
		foreach (GameObject go in goMeshes) go.GetComponent<Renderer>().enabled = true;
	}
	
	public void Stop () {
		print ("Stopping");
		isRunning = false;
		foreach (GameObject go in goMeshes) go.GetComponent<Renderer>().enabled = false;
	}
	
	void LateUpdate() {
	}
}

