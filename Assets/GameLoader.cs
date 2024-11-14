using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private static GameLoader instance;
    
    public static  GameLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType< GameLoader >();
            }

            return instance;
        }
        
    }

    private bool loadingAnExistingGame = false;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void openAGame()
    {
        
    }
}
