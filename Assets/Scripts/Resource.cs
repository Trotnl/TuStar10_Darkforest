using UnityEngine;


public class Resource : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 获取 Player 组件并增加得分
            Player player = other.GetComponent<Player>();
            //player.CmdIncreaseScore(1); // 每次增加1分
            player.GetResource(1);
            Destroy(gameObject);
        }
    }
    
    //[Command(requiresAuthority = false)]
    //void CmdDestroyResource()
    //{
    //    Debug.Log("xx  CmdDestroyResource!");
    //    RpcDestroyResource();
    //}

    //[ClientRpc]
    //void RpcDestroyResource()
    //{
    //    Debug.Log("xx   Destroyed!");
    //    Destroy(gameObject);
    //}
    
}
