using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.ScriptRework.Util;

public static class Astar
{
    public static List<NVector2> blockedPositions = new List<NVector2>();

    private static NVector2[] NeighborOffset = new[]
        {new NVector2(1, 0), new NVector2(-1, 0), new NVector2(0, 1), new NVector2(0, -1)};
    
    class Node
    {
        public NVector2 point;
        public int distance;
        public Node parent;

        public Node(NVector2 point, int dis = 0)
        {
            this.point = point;
            this.distance = dis;
        }

        protected bool Equals(Node other) {
            return point.Equals(other.point);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode() {
            return point.GetHashCode();
        }

        public static bool operator ==(Node left, Node right) { return Equals(left, right); }
        public static bool operator !=(Node left, Node right) { return !Equals(left, right); }
    }
    
    public delegate float heuristicFunc(NVector2 from, NVector2 to);
    public static heuristicFunc currentHeuristicFunc = CalculateHeuristic;

    [Obsolete]
    public static List<NVector2> CalculatePath(Vector3 startPos, Vector3 endPos, int depth = 10) { return CalculatePath(startPos, endPos, CalculateHeuristic, depth); }
    
    [Obsolete]
    public static List<NVector2> CalculatePath(Vector3 startPos, Vector3 endPos, heuristicFunc heuristicFunc, int depth = 10)
    {
        
        return new List<NVector2>();
        
    }

    #region AStar_FindPath

    private static void ExpandNode(Node current, NVector2 to, ref PriorityQueue<Node> openlist, ref HashSet<Node> closedlist,  int heurFactor = 1) {
        for (int i = 0; i < NeighborOffset.Length; ++i) {
            var offset = NeighborOffset[i];
            var child = new Node(current.point + offset);
            child.parent = current;
            
            // check if point available
            if(! GridUtil.IsPassable(child.point)) continue;
            
            if(closedlist.Contains(child)) continue;
            child.distance = current.distance + 1;
            var heur = heurFactor * (int)currentHeuristicFunc(child.point, to);
            if(openlist.Contains(child, heur)) continue;
            
            openlist.Enqueue(child, heur);
        }
    }

    public static List<NVector2> Path(NVector2 from, NVector2 to, int max_depth = 10, int distance = 0, int heurFactor = 1) {
        PriorityQueue<Node> openlist = new PriorityQueue<Node>();
        HashSet<Node> closedlist = new HashSet<Node>();
        
        openlist.Enqueue(new Node(from));

        distance = GridUtil.IsPassable(to) ? 0 : distance;
        
        do {
            var current = openlist.Dequeue();
            if (current.point == to || currentHeuristicFunc(current.point, to) <= distance) {
                List<NVector2> outlist = new List<NVector2>();
                outlist.Add(current.point);
                while (current.parent != null) {
                    current = current.parent;
                    outlist.Insert(0, current.point);                
                }
                return outlist;
            }
            closedlist.Add(current);

            if (current.distance >= max_depth) {
                break;
            }
            ExpandNode(current, to, ref openlist, ref closedlist, heurFactor: heurFactor);
            
        } while (! openlist.Empty);
        
        return new List<NVector2>();
    }

    #endregion


    #region Dijstra_FindArea

    private static void ExpandNode(Node current, ref Queue<Node> openlist, ref HashSet<Node> closedlist) {
        for (int i = 0; i < NeighborOffset.Length; ++i) {
            var offset = NeighborOffset[i];
            var child = new Node(current.point + offset);
            child.parent = current;
            
            // check if point available
            if(! GridUtil.IsPassable(child.point)) continue;
            
            if(closedlist.Contains(child)) continue;
            child.distance = current.distance + 1;
            
            openlist.Enqueue(child);
        }
    }
    
    public static List<NVector2> Area(NVector2 from, int max_depth, bool includeStart = false) {
        Queue<Node> openlist = new Queue<Node>();
        HashSet<Node> closedlist = new HashSet<Node>();
        
        openlist.Enqueue(new Node(from));

        do {
            var current = openlist.Dequeue();
           
            closedlist.Add(current);

            if (current.distance >= max_depth) {
                closedlist.Remove(current);
                break;
            }
            ExpandNode(current, ref openlist, ref closedlist);
            
        } while (openlist.Count > 0);

        var outlist = closedlist.Select(n => n.point).ToList(); 
        outlist.Remove(from);
        return outlist;
    }

    #endregion
    
   

    public static float CalculateHeuristic(NVector2 from, NVector2 to)
    {
        //float schnellerAberUngenauer = 1f;
        //return Mathf.Abs(schnellerAberUngenauer * ((to.x - from.x) + (to.y - from.y)));
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }
}

