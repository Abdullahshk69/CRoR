using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] Rigidbody2D rb;

    public void Move(InputAction.CallbackContext context)
    {
        rb.velocity = context.ReadValue<Vector2>() * moveSpeed;
    }
}
