using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
public class MeshEditor : Editor {
	private MeshGenerator meshGenerator;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if (GUILayout.Button("Generate Mesh")) {
			meshGenerator.ConstructMesh();
		}

		var numIterationsString = meshGenerator.numErosionIterations.ToString();
		if (meshGenerator.numErosionIterations >= 1000) {
			numIterationsString = (meshGenerator.numErosionIterations / 1000) + "k";
		}

		if (GUILayout.Button("Erode (" + numIterationsString + " iterations)")) {
			var sw = new System.Diagnostics.Stopwatch();

			sw.Start();
			meshGenerator.Erode();
			var erosionTimer = (int)sw.ElapsedMilliseconds;
			sw.Reset();

			sw.Start();
			meshGenerator.ConstructMesh();
			var meshTimer = (int)sw.ElapsedMilliseconds;

			if (!meshGenerator.printTimers) return;
			Debug.Log($"{meshGenerator.mapSizeX}x{meshGenerator.mapSizeZ} heightmap generated");
			Debug.Log($"{numIterationsString} erosion iterations completed in {erosionTimer}ms");
			Debug.Log($"Mesh constructed in {meshTimer}ms");
		}
	}

	void OnEnable() {
		meshGenerator = (MeshGenerator)target;
		Tools.hidden = true;
	}

	void OnDisable() {
		Tools.hidden = false;
	}
}