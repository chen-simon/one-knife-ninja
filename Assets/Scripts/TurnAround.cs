using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAround : MonoBehaviour
{
    public bool onExit;
    public Enemy main;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onExit && collision.tag == "Ground")
        {
            main.Turn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onExit && collision.tag == "Ground")
        {
            main.Turn();
        }
    }
}
