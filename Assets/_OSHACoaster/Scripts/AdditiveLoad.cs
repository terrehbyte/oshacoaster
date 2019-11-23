using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class AdditiveLoad : MonoBehaviour
{
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying) { return; }

        if(Application.isEditor)
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(sceneName, UnityEditor.SceneManagement.OpenSceneMode.Additive);
#endif
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.F5))
        {
            SceneManager.LoadScene("Splash-Menu", LoadSceneMode.Single);
        }
    }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Scenes/Reload Scene")]
    public static void ReloadScene()
    {
        if (Application.isPlaying) { return; }

        if (Application.isEditor)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path);
        }
    }
#endif
}
