using System;
using UnityEngine;
using System.Collections;

public abstract class MeshShape {
    public const int NVertices = 0;
    public const int NNormals = 1;
    public const int NUVs = 2;
    public const int NTriangles = 3;

    abstract public int[] getSize();
    /// <summary>
    /// vertices, triangles, normals, uvs, objectIndex, offset, size
    /// </summary>
    /// <returns></returns>
    abstract public void AddShape(ref Vector3[] mesh_vertices, ref int[] mesh_triangles, ref Vector3[] mesh_normals, ref Vector2[] mesh_uv,
                               int objIndex, Vector3 positionOffset, float size);
    /// <summary>
    /// NVertices, NNormals, NUVs, NTriangles
    /// </summary>
    /// <returns></returns>
   // abstract public Tuple<int, int, int, int> getSizes();
}

