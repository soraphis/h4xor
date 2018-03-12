using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace _Game.ScriptRework.Util {


    public static class GenerationLib{
        public static float[,,] makeVolume(float[][] dims, Func<float, float, float, float> f) {
            var res = new int[3];

            for (var i = 0; i < 3; ++i) {
                res[i] = 2 + Mathf.CeilToInt((dims[i][1] - dims[i][0]) / (float)dims[i][2]);
            }
            var volume = new float[res[0], res[1], res[2]];

            float z = dims[2][0] - dims[2][2];
            for (int k = 0; k < res[2]; ++k, z += dims[2][2]) {
                float y = dims[1][0] - dims[1][2];
                for (int j = 0; j < res[1]; ++j, y += dims[1][2]) {
                    float x = dims[0][0] - dims[0][2];
                    for (int i = 0; i < res[0]; ++i, x += dims[0][2]) {
                        volume[i, j, k] = f(x, y, z);
                    }
                }
            }
            return volume;
        }
    }
    
    
public static class SurfaceNets {

    // private static Thread[] workers = new Thread[2];
    private static Queue<Action> queue = new Queue<Action>();
    private static object _door = new object();

    public static void Produce(Action a) {
        lock (_door) {
            queue.Enqueue(a);
            Monitor.Pulse(_door);
        }
    }

    private static void Consume() {
        while (true) {
            Action item = null;
            lock (_door) {
                while (queue.Count < 1){
                    Monitor.Wait(_door);
                }
                item = queue.Dequeue();
                Monitor.Pulse(_door);
            }
            item();
        }
    }

    // /////////////////////////////////

    private static void AddOrUpdate<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 val) {
        if (dict.ContainsKey(key)) dict[key] = val;
        else dict.Add(key, val);
    }

    private static int[] cube_edges = new int[24];
    private static int[] edge_table = new int[256];

    static SurfaceNets() {
        int k = 0;
        for (int i = 0; i < 8; ++i) {
            for (int j = 1; j <= 4; j = j << 1) {
                int p = i ^ j;
                if (i <= p) {
                    cube_edges[k++] = i;
                    cube_edges[k++] = p;
                }
            }
        }

        for (int i = 0; i < 256; ++i) {
            int em = 0;
            for (int j = 0; j < 24; j += 2) {
                var a = (i & (1 << cube_edges[j])) != 0;
                var b = (i & (1<<cube_edges[j+1])) != 0;
                em |= a != b ? (1 << (j >> 1)) : 0;
            }
            edge_table[i] = em;
        }
/*
        workers[0] = new Thread(Consume);
        workers[1] = new Thread(Consume);

        workers[0].Start();
        workers[1].Start();
*/
    }

//    public static void GenerateSurfaceMesh(ref Mesh mesh, float[,,] voxelData, float level = 50) {
//        
//        lock (_door) {
//            Mesh mesh1 = mesh;
//            queue.Enqueue(() => {
//                List<Vector3> flat_vertices = null;
//                List<int> flat_indices = null;
//                List<Vector2> myUVs = null;
//
//                GenerateSurfaceMesh(voxelData, level, out flat_vertices, out flat_indices, out myUVs);
//
//                WorldGenerator.AddTask(() => {
//                    mesh1.SetVertices(flat_vertices/*.Select(v => new Vector3((int)v.x, (int)v.y, (int)v.z)).ToList()*/);
//                    mesh1.SetIndices(flat_indices.ToArray(), MeshTopology.Quads, 0);
//
//                    mesh1.uv = myUVs.ToArray();
//
//                    mesh1.RecalculateBounds();
//                    mesh1.RecalculateNormals();
//                }, "Generating Surface Mesh, mesh null? " + (mesh1 == null));
//
//            });
//            
//            Monitor.Pulse(_door);
//        }
//    }

        
    public static void GenerateSurfaceMesh(float[,,] voxelData, float level, 
        out List<Vector3> flat_vertices, out List<int> flat_indices, out List<Vector2> myUVs) {
        
        var vertices = new List<Vector3>();
        var indices = new List<int>();
        //var uvs = new Dictionary<int, Vector2>();

        int[] R = new int[]{1, 
            voxelData.GetLength(0)+1,
            ((voxelData.GetLength(0) + 1) * (voxelData.GetLength(1) + 1))};
        var buffer = new int[ R[2] * 2];

        int n = 0;
        int buf_no = 1;

        int[] x = new int[3];

        for (x[2] = 0; x[2] < voxelData.GetLength(2) - 1; ++x[2]) {

            var m = 1 + (voxelData.GetLength(0) + 1) * (1 + buf_no * (voxelData.GetLength(1) + 1));
                        // for dimensions 16,16,16 this evaluates to: 579 or 290 (depending on buf_no)

            for (x[1] = 0; x[1] < voxelData.GetLength(1) - 1;     ++x[1], ++n, m += 2){
                for (x[0] = 0; x[0] < voxelData.GetLength(0) - 1; ++x[0], ++n, ++m) {

                    float[] grid = new float[8];

					int mask = 0, g = 0, idx = n;
					for(int k=0; k<2; ++k, idx += voxelData.GetLength(0)*(voxelData.GetLength(1)-2))
					for(int j=0; j<2; ++j, idx += voxelData.GetLength(0)-2)
					for(int i=0; i<2; ++i, ++g, ++idx){
						float p = voxelData[x[0] + i, x[1] + j, x[2] + k] - level;
						grid[g] = p;
						// Debug.Log($"{n} -> {g}: {p}");
						mask |= (p< 0) ? (1<<g) : 0;
					}

                    /*int mask = 0;
                    foreach (var dx in new int[8][]{new int[]{0, 0, 0}, new int[]{0, 0, 1}, new int[]{1, 1, 0}, new int[]{0, 1, 0}, new int[]{0, 1, 1}, new int[]{1, 1, 1}, new int[]{1, 0, 0}, new int[]{1, 0, 1}}) {
                        var data = voxelData[x[0] + dx[0], x[1] + dx[1], x[2] + dx[2]] - 50;
                        var g = dx[0] + (dx[1] << 1) + (dx[2] << 2);
                        // Debug.Log($"{g}, {dx[0]} {dx[1]} {dx[2]} -- {(dx[1] << 1)} - {(dx[2] << 2)}");
                        grid[g] = data;
                        mask |= (data < 0) ? (1 << g) : 0;
                    }*/
                    
                    if(mask == 0 || mask == 0xff) continue;

                    var edge_mask = edge_table[mask];
                    var v = new Vector3();
                    var e_count = 0; // number of edge crossings

                    for (var i = 0; i < 12; ++i) {
                        if ((edge_mask & (1 << i)) == 0) continue;
                        ++e_count;

                        var e0 = cube_edges[i << 1]; // unpack vertices
                        var e1 = cube_edges[(i << 1) + 1];
                        var g0 = grid[e0]; // unpack grid values
                        var g1 = grid[e1];
                        var t = g0 - g1; // intersection point

                        if (Math.Abs(t) <= 1e-6) continue;
                        t = g0 / t;

                        for (int j = 0, k = 1; j < 3; ++j, k = k<<1) {
                            var a = e0 & k;
                            var b = e1 & k;
                            if (a != b) {
                                v[j] += a != 0 ? 1.0f - t : t;
                            } else {
                                v[j] += a != 0 ? 1.0f : 0;
                            }
                        }
                    }
                    //Now we just average the edge intersections and add them to coordinate
                    var s = 1.0f / e_count;
	                v[0] = x[0] + s * v[0];
	                v[1] = x[1] + s * v[1];
	                v[2] = x[2] + s * v[2];

                    //Add vertex to buffer, store pointer to vertex index in buffer
                    buffer[m] = vertices.Count;
                    // vertices.Add(v);
                    vertices.Add(new Vector3((int)v.x, (int)v.y, (int)v.z));
                    
                    for (var i = 0; i < 3; ++i) {
                        if((edge_mask & (1 << i)) == 0) continue;

                        var iu = (i + 1) % 3;
                        var iv = (i + 2) % 3;

                        if(x[iu] == 0 || x[iv] == 0) continue;

                        var du = R[iu];
                        var dv = R[iv];


                        if ((mask & 1) != 0) {
                            indices.AddRange(new int[] {buffer[m], buffer[m - du], buffer[m - du - dv], buffer[m - dv]});
                            /*uvs.AddOrUpdate(m, new Vector2(0, 0));
                            uvs.AddOrUpdate(m - du, new Vector2(0, 1));
                            uvs.AddOrUpdate(m - du - dv, new Vector2(1, 1));
                            uvs.AddOrUpdate(m - dv, new Vector2(1, 0));*/

                            /*indices.AddRange(new int[]{flat_vertices.Count, flat_vertices.Count+1, flat_vertices.Count+2, flat_vertices.Count+3});
                            flat_vertices.AddRange(new Vector3[]{vertices[m], vertices[m - du], vertices[m - du - dv], vertices[m - dv]});*/


                        } else {
                            indices.AddRange(new int[] {buffer[m], buffer[m-dv], buffer[m-du-dv], buffer[m-du]});
                            /*uvs.AddOrUpdate(m, new Vector2(0, 0));
                            uvs.AddOrUpdate(m - dv, new Vector2(0, 1));
                            uvs.AddOrUpdate(m - du - dv, new Vector2(1, 1));
                            uvs.AddOrUpdate(m - du, new Vector2(1, 0));*/

                            /*indices.AddRange(new int[]{flat_vertices.Count, flat_vertices.Count+1, flat_vertices.Count+2, flat_vertices.Count+3});
                            flat_vertices.AddRange(new Vector3[]{vertices[m], vertices[m - dv], vertices[m - du - dv], vertices[m - du]});*/
                        }

                    }
                }
            }
            buf_no ^= 1;
            R[2] = -R[2];
            n += voxelData.GetLength(0);
        }

		// flat shading:

        flat_vertices = new List<Vector3>();
        flat_indices = new List<int>();
        myUVs = new List<Vector2>();

        for (int i = 0; i < indices.Count; i+=4) {
            var idx = new int[] {indices[i], indices[i + 1], indices[i + 2], indices[i + 3]};

            flat_indices.AddRange(new int[]{flat_vertices.Count, flat_vertices.Count+1, flat_vertices.Count+2, flat_vertices.Count+3});
            flat_vertices.AddRange(new Vector3[]{vertices[idx[0]], vertices[idx[1]], vertices[idx[2]], vertices[idx[3]]});
            myUVs.AddRange(new Vector2[]{new Vector2(0, 0), new Vector2(1, 0),new Vector2(1, 1), new Vector2(1, 0)});
        }

		// Debug.Log(buffer.Select(a => a.ToString()).Aggregate((i, j) => i + "," + j));

        //var mesh = new Mesh();


        //return mesh;
    }

}



}