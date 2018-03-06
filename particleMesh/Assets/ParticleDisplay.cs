using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleDisplay : MonoBehaviour {
	public Camera camera;                     // Camera to use for rotation
	public int maxCubes = 1000;
	public int spaceRange = 1000;
	public bool dynamicColor = false;         // Use color defined in particle
	public Material material;                 // Material for particles
	private const int cubesPerMesh = 65520 / 24;
	public float defaultParticleSize = 1.0f;
	public Color defaultParticleColor = Color.white;
	
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
	
	private GameObject[] goMeshes;
	private Mesh[] meshes;
	private bool isRunning = false;
	
	private Transform camTrans;
	
	void Awake() {
		if (camera == null) 
			camera = Camera.main;
		
		if (camera != null)
			camTrans = camera.transform;
		
		CreateMeshes();
	}
	
	void CreateMeshes() {
        List<List<StockSample>> stockLists = MarketDataParser.LoadStocks();
        int maxSamples = 0;
        foreach (var list in stockLists)
        {
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

		GaussianRandom gaussianRandom = new GaussianRandom ();

        List<Color> mesh_colors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow };
        int color_choices = mesh_colors.Count;
		
		for (int i = 0; i < stockLists.Count; i++) {
			GameObject go = new GameObject( "Mesh " + i );
			go.transform.parent = transform;
			MeshFilter mf = go.AddComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			mesh.MarkDynamic ();
			mf.mesh = mesh;
			Renderer rend = go.AddComponent<MeshRenderer>();
			rend.material = material;
            rend.material.color = mesh_colors[i % color_choices];

            var vertices = new Vector3[24 * cubesPerMesh];
			var triangles = new int[36 * cubesPerMesh];
			var normals = new Vector3[24 * cubesPerMesh];
			var colors = new Color[24 * cubesPerMesh];
			int vertex_group = 10 * 24;
			for(int c=0; c < colors.Length / vertex_group; ++ c) { 
				var a_color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f));
				for(int k=0; k<vertex_group; ++ k)
					colors[vertex_group * c + k] = a_color;
			}
			var uv = new Vector2[24 * cubesPerMesh];

            float x_time=0, y_return=0;
            // TODO: cubesPerMesh might be smaller than the sample count 
            var samples = stockLists[i];
            for (int j=0; j<cubesPerMesh && j<samples.Count; j++) {
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
    			MeshCube.AddCube (ref vertices, ref triangles, ref normals, ref uv, j, 
                    new Vector3 (x_time, y_return, i*spaceRange*10), defaultParticleSize);
			}

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.normals = normals;
			mesh.colors = colors;
			mesh.uv = uv; 
			mesh.RecalculateBounds();
			mesh.Optimize();
			for( int k=0; k < 5; k++ ){ 
				print ( "k=" + k ); 
				print ( "vertices: " + mesh.vertices[k] );
				print ( "triangles: " + mesh.triangles[k] );
				print ( "normals: " + mesh.normals[k] );
				print ( "uv: " + mesh.uv[k] );
				print ("colors: " + mesh.colors[k]);
			}

			goMeshes[i] = go;
			meshes[i] = mesh;
		}

		print ("Meshes created");
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

