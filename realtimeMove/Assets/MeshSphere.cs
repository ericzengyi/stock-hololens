using UnityEngine;
using System;
using System.Collections.Generic;

public class MeshSphere: MeshShape {
    private void log(string fun, string content) {
        System.IO.File.AppendAllText("Shape.txt", fun + "," + content + "\r\n");

    }
    private const int nbLong = 12;//24;  // Longitude |||
    private const int nbLat = 8;//16;   // Latitude ---

    private const int NSphereVertices = (nbLong + 1) * nbLat + 2;
    private const int NSphereNormals = NSphereVertices;
    private const int NSphereUVs = NSphereVertices;
    private const int nbFaces = NSphereVertices;
    private const int nbTriangles = nbFaces * 2;
    private const int nbIndexes = nbTriangles * 3;
    private const int NSphereTriangles = nbIndexes;
    public override int[] getSize() {
        return new int[] {NSphereVertices, NSphereNormals, NSphereUVs, NSphereTriangles};
    }
    //public override Tuple<int, int, int, int> getSizes() {return new Tuple<int, int, int, int>(NSphereVertices, NSphereNormals, NSphereUVs, NSphereTriangles);    }
    public override void AddShape(ref Vector3[] mesh_vertices, ref int[] mesh_triangles, ref Vector3[] mesh_normals, ref Vector2[] mesh_uv,
                               int objIndex, Vector3 positionOffset, float size) {// change to sphere
        float radius = size;
        #region Vertices
        Vector3[] vertices = new Vector3[NSphereVertices];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        vertices[0] = Vector3.up * radius;
        for (int lat = 0; lat < nbLat; lat++) {
            float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= nbLong; lon++) {
                float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
            }
        }
        vertices[vertices.Length - 1] = Vector3.up * -radius;
        #endregion


        #region normals
        Vector3[] normals = new Vector3[NSphereNormals];// vertices.Length]; shoudl be same?

        for (int n = 0; n < vertices.Length; n++)
            normals[n] = vertices[n].normalized;
        #endregion

        #region UV
        Vector2[] uvs = new Vector2[NSphereUVs];// vertices.Length];

        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;
        for (int lat = 0; lat < nbLat; lat++)
            for (int lon = 0; lon <= nbLong; lon++)
                uvs[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
        #endregion

        #region Triangles

        int[] triangles = new int[NSphereTriangles];// nbIndexes];

        //Top Cap
        { 

            int i = 0;
            for (int lon = 0; lon < nbLong; lon++) {
                triangles[i++] = lon + 2;
                triangles[i++] = lon + 1;
                triangles[i++] = 0;
            }

            //Middle
            for (int lat = 0; lat < nbLat - 1; lat++) {
                for (int lon = 0; lon < nbLong; lon++) {
                    int current = lon + lat * (nbLong + 1) + 1;
                    int next = current + nbLong + 1;

                    triangles[i++] = current;
                    triangles[i++] = current + 1;
                    triangles[i++] = next + 1;

                    triangles[i++] = current;
                    triangles[i++] = next + 1;
                    triangles[i++] = next;
                }
            }

            //Bottom Cap
            for (int lon = 0; lon < nbLong; lon++) {
                triangles[i++] = vertices.Length - 1;
                triangles[i++] = vertices.Length - (lon + 2) - 1;
                triangles[i++] = vertices.Length - (lon + 1) - 1;
            }
        }
        #endregion

        //mesh_vertices = vertices; // 24 per cube 
        //mesh_normals = normals; // 24 per cube 
        //mesh_uv = uv; // 24 per cube 
        //mesh_triangles = triangles; // 36 per cube 
        
        try {
            for (int i = 0; i < NSphereVertices; i++) {
                mesh_vertices[objIndex * NSphereVertices + i] = vertices[i] + positionOffset;
            }
        } catch (IndexOutOfRangeException e) {

            string lineNumber = e.StackTrace.Substring(e.StackTrace.Length - 7, 7);
            log("Copy", e.Message.ToString() + ":" + e.StackTrace);
        }
        
        for (int i = 0; i < NSphereNormals; i++) {
                mesh_normals[objIndex * NSphereNormals + i] = normals[i];
            }
        
        for (int i = 0; i < NSphereUVs; i++) {
                mesh_uv[objIndex * NSphereUVs + i] = uvs[i];
            }
        
        for (int i = 0; i < NSphereTriangles; i++) {
                mesh_triangles[objIndex * NSphereTriangles + i] = objIndex * NSphereVertices + triangles[i];
            }
            

        //mesh_RecalculateBounds();
        //mesh_Optimize();

    }
}

