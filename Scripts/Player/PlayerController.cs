using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnAmbushed;
    public float moveAgility;
    public LayerMask battlezoneLayer;
    public LayerMask solidObjectsLayer;

    private Animator animator;
    private bool currentlyMoving;
    private Vector2 input;

   
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!currentlyMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // remove diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (CanWalkOn(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("currentlyMoving", currentlyMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        currentlyMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveAgility * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        currentlyMoving = false;

        CheckForAmbush();
    }

    private bool CanWalkOn(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos - new Vector3 (0f, 0f), 0f, solidObjectsLayer) != null)
        {
            return false;
        }

            return true;
    }

    private void CheckForAmbush()
    {
        if (Physics2D.OverlapCircle(transform.position - new Vector3 (0f, 0.2f), 0.2f, battlezoneLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 8)
            {
                animator.SetBool("currentlyMoving", false);
                OnAmbushed();
            }
        }
    }
}
