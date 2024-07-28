using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potral : MonoBehaviour
{
    public int index;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.UsePotral(index);
        }
    }
}
