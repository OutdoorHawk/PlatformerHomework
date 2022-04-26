using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private static Scene _currentScene;
    

    private void Awake()
    {
        _currentScene = SceneManager.GetActiveScene();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(_currentScene.buildIndex);
        _player.layer = 3;

    }

}
