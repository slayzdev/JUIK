using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        string[] scenes = new string[] { EditorSceneManager.GetActiveScene().path };
        string buildPath = "Builds/Windows/IFP.exe";
        
        if (!System.IO.Directory.Exists("Builds/Windows"))
        {
            System.IO.Directory.CreateDirectory("Builds/Windows");
        }
        
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
} 