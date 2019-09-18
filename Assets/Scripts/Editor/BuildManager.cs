using UnityEditor;
using System.Diagnostics;
using UnityEngine;

public class BuildManager
{
    [MenuItem("Custom Tools/Build Tools/Full Build")]
    public static void FullBuild()
    {
        BuildClient();
        BuildServer();
        BuildSpawner();
        BuildGameserver();
    }

    [MenuItem("Custom Tools/Build Tools/Full Build - Server")]
    public static void FullBuild_Server()
    {
        BuildServer();
        BuildSpawner();
        BuildGameserver();
    }

    [MenuItem("Custom Tools/Build Tools/Build Client")]
    public static void BuildClient()
    {
        string _path_ = PlayerPrefs.GetString("clientPath");
        string path = EditorUtility.SaveFolderPanel("Choose build location for Client", _path_, "");
        if (string.IsNullOrEmpty(path))
            return;
        PlayerPrefs.SetString("clientPath", path);
        PlayerPrefs.Save();
        string[] scenes = new string[] { "Assets/Scenes/Client.unity" };
        string _path = path + "/EarthLike.exe";

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = scenes;
        options.locationPathName = _path;
        options.target = BuildTarget.StandaloneWindows;
        options.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(options);

        Process p = new Process();
        p.StartInfo.FileName = path;
        p.Start();
    }

    [MenuItem("Custom Tools/Build Tools/Build Server")]
    public static void BuildServer()
    {
        string _path_ = PlayerPrefs.GetString("serverPath");
        string path = EditorUtility.SaveFolderPanel("Choose build location for Server", _path_, "");
        if (string.IsNullOrEmpty(path))
            return;
        PlayerPrefs.SetString("serverPath", path);
        PlayerPrefs.Save();
        string[] scenes = new string[] { "Assets/Scenes/Server.unity" };
        string _path = path + "/Server.exe";

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = scenes;
        options.locationPathName = _path;
        options.target = BuildTarget.StandaloneWindows;
        options.options = BuildOptions.EnableHeadlessMode | BuildOptions.Development | BuildOptions.StripDebugSymbols;
        BuildPipeline.BuildPlayer(options);

        Process p = new Process();
        p.StartInfo.FileName = path;
        p.Start();
    }

    [MenuItem("Custom Tools/Build Tools/Build Spawner")]
    public static void BuildSpawner()
    {
        string _path_ = PlayerPrefs.GetString("spawnerPath");
        string path = EditorUtility.SaveFolderPanel("Choose build location for Spawner", _path_, "");
        if (string.IsNullOrEmpty(path))
            return;
        PlayerPrefs.SetString("spawnerPath", path);
        PlayerPrefs.Save();
        string[] scenes = new string[] { "Assets/Scenes/Spawner.unity" };
        string _path = path + "/Spawner.exe";

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = scenes;
        options.locationPathName = _path;
        options.target = BuildTarget.StandaloneWindows;
        options.options = BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(options);

        Process p = new Process();
        p.StartInfo.FileName = path;
        p.Start();
    }

    [MenuItem("Custom Tools/Build Tools/Build Game Server")]
    public static void BuildGameserver()
    {
        string _path_ = PlayerPrefs.GetString("gameserverPath");
        string path = EditorUtility.SaveFolderPanel("Choose build location for Game Server", _path_, "");
        if (string.IsNullOrEmpty(path))
            return;
        PlayerPrefs.SetString("gameserverPath", path);
        PlayerPrefs.Save();
        string[] scenes = new string[] { "Assets/Scenes/GameServer.unity" };
        string _path = path + "/Gameserver.exe";

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = scenes;
        options.locationPathName = _path;
        options.target = BuildTarget.StandaloneWindows;
        options.options = BuildOptions.EnableHeadlessMode;
        BuildPipeline.BuildPlayer(options);

        Process p = new Process();
        p.StartInfo.FileName = path;
        p.Start();
    }
}