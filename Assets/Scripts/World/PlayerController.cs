using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] private float Speed;
    [SerializeField] private Animator anim;
    private float speedX, speedY;
    private Rigidbody2D rb;
    bool isCoroutineCheckEnemyRunning = false;
    bool isInCombat = false;
    [SerializeField] private LayerMask spawnEnemy;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isInCombat)
        {
            speedX = Input.GetAxisRaw("Horizontal") * Speed;
            speedY = Input.GetAxisRaw("Vertical") * Speed;

            //Animator
            anim.SetInteger("Direction", 0);
            if (speedY < 0) //Going down
            {
                anim.SetInteger("Direction", -1);
            }
            else if (speedY > 0) //Going up
            {
                anim.SetInteger("Direction", 1);
            }
            if (speedX < 0) //Going left TEMP!!
            {
                anim.SetInteger("Direction", -1);
            }
            else if (speedX > 0) //Going right TEMP!!
            {
                anim.SetInteger("Direction", -1);
            }

            // moving
            if (speedX != 0 || speedY != 0)
            {
                if (!isCoroutineCheckEnemyRunning)
                {
                    StartCoroutine(CheckEnemyEncounter());
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isInCombat)
        {
            rb.velocity = new Vector2(speedX, speedY);
        }
    }

    public void moveToSpawn(GameObject spawn)
    {
        transform.position = spawn.transform.position;
    }

    IEnumerator CheckEnemyEncounter()
    {
        isCoroutineCheckEnemyRunning = true;
        Debug.Log("Coroutine Called");
        if(Physics2D.OverlapCircle(transform.position, 0.2f, spawnEnemy))
        {
            // Check 
            if (Random.Range(1, 101) <= 10)
            {
                Debug.Log("Combat Triggered");
                LoadEnemyCombat();
            }
        }
        yield return new WaitForSeconds(1);
        isCoroutineCheckEnemyRunning = false;
    }

    private void LoadEnemyCombat()
    {
        SceneController.instance.ToCombat();
    }

    public void OnLoadScene()
    {
        rb.velocity = Vector2.zero;
        isInCombat = false;
    }

    public void OnLoadCombat()
    {
        rb.velocity = Vector2.zero;
        isInCombat = true;
    }
}
