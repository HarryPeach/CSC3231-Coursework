using System;
using System.Collections;
using System.Collections.Generic;
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
    private float[, ,] _alphamapBackup;

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

    private void DeformTerrain(Vector3 pos, float blastSize)
    {
        Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, _terrain, _heightmapResolution,
            _heightmapResolution);
        int heightmapBlastWidth =
            (int)(blastSize * (_heightmapResolution / _terrain.terrainData.size.x));
        int heightmapBlastLength =
            (int)(blastSize * (_heightmapResolution / _terrain.terrainData.size.z));

        int heightmapStartPosX = (int)(terrainPos.x - heightmapBlastWidth / 2);
        int heightmapStartPosZ = (int)(terrainPos.z - heightmapBlastLength / 2);

        float[,] heights = _terrain.terrainData.GetHeights(heightmapStartPosX, heightmapStartPosZ,
            heightmapBlastWidth, heightmapBlastLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCentre;
        float depthMult;
        float deformationDepth = (blastSize / 3.0f) / _terrain.terrainData.size.y;

        for (int i = 0; i < heightmapBlastLength; i++)
        {
            for (int j = 0; j < heightmapBlastWidth; j++)
            {
                circlePosX = (j - (heightmapBlastWidth / 2)) / (_heightmapResolution / _terrain.terrainData.size.x);
                circlePosY = (i - (heightmapBlastLength / 2)) / (_heightmapResolution / _terrain.terrainData.size.z);
                distanceFromCentre = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                //convert back to values without skew
                 
                if (distanceFromCentre < (blastSize / 2.0f))
                {
                    depthMult = ((blastSize / 2.0f - distanceFromCentre) / (blastSize / 2.0f));
 
                    depthMult += 0.1f;
                    depthMult += Random.value * .1f;
 
                    depthMult = Mathf.Clamp(depthMult, 0, 1);
                    heights[i, j] = Mathf.Clamp(heights[i, j] - deformationDepth * depthMult, 0, 1);
                }
            }
        }
        _terrain.terrainData.SetHeights(heightmapStartPosX, heightmapStartPosZ, heights);
    }
    
    private Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        //code based on: http://answers.unity3d.com/questions/3633/modifying-terrain-height-under-a-gameobject-at-runtime
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / _terrain.terrainData.size.x;
        coord.y = tempCoord.y / _terrain.terrainData.size.y;
        coord.z = tempCoord.z / _terrain.terrainData.size.z;
 
        return coord;
    }
 
    private Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos,Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        // get the position of the terrain heightmap where this game object is
        return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
    }     
}
