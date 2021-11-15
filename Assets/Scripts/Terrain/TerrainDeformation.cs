using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain
{
	/// <summary>
	/// Performs basic terrain deformation, such as explosions
	/// </summary>
	/// <remarks>Some terrain deformation code modified from: http://blog.almostlogical.com/2010/06/10/real-time-terrain-deformation-in-unity3d/</remarks>
	[RequireComponent(typeof(UnityEngine.Terrain))]
	public class TerrainDeformation : MonoBehaviour
	{
		[Header("Particle System")]
		[SerializeField] [Tooltip("The particle system that creates the debris")] 
		private new ParticleSystem particleSystem;

		[Header("Explosion")]
		[SerializeField] [Tooltip("The prefab to create at the site of every explosion")] 
		private GameObject explosionPrefab;
		[SerializeField] [Tooltip("How large the explosion of the debris should be")]
		private float debrisBlastSize = 10f;
		[SerializeField] [Tooltip("Whether to change the alphas on an explosion")]
		private bool deformAlphas = true;
		[SerializeField] [Tooltip("Whether to change the terrain on an explosion")] 
		private bool deformTerrain = true;
		[SerializeField] [Tooltip("The index of the texture to paint on an explosion")] [Range(0, 10)]
		private int terrainDeformationTextureNum = 1;

		[Header("Splash")]
		[SerializeField] [Tooltip("The prefab to create at the site of debris hitting water")]
		private GameObject splashPrefab;
		[SerializeField] [Tooltip("How large the splash of debris should be")]
		private float splashSize = 10f;
		[SerializeField] [Tooltip("The y-height of water, for optimization and splash purposes")] 
		private int waterHeight = 80;

		private UnityEngine.Terrain _terrain;
		private int _heightmapResolution;
		private int _alphamapResolution;
		private int _alphaLayersCount;

		private float[,] _heightmapBackup;
		private float[,,] _alphamapBackup;

		private List<ParticleCollisionEvent> _collisionEvents;

		#region Unity Callbacks

		private void Start()
		{
			_terrain = GetComponent<UnityEngine.Terrain>();
			TerrainData terrainData = _terrain.terrainData;

			_heightmapResolution = terrainData.heightmapResolution;
			_alphamapResolution = terrainData.alphamapResolution;
			_alphaLayersCount = terrainData.alphamapLayers;

			_collisionEvents = new List<ParticleCollisionEvent>();

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

		private void OnParticleCollision(GameObject other)
		{
			int numCollisionEvents = particleSystem.GetCollisionEvents(_terrain.gameObject, _collisionEvents);

			// Find all collisions and detonate them
			for (int i = 0; i < numCollisionEvents; i++)
			{
				// Don't detonate if the debris is underwater
				// BUG: This spawns the splash at the site of the landing (underwater) instead of where the debris hits the water
				if (_collisionEvents[i].intersection.y < waterHeight)
				{
					Vector3 colLoc = _collisionEvents[i].intersection;
					splashPrefab.transform.localScale = new Vector3(splashSize / 5f, splashSize / 5f, splashSize / 5f);
					Instantiate(splashPrefab, new Vector3(colLoc.x, waterHeight, colLoc.z), Quaternion.identity);
					return;
				}
				
				explosionPrefab.transform.localScale = new Vector3(debrisBlastSize / 5f, debrisBlastSize / 5f, debrisBlastSize / 5f);
				Instantiate(explosionPrefab, _collisionEvents[i].intersection, Quaternion.identity);
				ExplodeTerrain(_collisionEvents[i].intersection, debrisBlastSize, 1f);
			}
		}

		#endregion

		/// <summary>
		/// Explode the terrain at a given point
		/// </summary>
		/// <param name="pos">The position to explode at</param>
		/// <param name="blastSize">The size of the blast (crater)</param>
		/// <param name="textureSizeFactor">How large the blast texture should be in comparison to the blast</param>
		public void ExplodeTerrain(Vector3 pos, float blastSize, float textureSizeFactor)
		{
			if (deformTerrain) DeformTerrain(pos, blastSize);
			if (deformAlphas) ChangeAlphas(pos, blastSize * textureSizeFactor);
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
			float deformationDepth = blastSize / 3.0f / terrainData.size.y;

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
					if (!(distanceFromCentre < blastSize / 2.0f)) continue;

					depthMult += 0.1f;
					depthMult += Random.value * .1f;

					depthMult = Mathf.Clamp(depthMult, 0, 1);
					heights[i, j] = Mathf.Clamp(heights[i, j] - deformationDepth * depthMult, 0, 1);
				}
			}

			_terrain.terrainData.SetHeights(heightmapStartPosX, heightmapStartPosZ, heights);
		}

		/// <summary>
		/// Modify the alphas at the given position, i.e. blast scorching
		/// </summary>
		/// <param name="pos">The position to modify</param>
		/// <param name="blastSize">The size of the blast</param>
		private void ChangeAlphas(Vector3 pos, float blastSize)
		{
			TerrainData terrainData = _terrain.terrainData;
			Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos(pos, _alphamapResolution);
			int alphaMapCraterWidth = (int)(blastSize * (_alphamapResolution / terrainData.size.x));
			int alphaMapCraterLength = (int)(blastSize * (_alphamapResolution / terrainData.size.z));

			int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - alphaMapCraterWidth / 2.0);
			int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - alphaMapCraterLength / 2.0);

			float[,,] alphas = _terrain.terrainData.GetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ,
				alphaMapCraterWidth, alphaMapCraterLength);

			for (int i = 0; i < alphaMapCraterLength; i++) //width
			{
				for (int j = 0; j < alphaMapCraterWidth; j++) //height
				{
					float circlePosX = (j - alphaMapCraterWidth / 2) /
					                   (_alphamapResolution / terrainData.size.x);
					float circlePosY = (i - alphaMapCraterLength / 2) /
					                   (_alphamapResolution / terrainData.size.z);

					//convert back to values without skew
					float distanceFromCenter =
						Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));

					if (!(distanceFromCenter < blastSize / 2.0f)) continue;
					for (int layerCount = 0; layerCount < _alphaLayersCount; layerCount++)
					{
						alphas[i, j, layerCount] =
							layerCount == terrainDeformationTextureNum ? 1 : 0;
					}
				}
			}

			_terrain.terrainData.SetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphas);
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
}