using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public float fallThreshhold;
    Rigidbody2D rb;
    Animator anim;

    public bool fallen;
    bool touching;

    public float highlightAlpha;
    public SpriteRenderer highlight;

    public static float speed(Vector2 velocity)
    {
        float speed = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.y, 2));
        return speed;
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    void Update()
    {
        if (!fallen)
        {
            if (touching && speed(rb.velocity) < fallThreshhold)
            {
                fallen = true;
                anim.SetBool("Fallen", true);
            }
        }
        if (highlight.color.a != highlightAlpha)
        {
            Color temp = highlight.color;
            temp.a = Mathf.Lerp(temp.a, highlightAlpha, 0.03f);
            highlight.color = temp;
        }

        Vector2 vel = rb.velocity;
        if (vel.y < -12) { vel.y = -12; }
        rb.velocity = vel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            touching = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && fallen)
        {
            PlayerController.controller.GetKnife();
            Destroy(gameObject);
        }
        else if (collision.tag == "Ground")
        {
            touching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            touching = false;
        }
    }
}
