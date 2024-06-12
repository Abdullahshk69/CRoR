using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEnemy : MonoBehaviour
{
    Collider2D objectCollider;
    private void Start()
    {
        objectCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (objectCollider.OverlapPoint(mousePosition))
            {
                BattleSystem.instance.SelectEnemy(this.GetComponent<Unit>());
            }
        }
    }
}
