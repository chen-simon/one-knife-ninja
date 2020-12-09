using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    Animator anim;
    public SpriteRenderer highlight;
    public GameObject linkedDoor;
    Color color;

    bool broken = false;

    void Start()
    {
        color = highlight.color;
        anim = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (!broken)
        {
            color.a = ((Mathf.Cos(Time.time) + 1) / 8) + 0.1f;
            highlight.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Knife" && !broken)
        {
            Break();
            broken = true;
        }
    }

    void Break()
    {
        anim.SetTrigger("Break");
        highlight.gameObject.SetActive(false);
        linkedDoor.GetComponent<Door>().Open();
        GetComponent<AudioSource>().Play();
    }
}
