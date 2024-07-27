using UnityEngine;
using Mirror;

public class Resource : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.name + " ��������");

            // ��ȡ Player ��������ӵ÷�
            Player player = other.GetComponent<Player>();
            if (player != null && player.isServer)
            {
                player.CmdIncreaseScore(1); // ÿ������1��
            }

            // CmdDestroyResource();
            Destroy(gameObject);

            

        }
    }

    /*
    [Command]
    void CmdDestroyResource()
    {
        Debug.Log("xx  CmdDestroyResource!");
        RpcDestroyResource();
    }

    [ClientRpc]
    void RpcDestroyResource()
    {
        Debug.Log("xx   Destroyed!");
        Destroy(gameObject);
    }
    */
}
