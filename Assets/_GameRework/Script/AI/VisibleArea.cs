

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.ScriptRework.AI {
    
    /// <summary>
    /// This Class implements Algorithms to Calculate a Visibility Area in the Way some bordgames (e.g. Decent) do
    /// </summary>
    public static class VisibleArea {

        private static float sqrt_vector_length(float a, float b) { return a * a + b * b; }

        private static bool LineOfSight(int x1, int y1, int x2, int y2, ref bool[,] corners) {
            foreach (var c1 in CornersOfTile(x1, y1)) {
                foreach (var c2 in CornersOfTile(x2, y2)) {
                    // do line of sight from c1 to c2:

                    var dx = c2.x - c1.x;
                    var dy = c2.y - c1.y;

                    // x-cuts
                    var steps = (float)Math.Abs(dx);
                    var inc = dy / steps;

                    var blocked = false;
                    for (int v = 0; v < steps; ++v) {
                        var x = c1.x + v;
                        var y = c1.y + inc * v;

                        var y_up = Mathf.CeilToInt(y);
                        var y_down = Mathf.FloorToInt(y);

                        if (corners[x, y_up] && corners[x, y_down]) {
                            blocked = true;
                            break;
                        }
                    }
                    if(blocked) break;
                    // y-cuts
                    steps = (float)Math.Abs(dy);
                    inc = dx / steps;
                    
                    blocked = false;
                    for (int v = 0; v < steps; ++v) {
                        var x = c1.x + inc * v;
                        var y = c1.y + v;

                        var x_up = Mathf.CeilToInt(x);
                        var x_down = Mathf.FloorToInt(x);

                        if (corners[x_up, y] && corners[x_down, y]) {
                            blocked = true;
                            break;
                        }
                    }
                    if(blocked) break;

                    return true;
                }
            }
            
            return false;
            
        }

        private static IEnumerable<NVector2> CornersOfTile(int x, int y) {
            yield return new NVector2(x, y);
            yield return new NVector2(x+1, y);
            yield return new NVector2(x, y+1);
            yield return new NVector2(x+1, y+1);
        }
        
        public static bool[,] CalculateVisibleArea(NVector2 position, int range, NVector2 forward, float viewAngle = 0) {
            
            var right = new NVector2(forward.y, -forward.x);
            
            // step 1: get opaque tiles in region
            bool[,] opaques = new bool[(range+1)*2+1, 1+(range+1)+1];
            // pos_tile = [range+1, 1]
            for(int dx = -(range+1); dx <= (range+1); ++dx){
                for (int dy = -1; dy <= (range + 1); ++dy) {
                    var pos = position + forward * dy + right * dx;
                    var b = GridUtil.IsStaticObstacle(pos);
                    // Debug.Log(dx + ", " + dy + ": " + b);
                    opaques[(range+1)+dx, dy+1] = b;
                }
            }
            
            // step 2: get tile corners:
            // a corner is true if any of the 4 tiles that the corner belongs to are true
            // a corner is true if you cant look through it
            var range2 = range * range;
            
            bool[,] corners = new bool[opaques.GetLength(0)-1, opaques.GetLength(1)-1];
            for (int x = 0; x < corners.GetLength(0); ++x) {
                for (int y = 0; y < corners.GetLength(1); ++y) {
                    corners[x, y] = opaques[x, y] | opaques[x + 1, y] | opaques[x, y + 1] | opaques[x + 1, y + 1];
                    corners[x, y] |= (range2 <= sqrt_vector_length(x - 0.5f - range, y - 0.5f) );
                    // corners[x, y] |= Math.Abs((new Vector2(x - 0.5f - range, y - 0.5f).normalized).y) <= viewAngle;
                } 
            }

/*            GameTickManager.Instance.Gizmos += () => {
                
                
                for (int x = 0; x < corners.GetLength(0); ++x) {
                    for (int y = 0; y < corners.GetLength(1); ++y) {
                        var wpos = GridUtil.GridToWorld((position + forward*y + right*(x-range)));
                        Gizmos.color = corners[x,y] ? Color.magenta : Color.cyan;
                        
                        if(corners[x,y] && (range2 <= sqrt_vector_length(x - 0.5f - range, y - 0.5f) ))
                            Gizmos.color = Color.red;
                        Gizmos.DrawCube(wpos + new Vector3(0.5f, 0, 0.5f), Vector3.one * 0.15f);
                    } 
                }
                
                
                Debug.Break();
            };*/
            
            // step 3: visible tiles:
            bool[,] visibles = new bool[range*2+1,range+1];

            NVector2 pos_tile = new NVector2(range, 0);
            for (int x = 0; x < visibles.GetLength(0); ++x) {
                for (int y = 0; y < visibles.GetLength(1); ++y) {
                    if (opaques[x + 1, y + 1]) { visibles[x, y] = false; continue; }
                    if ((new Vector2(x - range, y).normalized).y <= viewAngle) { visibles[x, y] = false; continue; }
                    
                    if (CornersOfTile(x, y).All((xy) => corners[xy.x, xy.y])) { visibles[x, y] = false; continue; }
                    var b = LineOfSight(pos_tile.x, pos_tile.y, x, y, ref corners);
                    visibles[x, y] = b;
                    
                    
                } 
            }

/*
            GameTickManager.Instance.Gizmos += () => {
                
                for (int x = 0; x < visibles.GetLength(0); ++x) {
                    for (int y = 0; y < visibles.GetLength(1); ++y) {
                        var wpos = GridUtil.GridToWorld((position + forward*y + right*(x-range)));
                        
                        if (opaques[x + 1, y + 1]) {
                            Gizmos.color = Color.black;
                            Gizmos.DrawWireCube(wpos , Vector3.one * 0.15f);
                            continue;
                        }
                        if (!visibles[x, y]) {
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireCube(wpos , Vector3.one * 0.15f);
                            continue;
                        }
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireCube(wpos , Vector3.one * 0.15f);
                        continue;
                        
                    } 
                }
                
                Debug.Break();
            };
*/
            
            return visibles;
        }
        
    }
}