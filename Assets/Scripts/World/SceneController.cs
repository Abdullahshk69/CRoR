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
        Invoke(nameof(PlayerMovement), 1f);
        nextScene = "TopFloor - Hallway";
        AudioManager.instance.changeMusic("worldBgm");
        StartCoroutine(LoadNextScene());
    }
    public void ToStairs()
    {
        Invoke(nameof(PlayerMovement), 1f);
        nextScene = "TopFloor - Stairs";
        AudioManager.instance.changeMusic("worldBgm");
        StartCoroutine(LoadNextScene());
    }
    public void ToThroneRoom()
    {
        Invoke(nameof(PlayerMovement), 1f);
        nextScene = "TopFloor -Throne Room";
        AudioManager.instance.changeMusic("worldBgm");
        StartCoroutine(LoadNextScene());
    }
    public void ToCombat()
    {
        PlayerController.instance.OnLoadCombat();
        nextScene = "Combat";
        AudioManager.instance.changeMusic("combatBgm");
        StartCoroutine(LoadNextScene());
    }
    public void Win()
    {
        nextScene = "Win";
        AudioManager.instance.changeMusic("winBgm");
        StartCoroutine(LoadNextScene());
    }
     public void Lose()
    {
        nextScene = "Lose";
        AudioManager.instance.changeMusic("loseBgm");
        StartCoroutine(LoadNextScene());
    }

    public void PlayerMovement()
    {
        PlayerController.instance.OnLoadScene();
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

    public void SceneLoaded()
    {
        if(scene!="Combat")
        {
            PlayerController.instance.OnLoadScene();
        }
    }
}
