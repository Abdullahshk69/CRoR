using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private SpawnComponent spawnComponent;
    private float speedX, speedY;
    private Rigidbody2D rb;

    void Start()
    {
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
