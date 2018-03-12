using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


public class Analyse {

    private static bool? analyzed = null; // ternary

    [InitializeOnLoadMethod] private static void InitializeAnalyzer() {

        EditorApplication.update += StaticCodeAnalysis;
    }

    private static void StaticCodeAnalysis() {
        if (EditorApplication.isPlaying) return;
        // //////////////////////////////////////////////
        // this makes it execute exactly once _after_ the editor has compiled
        if (EditorApplication.isCompiling) {
            analyzed = null;
            return;
        }
        if (analyzed == null) {
            analyzed = false;
            return;
        }
        if((bool)analyzed) return;
        analyzed = true;
        // //////////////////////////////////////////////

        var assets = AssetDatabase.FindAssets("t:Script", new[] { "Assets/_Game" });

        
        if (assets.Length < 1) return; // maybe redundant
        for (int i = 0; i < assets.Length; ++i) {
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(assets[i]));

            var regex = @"((((Game)?Object\.)|([^\.]))(Find[^(]*))";
            Rule_Call_inMethodBlock(regex, "void Update", asset, asset.text);
            Rule_Call_inMethodBlock(regex, "void LateUpdate", asset, asset.text);
            Rule_Call_inMethodBlock(regex, "void FixedUpdate", asset, asset.text);

            FindWithTag_no_constants(asset, asset.text);
            CompareTag(asset, asset.text);

            UsingUnityEditorInRuntime(asset, AssetDatabase.GUIDToAssetPath(assets[i]), asset.text);
        }
        
    }

    private static int LinesUntil(string code, int pos) {
        int res = 1;
        for (int i = 0; i <= pos - 1; i++)
            if (code[i] == '\n') ++res;
        return res;
    }

    private static int CodeBlockEnd(string code, int pos) {
        bool open = false;
        int bracket = 0;

        while (pos < code.Length) {
            if (bracket > 0) open = true;
            if (code[pos] == '{') bracket++;
            if (code[pos] == '}') bracket--;

            if (open == true && bracket == 0) return pos;
            
            ++pos;
        }
        return -1;
    }


    // ////////////////////////////////////////////

    private static MatchCollection FindCodeLineInScript(string regex, string code) {
        var rx = new Regex(regex);
        var matches = rx.Matches(code);
        return matches;
    }

    private static void UsingUnityEditorInRuntime(MonoScript asset, string assetPath, string code) {
        if( assetPath.Contains("/Editor/")) return;

        var matches = FindCodeLineInScript("(using UnityEditor;)", code);

        foreach (Match match in matches){
            int ln = LinesUntil(code, match.Index);
            Debug.LogError(string.Format("Illegal Usage of {0} in Line: {1}. Using UnityEditor outside Editorscripts.", match.Value, ln), asset);
        }
    }

    private static void CompareTag(MonoScript asset, string code) {
        foreach (Match match in FindCodeLineInScript(@"(CompareTag\((.*)""(.*)""(.*)\))", code)){
            int ln = LinesUntil(code, match.Index);
            Debug.LogError(string.Format("Illegal Usage of {0} in Line: {1}. Use `GameTag.<TagName>` instead of a String.", match.Value, ln), asset);
        }

        foreach (Match match in FindCodeLineInScript(@"(([\w]+\.)?tag =(=)?((.*)""(.*)""))", code)){
            int ln = LinesUntil(code, match.Index);
            Debug.LogError(string.Format("Illegal Usage of {0} in Line: {1}. Use `GameTag.<TagName>` instead of a String.", match.Value, ln), asset);
        }
        
        foreach (Match match in FindCodeLineInScript(@"(([\w]+\.)?tag.Equals\(((.*)""(.*)""))", code)){
            int ln = LinesUntil(code, match.Index);
            Debug.LogError(string.Format("Illegal Usage of {0} in Line: {1}. Use `GameTag.<TagName>` instead of a String.", match.Value, ln), asset);
        }
    }

    /// <summary>
    /// This rule searchs for all bad usages of "call" in "method"-block
    /// e.g. all usages of "FindGameObjectWithTag" in "Update"
    /// </summary>
    private static void Rule_Call_inMethodBlock(string call, string method, MonoScript asset, string code) {
        Regex rx = null;
        MatchCollection matches = null;

        rx = new Regex("("+ method + ")");
        matches = rx.Matches(code);
        if (matches.Count == 0) {
            // method has no update
            return;
        }
        int update_start = LinesUntil(code, matches[0].Index);
        int update_end = LinesUntil(code, CodeBlockEnd(code, matches[0].Index));

        // is findwithtag in Update?
        rx = new Regex(call);
        matches = rx.Matches(code);
        foreach (Match match in matches) {
            int ln = LinesUntil(code, match.Index);
            if (ln < update_start || ln > update_end) continue;
            Debug.LogError(string.Format("Illegal Usage of {0} in Line: {1}. Do not use it in Update Methods", match.Value, ln), asset);
        }
    }


    /// <summary>
    /// This rule throws errors if GameTags are used as plain strings.
    /// </summary>
    private static void FindWithTag_no_constants(MonoScript asset, string code) {
        Regex rx = null;
        MatchCollection matches = null;


        rx = new Regex("(FindGameObject(s)?WithTag(\\s)?(\\((\\\"(.)*\\\")\\)))");
        matches = rx.Matches(code);
        foreach (Match match in matches){
            int ln = LinesUntil(code, match.Index);
            Debug.LogError(string.Format("Illegal Usage of {0} in Line: {1}. Use `GameTag.<TagName>` instead of a String.", match.Value, ln), asset);
        }

    }
}
