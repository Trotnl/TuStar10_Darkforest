using UnityEngine;
using Mirror;

public class Resource : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.name + " 碰到了我");

            // 获取 Player 组件并增加得分
            Player player = other.GetComponent<Player>();
            if (player != null && player.isServer)
            {
                player.CmdIncreaseScore(1); // 每次增加1分
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
