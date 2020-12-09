using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public Enemy main;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Knife" && !collision.GetComponent<Knife>().fallen && !main.dead)
        {
            main.Die();
            if (collision.GetComponent<Rigidbody2D>().velocity.x > 0) { main.rb.velocity = Vector2.right * 3; }
            else { main.rb.velocity = Vector2.left * 3; }
            Destroy(collision.gameObject);
        }
    }
}
