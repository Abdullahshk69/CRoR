using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VFXController : MonoBehaviour
{
    public static VFXController instance;
    [SerializeField] Animator vfxAnimation;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void moveVFX(Transform target)
    {
        transform.position = target.position;
    }

    public void playVFX(int hit)
    {
        StartCoroutine(playHit(hit));
    }

    IEnumerator playHit(int hit)
    {
        vfxAnimation.SetInteger("VFX Type", hit);
        yield return new WaitForSeconds(0.4f);
        vfxAnimation.SetInteger("VFX Type", 0);
    }
}
