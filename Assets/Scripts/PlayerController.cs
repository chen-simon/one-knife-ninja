using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController controller;

    int horizontal = 0;
    bool flipped;
    public bool dead;

    public bool grounded;
    bool controllable = true;
    public bool hiding = false;

    public int airJumps;
    public int maxAirJumps = 2;

    public float speed = 5;
    public float jumpSpeed = 5;
    public float fallSpeed = 1.8f;
    public float fastFallSpeed = 2.7f;

    public float startup = 0.5f;
    public float recovery = 0.1f;

    public float throwingSpeed = 1;

    public bool hasKnife = true;

    public GameObject knife;

    public GameObject interactable;
    public GameObject indicator;
    public SpriteRenderer vignette;
    public GameObject knifeIndicator;

    public AudioClip jumpSound;
    public AudioClip pickupSound;
    public AudioClip deathSound;

    float targetVignetteAlpha = 0.42f;

    Rigidbody2D rb;
    SpriteRenderer spr;
    Animator anim;
    AudioSource sound;

    public Text text;
    public Image image;

    public float fadeTime;

    void Awake()
    {
        //Singleton
        if (!controller) { controller = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {

        horizontal = 0;
        if (Input.GetKey(InputManager.manager.right)) { horizontal += 1; }
        else if (Input.GetKey(InputManager.manager.left)) { horizontal -= 1; }
        if (controllable && !GameManager.manager.paused)
        {
            //Movement
            if (horizontal != 0)
            {
                Vector2 shift = transform.position;
                shift.x += horizontal * speed * Time.deltaTime;
                transform.position = shift;

                if (horizontal == -1) { flipped = true; }
                else { flipped = false; }

                anim.SetBool("Running", true);
            }
            else { anim.SetBool("Running", false); }

            //if (spr.flipX != flipped) { anim.SetTrigger("Turn"); }
            spr.flipX = flipped;
        }
    }

    void Update()
    {
        Vector2 vel = rb.velocity;

        if (controllable && !GameManager.manager.paused)
        {
            //Fast Falling
            if (vel.y < 0)
            {
                anim.SetBool("Falling", true);
                if (Input.GetKeyDown(InputManager.manager.down))
                {
                    rb.gravityScale = fastFallSpeed;
                }
            }

            if (Input.GetKeyUp(InputManager.manager.down))
            {
                rb.gravityScale = fallSpeed;
            }

            //Jumping
            if (Input.GetKeyDown(InputManager.manager.jump) && !GameManager.manager.paused)
            {
                sound.clip = jumpSound;
                rb.gravityScale = fallSpeed;
                if (grounded)
                {
                    vel.y = jumpSpeed;
                    anim.SetBool("Falling", false);
                    sound.Play();
                }
                else if (airJumps > 0)
                {
                    airJumps -= 1;
                    vel.y = jumpSpeed;
                    anim.SetBool("Falling", false);
                    sound.Play();
                }
            }
            if (vel.y < -12) { vel.y = -12; }
            rb.velocity = vel;

            //Attacking
            if (Input.GetMouseButtonDown(0) && hasKnife)
            {
                StartCoroutine("Startup");
            }

            //Interacting
            if (Input.GetMouseButtonDown(1) && interactable)
            {
                //Hiding
                if (interactable.tag == "Hideable")
                {
                    transform.position = interactable.transform.position;
                    rb.velocity = Vector2.zero;
                    hiding = true;
                    targetVignetteAlpha = 1f;
                    spr.enabled = false;
                    controllable = false;
                }
                else if (interactable.tag == "Sign")
                {
                    image.sprite = interactable.GetComponent<Sign>().image;
                    text.text = interactable.GetComponent<Sign>().text;
                    image.color = Color.white; text.color = Color.white;
                    StartCoroutine("TextFade");
                }
            }
        }

        //Unhide
        else if (Input.GetMouseButtonDown(1) && hiding)
        {
            hiding = false;
            controllable = true;
            spr.enabled = true;
            targetVignetteAlpha = 0.42f;
        }
    }

    private void LateUpdate()
    {
        //Interaction
        if (interactable)
        {
            indicator.SetActive(true);
            indicator.transform.position = interactable.transform.position + new Vector3(0, 1, 0);
        }
        else
        {
            indicator.SetActive(false);
        }

        //Vignette Blending
        if (vignette.color.a != targetVignetteAlpha)
        {
            Color temp = vignette.color;
            temp.a = Mathf.Lerp(temp.a, targetVignetteAlpha, 0.05f);
            vignette.color = temp;
        }
    }

    //Collision Detection
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            airJumps = maxAirJumps;
            rb.gravityScale = fallSpeed;
            anim.SetBool("Falling", false);
            anim.SetBool("inAir", false);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            grounded = true;
            anim.SetBool("Falling", false);
            anim.SetBool("inAir", false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            grounded = false;
            anim.SetBool("inAir", true);
        }
    }

    public void Die(int dir)
    {
        if (!dead)
        {
            anim.SetTrigger("Dead");
            controllable = false;
            StopAllCoroutines();
            if (hiding)
            {
                hiding = false;
                spr.enabled = true;
                targetVignetteAlpha = 0.42f;
            }
            GameManager.manager.GameOver();
            rb.velocity = rb.velocity + new Vector2(dir * 3, 0);
            dead = true;
            sound.clip = deathSound;
            sound.Play();
        }
    }

    IEnumerator Startup()
    {
        controllable = false;
        anim.SetTrigger("Attacking");

        //Throw Direction
        //Directional Input
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(InputManager.manager.up)) { direction.y += 1; }
        if (Input.GetKey(InputManager.manager.down)) { direction.y -= 1; }
        if (Input.GetKey(InputManager.manager.right)) { direction.x += 1; }
        if (Input.GetKey(InputManager.manager.left)) { direction.x -= 1; }

        if (direction.y == 0 && direction.x == 0)
        {
            if (!flipped) { direction.x = 1; }
            else { direction.x = -1; }
        }

        yield return new WaitForSeconds(startup);

        //Knife Throwing
        GameObject weapon = Instantiate(knife, transform.position, Quaternion.identity) as GameObject;
        weapon.GetComponent<Rigidbody2D>().velocity = direction * throwingSpeed;
        hasKnife = false;
        knifeIndicator.SetActive(false);

        StartCoroutine("Recovery");
    }

    IEnumerator Recovery()
    {
        yield return new WaitForSeconds(recovery);
        controllable = true;
    }

    IEnumerator TextFade()
    {
        for (float time = 0; time < fadeTime; time += Time.deltaTime)
        {
            Color temp = text.color;
            temp.a = Mathf.Cos(time / fadeTime * Mathf.PI / 2);
            text.color = temp;
            image.color = temp;
            yield return new WaitForEndOfFrame();
        }
        text.color = new Color(1, 1, 1, 0);
        image.color = text.color;
    }

    public void GetKnife()
    {
        hasKnife = true;
        knifeIndicator.SetActive(true);
        sound.clip = pickupSound;
        sound.Play();
    }
}
