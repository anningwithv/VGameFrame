using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace VGameFrame
{
    public static class ABMenuItem
    {
        [MenuItem("Assets/VGameFrame/ABBundle/Apply Rule/Directory", false, 1)]
        private static void ApplyRuleDir()
        {
            var rules = ABBuilder.GetBuildRules();
            AddRulesForSelection(rules, rules.searchPatternDir);
        }

        [MenuItem("Assets/VGameFrame/ABBundle/BuildAssetBundles")]
        private static void BuildAssetBundles()
        {
            var watch = new Stopwatch();
            watch.Start();
            ABBuilder.ApplyBuildRules();
            ABBuilder.BuildAssetBundles();
            watch.Stop();
            UnityEngine.Debug.Log("BuildAssetBundles " + watch.ElapsedMilliseconds + " ms.");
        }

        private static void AddRulesForSelection(ABBuildRules rules, string searchPattern)
        {
            var isDir = rules.searchPatternDir.Equals(searchPattern);
            foreach (var item in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(item);
                var rule = new BuildRule
                {
                    searchPath = path,
                    searchPattern = searchPattern,
                    nameBy = isDir ? NameBy.Directory : NameBy.Path
                };
                ArrayUtility.Add(ref rules.rules, rule);
            }

            EditorUtility.SetDirty(rules);
            AssetDatabase.SaveAssets();
        }

    }
}