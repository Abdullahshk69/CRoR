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
        speedX = Input.GetAxisRaw("Horizontal")* Speed;
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
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2 (speedX, speedY);
    }

    public void moveToSpawn(GameObject spawn)
    {
        transform.position = spawn.transform.position;
    }
}
