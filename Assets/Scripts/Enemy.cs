using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1;

    public bool flipped;
    public float startup;

    public bool seen;
    public bool dead;
    public bool hasKnife;
    bool deathFrame;

    public Rigidbody2D rb;
    Animator anim;

    public LayerMask Obstructables;
    public SpriteRenderer alerted;
    public GameObject highlight;

    public AudioClip onHitSound;
    public AudioClip shootingSound;
    public AudioClip alertSound;

    AudioSource sound;

    public SpriteRenderer visionCone;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            if (!seen)
            {
                //Normal Guard Walking Script
                float shift = 0;
                if (flipped) { shift = -1; }
                else { shift = 1; }
                transform.position = transform.position + new Vector3(shift * speed * Time.deltaTime, 0, 0);
            }
        }
    }

    void Update()
    {
        if (!dead)
        {
            if (seen)
            {
                if (PlayerController.controller.transform.position.x > transform.position.x)
                {
                    if (flipped) { Turn(); }
                }
                else if (!flipped) { Turn(); }

                //Alert Exclaimation Point
                if (alerted.color.a != 1)
                {
                    Color temp = alerted.color;
                    temp.a = Mathf.Lerp(temp.a, 1, 0.05f);
                    alerted.color = temp;
                }
            }
        }
    }

    private void LateUpdate()
    {
        deathFrame = false;
        if (!dead)
        {
            Color color = Color.white;
            color.a = ((Mathf.Sin(Time.time) + 1) / 4) + 0.3f;
            visionCone.color = color;
        }
    }

    public void Die()
    {
        deathFrame = true;
        GetComponent<PolygonCollider2D>().enabled = false;
        dead = true;
        anim.SetTrigger("Die");
        sound.clip = onHitSound;
        sound.Play();
        visionCone.gameObject.SetActive(false);
        hasKnife = true;
        highlight.SetActive(true);
        alerted.gameObject.SetActive(false);
    }

    public void Turn()
    {
        if (!flipped)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            flipped = true;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            flipped = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Detect if can be seen
            if (!dead && !PlayerController.controller.hiding && !seen)
            {
                if (!Physics2D.Linecast(transform.position, PlayerController.controller.transform.position, Obstructables))
                {
                    sound.clip = alertSound;
                    sound.Play();
                    StartCoroutine("Shoot");
                }
            }
            else if (dead && hasKnife && !deathFrame)
            {
                PlayerController.controller.GetKnife();
                highlight.SetActive(false);
                alerted.gameObject.SetActive(false);
                anim.SetBool("hasKnife", false);
                hasKnife = false;
                enabled = false;
            }
        }
    }

    IEnumerator Shoot()
    {
        seen = true;
        anim.SetBool("Shooting", true);
        yield return new WaitForSeconds(startup);

        int dir = 1;
        if (PlayerController.controller.transform.position.x < transform.position.x) { dir = -1; }
        if (!dead) {
            PlayerController.controller.Die(dir);
            sound.clip = shootingSound;
            sound.Play();
        }
    }
}
