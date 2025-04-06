using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayModeManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject _playCamera;
    [SerializeField] private GameObject _menuCamera;
    [SerializeField] private GameObject _gameOverCanvas;

    [Header("Canvases")]
    [SerializeField] private GameObject _playCanvas;
    [SerializeField] private GameObject _menuCanvas;

    public void Play()
    {
        _playCamera.SetActive(true);
        _menuCamera.SetActive(false);

        StartCoroutine(AcivatePlayCanvase());
    }

    public void BackToMenu()
    {
        _menuCamera.SetActive(true);
        _playCamera.SetActive(false);

        StartCoroutine(AcivateMenuCanvase());
    }

    public void Restart()
    {
        string savePath = Application.persistentDataPath;
        string[] saveFiles = Directory.GetFiles(savePath, "*.json");

        foreach (string file in saveFiles)
        {
            File.Delete(file);
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private IEnumerator AcivatePlayCanvase()
    {
        _menuCanvas.SetActive(false);
        yield return new WaitForSeconds(2f);
        _playCanvas.SetActive(true);
    }

    private IEnumerator AcivateMenuCanvase()
    {
        _playCanvas.SetActive(false);
        yield return new WaitForSeconds(2f);
        _menuCanvas.SetActive(true);
    }

    public void AcivateGameOverCanvase()
    {
        _playCanvas.SetActive(false);
        _menuCanvas.SetActive(false);

        _gameOverCanvas.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
