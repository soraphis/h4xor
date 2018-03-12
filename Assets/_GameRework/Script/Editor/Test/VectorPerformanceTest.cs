

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

namespace _GameRework.Script.Editor.Test {

    static class Vector3Additons {
        public static void Add(this Vector3 v, Vector3 other) {
            v.x += other.x;
            v.y += other.y;
            v.z += other.z;
        }
    } 
    
    public class VectorPerformanceTest {
        
        [Test] 
        public void Vector3AddPerformance() {
            Vector3 a;
            Stopwatch watch;
            
            // ////////////
            
            watch = new Stopwatch();
            watch.Start();
            a = Vector3.zero;
            for (int i = 0; i < 1e6; ++i) {
                a.Add(Vector3.one);
                
            }
            watch.Stop();
            
            Console.WriteLine($"add method: \t{watch.ElapsedMilliseconds}ms");
            
            // ////////////
            
            watch = new Stopwatch();
            watch.Start();
            a = Vector3.zero;
            for (int i = 0; i < 1e6; ++i) {
                a += Vector3.one;
            }
            watch.Stop();
            
            Console.WriteLine($"+= operator: \t{watch.ElapsedMilliseconds}ms");
        }
        
    }
}