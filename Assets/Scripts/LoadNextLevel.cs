using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{

    [SerializeField]
    private string nextScene;

    private Scene activeScene;

    private void Awake()
    {
        activeScene = SceneManager.GetActiveScene();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
#if UNITY_EDITOR
            ClearConsole();
#endif
            if (nextScene == "PCGLevel")
            {
                //Scene scene = SceneManager.GetActiveScene();
                //SceneManager.LoadScene(scene.name);
                SceneManager.LoadScene(activeScene.buildIndex + 1);
            }
            else
            {
                int nextBuildScene = SceneManager.GetActiveScene().buildIndex + 1;

                SceneManager.LoadScene(nextBuildScene);
            }
        }
    }

    static void ClearConsole()
    {
        // This simply does "LogEntries.Clear()" the long way:
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
}
