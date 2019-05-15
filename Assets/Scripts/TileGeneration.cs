using UnityEngine;

public class TileGeneration : MonoBehaviour {

	NoiseMapGeneration m_NoiseMapGeneration;
	MeshRenderer m_Renderer;
	MeshFilter m_MeshFilter;
	MeshCollider m_Collider;

	[SerializeField] float MapScale;

	[SerializeField] TerrainType[] m_TerrainTypes;
	[SerializeField] float heightMultiplier;
	[SerializeField] AnimationCurve animationCurve;
	[SerializeField] Wave[] waves;

	private void Awake() {
		m_NoiseMapGeneration = new NoiseMapGeneration();
		m_Renderer = GetComponent<MeshRenderer>();
		m_MeshFilter = GetComponent<MeshFilter>();
		m_Collider = GetComponent<MeshCollider>();
	}

	private void Start() {
		GenerateTile();
	}

	[ContextMenu("Generate New map")]
	public void GenerateTile() {
		Vector3[] meshVertices = m_MeshFilter.mesh.vertices;
		int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
		int tileWidth = tileDepth;

		m_NoiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, MapScale, -transform.position.x, -transform.position.z, waves, noiseMap => {
			float[,] heightMap = noiseMap;
			Texture2D tex = BuildTexture(heightMap);
			m_Renderer.material.mainTexture = tex;
			ChangeTerrainHeight(heightMap);
		});
	}

	public void ChangeTerrainHeight(float[,] heightMap) {
		int tileDepth = heightMap.GetLength(0);
		int tileWidth = heightMap.GetLength(1);

		Vector3[] meshVertices = m_MeshFilter.mesh.vertices;

		int vertexIndex = 0;
		for (int z = 0; z < tileDepth; z++) {
			for (int x = 0; x < tileWidth; x++) {
				float height = heightMap[z, x];

				Vector3 vertex = meshVertices[vertexIndex];
				meshVertices[vertexIndex] = new Vector3(vertex.x, animationCurve.Evaluate(height) * heightMultiplier, vertex.z);
				vertexIndex++;
			}
		}

		m_MeshFilter.mesh.vertices = meshVertices;
		m_MeshFilter.mesh.RecalculateBounds();
		m_MeshFilter.mesh.RecalculateNormals();
		m_Collider.sharedMesh = m_MeshFilter.mesh;
	}

	private Texture2D BuildTexture(float[,] noiseMap) {
		int tileDepth = noiseMap.GetLength(0);
		int tileWidth = noiseMap.GetLength(1);

		Color[] colorMap = new Color[tileDepth * tileWidth];

		for (int z = 0; z < tileDepth; z++) {
			for (int x = 0; x < tileWidth; x++) {
				int colorIndex = z * tileWidth + x;
				float height = noiseMap[z, x];

				colorMap[colorIndex] = ChooseTerrainType(height).color;
			}
		}
		Texture2D tex = new Texture2D(tileDepth, tileWidth);
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.SetPixels(colorMap);
		tex.Apply();

		return tex;
	}

	public TerrainType ChooseTerrainType(float height) {
		foreach (var type in m_TerrainTypes) {
			if (height < type.Height) {
				return type;
			}
		}
		return m_TerrainTypes[m_TerrainTypes.Length - 1];
	}
}
