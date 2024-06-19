using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource vfxSource;

    [SerializeField] private float volume = 0.5f;
    [SerializeField] private float fadeDuration = 1;

    private float time;
    private float currentVolume;

    [Header("BGMs")]
    [SerializeField] private AudioClip worldBgm;
    [SerializeField] private AudioClip combatBgm;
    [SerializeField] private AudioClip winBgm;
    [SerializeField] private AudioClip loseBgm;

    [Header("SFXs")]
    [SerializeField] private AudioClip hitSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        bgmSource.volume = 0;
        currentVolume = volume;

        unmuteMusic();
    }

    public void changeMusic(string track)
    {
        if (track == "worldBgm")
        {
            StartCoroutine(fadeMusic(worldBgm));
        }
        else if (track == "combatBgm")
        {
            StartCoroutine(fadeMusic(combatBgm));
        }
        else if (track == "winBgm")
        {
            StartCoroutine(fadeMusic(winBgm));
        }
        else if (track == "loseBgm")
        {
            StartCoroutine(fadeMusic(loseBgm));
        }

    }

    public void muteMusic()
    {
        currentVolume = 0f;
        time = 0f;
        StartCoroutine(fadeMusic());
    }

    public void unmuteMusic()
    {
        currentVolume = volume;
        time = 0f;
        StartCoroutine(fadeMusic());
    }

    IEnumerator fadeMusic()
    {
        float targetVolume = currentVolume;
        float startVolume = bgmSource.volume;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, time / fadeDuration);
            yield return null;
        }

        yield break;
    }

    IEnumerator fadeMusic(AudioClip nextClip)
    {
        if (nextClip == bgmSource.clip)
        {
            yield break;
        }

        time = 0f;
        float targetVolume = 0f;
        float startVolume = bgmSource.volume;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, time / fadeDuration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = nextClip;
        bgmSource.Play();

        time = 0f;
        targetVolume = volume;
        startVolume = bgmSource.volume;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, time / fadeDuration);
            yield return null;
        }

        yield break;
    }

    public void playHitSound()
    {
        vfxSource.Stop();
        vfxSource.clip = hitSound;
        vfxSource.Play();
    }
}
