using System;
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
    private GameObject[] cameras;

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

        GameObject cameraGroup = GameObject.Find("Cameras");
        cameras = new GameObject[cameraGroup.transform.childCount];

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i] = cameraGroup.transform.GetChild(i).gameObject;
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
    public void ToCombat()
    {
        nextScene = "Combat";
        StartCoroutine(LoadNextScene());
    }
    public void Win()
    {
        nextScene = "Win";
        StartCoroutine(LoadNextScene());
    }
     public void Lose()
    {
        nextScene = "Lose";
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        transitionAnimation.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
        transitionAnimation.SetTrigger("Start");
        foreach (GameObject cam in cameras)
        {
            if (cam.name == nextScene)
            {
                cam.SetActive(true);
            }
            else
            {
                cam.SetActive(false);
            }
        }
    }
}
