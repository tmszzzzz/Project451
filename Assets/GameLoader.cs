using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;
    

    public bool loadingAnExistingGame = false;
    public string loadFilePath = "";
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void newGame()
    {
        loadingAnExistingGame = false;
        SceneManager.LoadScene(1,LoadSceneMode.Single);
    }

    public void openAGame()
    {
        loadingAnExistingGame = true;
        SceneManager.LoadScene(1,LoadSceneMode.Single);
    }
}
