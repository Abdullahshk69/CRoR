using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] private float Speed;
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
