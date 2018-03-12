using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Adds a Menu item "Tools/Utils/GameTags and Layers to Code" to generate class files for untiy layers and tags
/// </summary>
public class EditorVariablesCodeGen {

    private static SerializedProperty cached_tags;
    private static SerializedProperty cached_layers;
    private static Stopwatch sw = new Stopwatch();

    [InitializeOnLoadMethod]
    private static void Initialize(){
        sw.Start();
        EditorApplication.update += Generate;
    }

    private static void Generate() {
        if (EditorApplication.isPlaying) return;

        if (sw.ElapsedMilliseconds > 2000) { // once every two seconds
            GenerateCodeFiles();
            sw.Reset();
            sw.Start();
        }
    }

    [MenuItem("Tools/Utils/GameTags and Layers to Code")]
    static void GenerateCodeFiles() {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        const string writePath = "Assets/GameLayer.cs";
        const string CLASS_TEMPLATE = "public static class {CLASS_NAME} { \n\n{CLASS_BODY} }\n";
        const string VARIABLE_TEMPLATE = "    public const {TYPE} {NAME} = {VALUE};\n";

        string[] buildinTags = new[]
        {"Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController"};

        var tags = tagManager.FindProperty("tags");
        var layers = tagManager.FindProperty("layers");

        if(tags == cached_tags && layers == cached_layers) return;
        cached_tags = tags;
        cached_layers = layers;

        var OutputFile = "/* THIS FILE IS AUTOMATICALLY GENERATED! */\n";

        var _out = "";
        HashSet<string> tagnames = new HashSet<string>();
        foreach(var buildinTag in buildinTags) {
            var tagname = ValidString(buildinTag, tagnames);
            tagnames.Add(tagname);

            _out += VARIABLE_TEMPLATE.Replace("{TYPE}", "string")
                .Replace("{NAME}", tagname)
                .Replace("{VALUE}", '"' + buildinTag + '"');
        }

        for (int i = 0; i < tags.arraySize; ++i) {
            var c = tags.GetArrayElementAtIndex(i);

            var tagname = ValidString(c.stringValue, tagnames);
            if (string.IsNullOrEmpty(tagname)) continue;

            tagnames.Add(tagname);

            _out += VARIABLE_TEMPLATE.Replace("{TYPE}", "string")
                .Replace("{NAME}", tagname)
                .Replace("{VALUE}", '"'+c.stringValue+'"');
        }

        OutputFile += CLASS_TEMPLATE.Replace("{CLASS_NAME}", "GameTag").Replace("{CLASS_BODY}", _out);
        OutputFile += "\n";

        _out = "";
        HashSet<string> layernames = new HashSet<string>();
        for(int i = 0; i < layers.arraySize; ++i) {
            var c = layers.GetArrayElementAtIndex(i);
            if(string.IsNullOrEmpty(c.stringValue)) continue;

            var layername = ValidString(c.stringValue, layernames);
            layernames.Add(layername);

            _out += VARIABLE_TEMPLATE.Replace("{TYPE}", "int")
                .Replace("{NAME}", layername)
                .Replace("{VALUE}", i+"");
            _out += VARIABLE_TEMPLATE.Replace("{TYPE}", "int")
                .Replace("{NAME}", layername + "Mask")
                .Replace("{VALUE}", "1 << " +i);
        }
        OutputFile += CLASS_TEMPLATE.Replace("{CLASS_NAME}", "GameLayer").Replace("{CLASS_BODY}", _out);

        var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(writePath);
        if (asset != null && asset.text == OutputFile) return;

        File.WriteAllText(writePath, OutputFile);
        AssetDatabase.ImportAsset(writePath);
        AssetDatabase.Refresh();

    }

    private static string ValidString(string s, HashSet<string> invalidNames) {
        var m = s.Replace(" ", "").Replace('"', '_').Replace('\'', '_');
        if (invalidNames.Contains(m)) m = s.Replace(" ", "_");

        while (invalidNames.Contains(m)) {
            m = m.Replace("_", "__");
        }

        return m;
    }

}
