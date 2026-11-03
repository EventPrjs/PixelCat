using System;
using System.IO;
using Schoolmakerdays;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WorkshopBuild
{
    private const string ScenePath = "Assets/EndlessPixelGrid/EndlessPixelGrid.unity";

    [MenuItem("Schoolmakerdays/Prepare workshop scene")]
    public static void Prepare()
    {
        VerifyModel();
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Branding" }))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
            importer.textureType = TextureImporterType.Default;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        if (!File.Exists(ScenePath))
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var controller = new GameObject("EndlessPixelGrid").AddComponent<EndlessPixelGrid>();
            controller.CreateLayout();
            EditorSceneManager.SaveScene(scene, ScenePath);
        }
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        PlayerSettings.companyName = "Schoolmakerdays";
        PlayerSettings.productName = "Endless Pixelgrid";
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.runInBackground = false;
        PlayerSettings.WebGL.template = "PROJECT:Schoolmakerdays";
        // GitHub Pages serves these files without custom Content-Encoding headers.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.dataCaching = false;
        PlayerSettings.WebGL.initialMemorySize = 64;
        PlayerSettings.WebGL.maximumMemorySize = 512;
        AssetDatabase.SaveAssets();
        Debug.Log("WORKSHOP: scene prepared and model checks passed.");
    }

    [MenuItem("Schoolmakerdays/Build WebGL for GitHub Pages")]
    public static void BuildWebGL()
    {
        Prepare();
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { ScenePath }, locationPathName = "docs", target = BuildTarget.WebGL,
            options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("WebGL build failed: " + report.summary.result);
        File.WriteAllText("docs/.nojekyll", "");
        Debug.Log("WORKSHOP: WebGL build succeeded: " + report.summary.totalSize + " bytes.");
    }

    [MenuItem("Schoolmakerdays/Verify FIFO and live input model")]
    public static void VerifyModel()
    {
        var model = new PixelGridModel();
        model.InputLine[0] = true;
        model.InputLine[6] = true;
        model.Commit();
        Require(model.Grid[9, 0] && model.Grid[9, 6], "Q6/Q0 ordering");
        Require(model.InputLine[0] && model.InputLine[6], "contacts survive commit");
        model.ClearInput();
        model.InputLine[3] = true;
        model.Commit();
        Require(model.Grid[8, 0] && model.Grid[8, 6] && model.Grid[9, 3], "FIFO shift/copy isolation");
        for (int i = 0; i < 9; i++) { model.ClearInput(); model.Commit(); }
        Require(!model.Grid[0, 0] && model.Grid[0, 3], "oldest row discarded after eleven commits");
        model.InputLine[2] = true;
        model.ResetGrid();
        foreach (bool value in model.Grid) Require(!value, "reset empties full raster");
        Require(model.InputLine[2], "reset preserves live contact");
        model.ClearInput();
        foreach (bool value in model.InputLine) Require(!value, "focus loss clears live inputs");
    }

    private static void Require(bool condition, string label)
    {
        if (!condition) throw new Exception("Model check failed: " + label);
    }
}
