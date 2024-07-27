using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Torch : NetworkBehaviour
{
    public GameObject owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocalPlayer) return;

        Player player = collision.GetComponent<Player>();
        if (player != null && player.gameObject == owner)
        {
            return;
        }
        if (player != null)
        {
            // ����Ǳ��ˣ�ʩ��һ�����շ������
            owner.GetComponent<Player>().Attack(collision.gameObject);
        }
    }
}
