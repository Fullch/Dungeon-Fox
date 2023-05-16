using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float vel;
    private Vector2 dir;

    private Animator animator;

    [SerializeField]
    Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        takeInput();
        move();
    }

    private void move()
    {
        //transform.Translate(dir * vel * Time.deltaTime);
        rb.velocity = dir * vel;
        setMovAnim(dir);
    }

    private void takeInput()
    {
        dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector2.down;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("spacePress", true);
        }
        else
        {
            animator.SetBool("spacePress", false);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            vel = 8;
        }
        else
        {
            vel = 5;
        }
    }

    private void setMovAnim(Vector2 direccion)
    {
        animator.SetFloat("xDir", direccion.x);
        animator.SetFloat("yDir", direccion.y);
    }
}
