using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    static readonly string textFile = "C:/Users/Mich Ał/Desktop/Magisterka_DZ/ASCII/points.txt";
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    private int xOffset = 577372;
    private int zOffset = 146223;
    private float yOffset = 1386f;


    void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        ReadPointsFromFile();
        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.OrderByDescending(v => v.x).ThenBy(v => v.z).ToArray();
        mesh.triangles = getTriangles();
        mesh.RecalculateNormals();
    }


    int[] getTriangles()
    {

        int vert = 0;
        int tris = 0;
        //1974
        var xSize = (int)((vertices.Max(v => v.x)) - (vertices.Min(v => v.x)));
        //2848
        var zSize = (int)((vertices.Max(v => v.z)) - (vertices.Min(v => v.z)));

        var triangles = new int[xSize * zSize * 6];


        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + zSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + zSize + 1;
                triangles[tris + 5] = vert + zSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        return triangles;
    }

    private void ReadPointsFromFile()
    {
        var reader = new StreamReader(textFile);
        var lines = new List<String>();
        while (!reader.EndOfStream)
        {
            lines.Add(reader.ReadLine());
        }
        reader.Close();

        foreach (var line in lines)
        {
            var values = line.Split(',');
            var y = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
            vertices.Add(new Vector3(
             float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat) - xOffset,
             (y == 0 ? y : y - yOffset),
             float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat) - zOffset)
             );
        }
    }
}