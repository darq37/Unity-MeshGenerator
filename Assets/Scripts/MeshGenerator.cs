using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {
	static readonly string textFile = "./Assets/Resources/points.txt";
	Mesh mesh;
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;

	private List<Vector3> _vertices = new List<Vector3>();
	private int xOffset = 577372; // najmniejsza wspolrzędna x
	private int zOffset = 146223; // najmniejsza wspolrzędna y
	private float yOffset = 1386f; // najnizszy punkt na mapie

	[Header("Mesh Settings")]
	public Material material;
	public float elevationScale = 1;


	public void ConstructMesh() {
		ReadPointsFromFile(textFile);
		if (mesh == null) {
			mesh = new Mesh();
		}
		else {
			mesh.Clear();
		}

		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = _vertices.OrderByDescending(v => v.x).ThenBy(v => v.z).ToArray();
		mesh.triangles = getTriangles();
		mesh.RecalculateNormals();

		AssignMeshComponents();
		meshFilter.sharedMesh = mesh;
		meshRenderer.sharedMaterial = material;

		material.SetFloat("_MaxHeight", elevationScale);
	}


	int[] getTriangles() {
		int vert = 0;
		int tris = 0;
		//1974
		var mapSizeX = (int)((_vertices.Max(v => v.x)) - (_vertices.Min(v => v.x)));
		//2848
		var mapSizeZ = (int)((_vertices.Max(v => v.z)) - (_vertices.Min(v => v.z)));

		var triangles = new int[mapSizeX * mapSizeZ * 6];


		for (var x = 0; x < mapSizeX; x++) {
			for (var z = 0; z < mapSizeZ; z++) {
				triangles[tris + 0] = vert + 0;
				triangles[tris + 1] = vert + mapSizeZ + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + mapSizeZ + 1;
				triangles[tris + 5] = vert + mapSizeZ + 2;
				vert++;
				tris += 6;
			}

			vert++;
		}

		return triangles;
	}

	private void ReadPointsFromFile(string path) {
		var reader = new StreamReader(path);
		var lines = new List<String>();
		while (!reader.EndOfStream) {
			lines.Add(reader.ReadLine());
		}

		reader.Close();

		foreach (var line in lines) {
			var values = line.Split(',');
			var y = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
			_vertices.Add(new Vector3(
				float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat) - xOffset,
				(y == 0 ? y : y - yOffset),
				float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat) - zOffset)
			);
		}
	}

	private void AssignMeshComponents() {
		// Find/creator mesh holder object in children
		string meshHolderName = "Mesh Holder";
		Transform meshHolder = transform.Find(meshHolderName);
		if (meshHolder == null) {
			meshHolder = new GameObject(meshHolderName).transform;
			meshHolder.transform.parent = transform;
			meshHolder.transform.localPosition = Vector3.zero;
			meshHolder.transform.localRotation = Quaternion.identity;
		}

		// Ensure mesh renderer and filter components are assigned
		if (!meshHolder.gameObject.GetComponent<MeshFilter>()) {
			meshHolder.gameObject.AddComponent<MeshFilter>();
		}

		if (!meshHolder.GetComponent<MeshRenderer>()) {
			meshHolder.gameObject.AddComponent<MeshRenderer>();
		}

		meshRenderer = meshHolder.GetComponent<MeshRenderer>();
		meshFilter = meshHolder.GetComponent<MeshFilter>();
	}
}