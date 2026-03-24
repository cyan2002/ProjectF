using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

//for playtesting or real deal
[InitializeOnLoad]
public static class PlayFromMaster
{
    private static bool skipTitleScreen = false;

    static PlayFromMaster()
    {
        if (skipTitleScreen)
        {
            // Remember the scene the developer had open
            EditorPrefs.SetString("DevStartScene", EditorSceneManager.GetActiveScene().path);
            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Master.unity");
        }
        else
        {
            EditorPrefs.DeleteKey("DevStartScene");
            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/TitleScreen.unity");
        }
    }
}
