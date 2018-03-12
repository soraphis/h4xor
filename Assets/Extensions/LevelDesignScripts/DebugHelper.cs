using System.Collections.Generic;
using UnityEngine;

namespace DebugLabel {
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    internal class LogData {
        public readonly string text;
        public readonly Vector3 pos;
        public float time;

        public LogData(string text, Vector3 pos, float time) {
            this.text = text;
            this.pos = pos;
            this.time = time;
        }
    }

    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public static class DebugHelper {

        private static readonly List<LogData> worldData = new List<LogData>();
        private static readonly List<LogData> screenData = new List<LogData>();

        static DebugHelper(){
            #if UNITY_EDITOR
            SceneView.onSceneGUIDelegate += OnGUI;
            #endif
        }
#if UNITY_EDITOR
        private static void OnGUI(SceneView sceneView) {
            worldData.ForEach(d => Handles.Label(d.pos, d.text));
            screenData.ForEach(d => Handles.Label(Camera.current.ScreenToWorldPoint(d.pos), d.text));

            worldData.RemoveAll(d => d.time < EditorApplication.timeSinceStartup);
            screenData.RemoveAll(d => d.time < EditorApplication.timeSinceStartup);
        }
#endif
        /// <summary>
        /// Logt einen Text für 'time' sekunden auf dem Scene View. Welt position.
        /// </summary>
        public static void LogWorld(string text, Vector3 worldposition, float time = 0) {
#if UNITY_EDITOR || DEBUG
            time = Mathf.Max(time, Time.deltaTime);
            var d = new LogData(text, worldposition, (float)EditorApplication.timeSinceStartup+time);
            worldData.Add(d);
#endif
        }

        /// <summary>
        /// Logt einen Text für 'time' sekunden auf dem Scene View. (0, 0) ist links unten, (100, 100) ist ein guter startwert.
        /// </summary>
        public static void LogScreen(string text, Vector3 screenposition, float time = 0) {
#if UNITY_EDITOR || DEBUG
            time = Mathf.Max(time, Time.deltaTime);
            var d = new LogData(text, screenposition, (float)EditorApplication.timeSinceStartup+time);
            screenData.Add(d);
#endif
        }




    }

}