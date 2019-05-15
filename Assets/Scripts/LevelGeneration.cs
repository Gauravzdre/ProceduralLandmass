using UnityEngine;

public class LevelGeneration : MonoBehaviour {

	[SerializeField] GameObject tilePrefab;
	[SerializeField] int mapWidthInTiles, mapDepthInTiles;

	private void Start() {
		GenerateTiles();
	}

	public void GenerateTiles() {
		Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
		int tileWidth = (int)tileSize.x;
		int tileDepth = (int)tileSize.z;

		for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
			for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
				Vector3 tilePosition = new Vector3(
				this.gameObject.transform.position.x + xTileIndex * tileWidth,
				this.gameObject.transform.position.y,
				this.gameObject.transform.position.z + zTileIndex * tileDepth
				);
				GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
			}
		}
	}
}