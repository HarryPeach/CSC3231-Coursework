using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Terrain))]
public class TerrainDeformation : MonoBehaviour
{
	private Terrain _terrain;
	private int _heightmapResolution;
	private int _alphamapResolution;
	private int _alphaLayersCount;

	private float[,] _heightmapBackup;
	private float[,,] _alphamapBackup;

	private void Start()
	{
		_terrain = GetComponent<Terrain>();
		TerrainData terrainData = _terrain.terrainData;

		_heightmapResolution = terrainData.heightmapResolution;
		_alphamapResolution = terrainData.alphamapResolution;
		_alphaLayersCount = terrainData.alphamapLayers;

		// Save a copy of the current height and alpha maps so that it can be restored when the application quits
		_heightmapBackup = terrainData.GetHeights(0, 0, _heightmapResolution, _heightmapResolution);
		_alphamapBackup = terrainData.GetAlphamaps(0, 0, _alphamapResolution, _alphamapResolution);
	}

	private void OnApplicationQuit()
	{
		// Restore the backed up heights and alphas when the application quits
		_terrain.terrainData.SetHeights(0, 0, _heightmapBackup);
		_terrain.terrainData.SetAlphamaps(0, 0, _alphamapBackup);
	}

	public void ExplodeTerrain(Vector3 pos, float blastSize, float textureSize)
	{
		Debug.Log($"Exploding at: {pos}");
		DeformTerrain(pos, blastSize);
	}

	/// <summary>
	/// Deform the terrain at the given position
	/// </summary>
	/// <param name="pos">The position to deform the terrain at</param>
	/// <param name="blastSize">The size of the blast</param>
	private void DeformTerrain(Vector3 pos, float blastSize)
	{
		Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, _heightmapResolution);
		TerrainData terrainData = _terrain.terrainData;

		int heightmapBlastWidth =
			(int)(blastSize * (_heightmapResolution / terrainData.size.x));
		int heightmapBlastLength =
			(int)(blastSize * (_heightmapResolution / terrainData.size.z));

		int heightmapStartPosX = (int)(terrainPos.x - heightmapBlastWidth / 2.0);
		int heightmapStartPosZ = (int)(terrainPos.z - heightmapBlastLength / 2.0);

		float[,] heights = _terrain.terrainData.GetHeights(heightmapStartPosX, heightmapStartPosZ,
			heightmapBlastWidth, heightmapBlastLength);
		float deformationDepth = (blastSize / 3.0f) / terrainData.size.y;

		for (int i = 0; i < heightmapBlastLength; i++)
		{
			for (int j = 0; j < heightmapBlastWidth; j++)
			{
				float circlePosX = (j - heightmapBlastWidth / 2) /
				                   (_heightmapResolution / terrainData.size.x);
				float circlePosY = (i - heightmapBlastLength / 2) /
				                   (_heightmapResolution / terrainData.size.z);
				float distanceFromCentre =
					Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
				float depthMult = (blastSize / 2.0f - distanceFromCentre) / (blastSize / 2.0f);

				//convert back to values without skew
				if (!(distanceFromCentre < (blastSize / 2.0f))) continue;

				depthMult += 0.1f;
				depthMult += Random.value * .1f;

				depthMult = Mathf.Clamp(depthMult, 0, 1);
				heights[i, j] = Mathf.Clamp(heights[i, j] - deformationDepth * depthMult, 0, 1);
			}
		}

		_terrain.terrainData.SetHeights(heightmapStartPosX, heightmapStartPosZ, heights);
	}

	/// <summary>
	/// Get the normalised position of the game object relative to the terrain
	/// </summary>
	/// <param name="pos">The input position</param>
	/// <returns>The normalised position</returns>
	/// <remarks>Modified from http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime</remarks>
	private Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos)
	{
		Vector3 tempCoord = pos - _terrain.gameObject.transform.position;
		TerrainData terrainData = _terrain.terrainData;
		Vector3 coord = new(tempCoord.x / terrainData.size.x, tempCoord.y / terrainData.size.y,
			tempCoord.z / terrainData.size.z);

		return coord;
	}

	/// <summary>
	/// Get the position of the terrain heightmap where this object is
	/// </summary>
	/// <param name="pos">The input position</param>
	/// <param name="mapRes">Resolution of the heightmap</param>
	/// <returns>The position of the heightmap where the object is</returns>
	private Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos, int mapRes)
	{
		Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos);
		return new Vector3(coord.x * mapRes, 0, coord.z * mapRes);
	}
}