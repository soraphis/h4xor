﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridUtil {
    
    /// <summary>
    /// true, if the tile is outside of the world border, or static geometry
    /// </summary>
    private static readonly HashSet<NVector2> worldBorderCache = new HashSet<NVector2>();

    public static Vector3 GridToWorld(Vector2 pos) {
        return new Vector3(pos.x + 0.5f, 0, pos.y + 0.5f);
    }

    public static Vector2 WorldToGrid(Vector3 pos) {
        return new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.z));
    }

    /// <summary>
    /// Shoots a ray down on a field on the grid. If something is hit, "hit" will contain the corresponding RaycastHit.
    /// </summary>
    /// <param name="pos">The position on the grid</param>
    /// <param name="hit">If something is hit, more information will be stored here.</param>
    /// <returns>true, if something is hit, false otherwise</returns>
	public static bool RaycastGrid(Vector2 pos, out RaycastHit hit, int layerMask = Physics.DefaultRaycastLayers) {
        return Physics.Raycast(GridToWorld(pos) + new Vector3(0, 10, 0), new Vector3(0, -1, 0), out hit, maxDistance: 11, layerMask: layerMask);
    }
    
    public static List<GameObject> FindEntitysInRange(Vector2 gridPos, float radius) {
        return Physics.OverlapCapsule(GridToWorld(gridPos) + new Vector3(0, 10, 0),
                                      GridToWorld(gridPos) + new Vector3(0, -10, 0), radius)
                      .Select(c => c.gameObject)
                      .ToList();
    }

    public static GameObject FindEntityOnGrid(Vector2 gridPos) {
        RaycastHit hit;
        if(!RaycastGrid(gridPos, out hit)) return null;
        //If a placeholder was hit, try to get the entity it belongs to
        
        if (hit.collider.gameObject.tag == GameTag.MovementPlaceholder)
        {
            GameObject obj;
/*            if (MovementBehaviour.placeholderToGameObject.TryGetValue(hit.collider.gameObject, out obj))
            {
                return obj;
            }
            else
            {
                Debug.Log("Not found (If this happens something went wrong.)");
            }*/
        }
        return hit.collider.gameObject;
    }

    /// <summary>
    /// Checks if there is a static obstacle on the grid. 
    /// </summary>
    /// <param name="gridPos">The position on the grid to test</param>
    /// <returns>true, if there is a static obstacle or no ground at all, false, if there is no static obstacle and ground</returns>
    public static bool IsStaticObstacle(Vector2 gridPos)
    {
        RaycastHit hit;
        if (RaycastGrid(gridPos, out hit, (GameLayer.GroundMask | GameLayer.StaticGeometryMask))) {
            if (hit.collider.gameObject.layer == GameLayer.Ground)
            {
                return false;
            }
        }
        return true;
    }


    public static bool IsPassable(Vector2 pos) {
        RaycastHit hit;

        if (worldBorderCache.Contains(new NVector2(pos))) { return false; }
        if (!RaycastGrid(pos, out hit, (GameLayer.DefaultMask | GameLayer.GroundMask | GameLayer.StaticGeometryMask))) {
            worldBorderCache.Add(new NVector2(pos));
            return false;
        }
        
        switch (hit.collider.gameObject.layer) {
            case GameLayer.Ground: return true;
            case GameLayer.StaticGeometry: worldBorderCache.Add(new NVector2(pos)); goto default;
            default: return false;
        }
    }



}