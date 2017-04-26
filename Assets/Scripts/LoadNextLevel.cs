using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{

    [SerializeField]
    private string nextScene;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Player")
        {
            ClearConsole();
            if (nextScene == "PCGLevel")
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
            else
                SceneManager.LoadScene(nextScene);
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
