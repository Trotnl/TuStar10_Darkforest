using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject owner;
    public float activeTime = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null && player.gameObject == owner)
        {
            return;
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        activeTime -= Time.deltaTime;
        if (activeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
