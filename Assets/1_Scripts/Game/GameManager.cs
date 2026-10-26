using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Config")]
    [SerializeField] public  bool debugMode = false;

    private void Start()
    {
        GameDebugger.debugMode = debugMode;
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadThisScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public void ExitGame()
    {
        Application.Quit(); 
    }

    private IEnumerator LoadScene(string sceneName)
    {
        if (GameDebugger.debugMode) Debug.Log("Loading " + sceneName);

        SceneManager.LoadSceneAsync(sceneName);

        yield return null;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
    }
}
