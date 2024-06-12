using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] Animator transitionAnimation;
    private string scene;
    private string nextScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            scene = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string getScene()
    { 
        return scene;
    }
    public void updateScene()
    {
        scene = SceneManager.GetActiveScene().name;
    }

    public void ToHallway()
    {
        nextScene = "TopFloor - Hallway";
        StartCoroutine(LoadNextScene());
    }
    public void ToStairs()
    {
        nextScene = "TopFloor - Stairs";
        StartCoroutine(LoadNextScene());
    }
    public void ToThroneRoom()
    {
        nextScene = "TopFloor -Throne Room";
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        transitionAnimation.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
        transitionAnimation.SetTrigger("Start");
    }
}
