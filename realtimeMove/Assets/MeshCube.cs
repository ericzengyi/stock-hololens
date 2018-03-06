using UnityEngine;
using System;
using System.Collections;

public class MeshCube:MeshShape {

    private const int NCubeVertices = 24;
    private const int NCubeNormals = 24;
    private const int NCubeUVs = 24;
    private const int NCubeTriangles = 36;

    public override int[] getSize() {
        return new int[] {NCubeVertices, NCubeNormals, NCubeUVs, NCubeTriangles };
    }
    //public override Tuple<int, int, int, int> getSizes() {return new Tuple<int, int, int, int>(NCubeVertices, NCubeNormals, NCubeUVs, NCubeTriangles);  }
    // This sourced from http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_Box
    public override void AddShape( ref Vector3[] mesh_vertices, ref int[] mesh_triangles, ref Vector3[] mesh_normals, ref Vector2[] mesh_uv, 
	                           int cubeIndex, Vector3 positionOffset, float size ) {
	// You can change that line to provide another MeshFilter
	//MeshFilter filter = gameObject.AddComponent< MeshFilter >();
	//Mesh mesh = filter.mesh;
	//mesh_Clear();
	 
	float length = size;
	float width = size;
	float height = size;
	 
	#region Vertices
	Vector3 p0 = new Vector3( -length * .5f,	-width * .5f, height * .5f ) + positionOffset; 
	Vector3 p1 = new Vector3( length * .5f, 	-width * .5f, height * .5f ) + positionOffset;
	Vector3 p2 = new Vector3( length * .5f, 	-width * .5f, -height * .5f ) + positionOffset;
	Vector3 p3 = new Vector3( -length * .5f,	-width * .5f, -height * .5f ) + positionOffset;	
 
	Vector3 p4 = new Vector3( -length * .5f,	width * .5f,  height * .5f ) + positionOffset;
	Vector3 p5 = new Vector3( length * .5f, 	width * .5f,  height * .5f ) + positionOffset;
	Vector3 p6 = new Vector3( length * .5f, 	width * .5f,  -height * .5f ) + positionOffset;
	Vector3 p7 = new Vector3( -length * .5f,	width * .5f,  -height * .5f ) + positionOffset;

	Vector3[] vertices = new Vector3[]
	{
		// Bottom
		p0, p1, p2, p3,
		// Left
		p7, p4, p0, p3,
		// Front
		p4, p5, p1, p0,
		// Back
		p6, p7, p3, p2,
		// Right
		p5, p6, p2, p1,
		// Top
		p7, p6, p5, p4
	};
	#endregion
	 
	#region normals
	Vector3 up 	= Vector3.up;
	Vector3 down 	= Vector3.down;
	Vector3 front 	= Vector3.forward;
	Vector3 back 	= Vector3.back;
	Vector3 left 	= Vector3.left;
	Vector3 right 	= Vector3.right;
	Vector3[] normals = new Vector3[]
	{
		// Bottom
		down, down, down, down,
		// Left
		left, left, left, left,
		// Front
		front, front, front, front,
		// Back
		back, back, back, back,
		// Right
		right, right, right, right,
		// Top
		up, up, up, up
	};
	#endregion	
	 
	#region UV
	Vector2 _00 = new Vector2( 0f, 0f );
	Vector2 _10 = new Vector2( 1f, 0f );
	Vector2 _01 = new Vector2( 0f, 1f );
	Vector2 _11 = new Vector2( 1f, 1f );
	Vector2[] uv = new Vector2[]
	{
		// Bottom
		_11, _01, _00, _10,
		// Left
		_11, _01, _00, _10,
		// Front
		_11, _01, _00, _10,
		// Back
		_11, _01, _00, _10,
		// Right
		_11, _01, _00, _10,
		// Top
		_11, _01, _00, _10,
	};
	#endregion
	 
	#region Triangles
	int[] triangles = new int[]
	{
		// Bottom
		3, 1, 0,
		3, 2, 1,			
		// Left
		3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
		3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
		// Front
		3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
		3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
		// Back
		3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
		3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
		// Right
		3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
		3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
		// Top
		3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
		3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
	};
	#endregion
	 
	//mesh_vertices = vertices; // 24 per cube 
	//mesh_normals = normals; // 24 per cube 
	//mesh_uv = uv; // 24 per cube 
	//mesh_triangles = triangles; // 36 per cube 

	for( int i=0; i < NCubeVertices; i++ ) {
			mesh_vertices[ cubeIndex * NCubeVertices + i ] = vertices[i];
	}

	for( int i=0; i < NCubeNormals; i++ ) {
		mesh_normals[ cubeIndex * NCubeNormals + i ] = normals[i];
	}
	for( int i=0; i < NCubeUVs; i++ ) {
		mesh_uv[ cubeIndex * NCubeUVs + i ] = uv[i];
	}
	for( int i=0; i < NCubeTriangles; i++ ) {
		mesh_triangles[ cubeIndex * NCubeTriangles + i ] = cubeIndex *NCubeVertices + triangles[i];
	}
	//mesh_RecalculateBounds();
	//mesh_Optimize();
} // end of addCube function

}