using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundles : MonoBehaviour
{
    //Creates a new item (Strict Mode) in the new Build Asset Bundles menu
    [MenuItem("Asset Bundles/Build - Strict Mode ")]
    static void BuildABsStrict()
    {
        //Build the AssetBundles in strict mode (build fails if any errors are detected)
        BuildPipeline.BuildAssetBundles("Assets/AssetBundleExport", BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
    }
}
