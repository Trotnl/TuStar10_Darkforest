using System.Collections;
using UnityEngine;
using Mirror;

public class OurNetworkManager : NetworkManager
{
    private int playerIndex = 0;
    private bool used = false;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player;

        if (playerPrefab != null && used == false)
        {
            player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
            used = true;
        } 
        else
        {
            // 从玩家预制体数组中选择对应的预制体
            GameObject spawnPrefab = spawnPrefabs[playerIndex++];

            player = startPos != null
            ? Instantiate(spawnPrefab, startPos.position, startPos.rotation)
            : Instantiate(spawnPrefab);

            player.name = $"{spawnPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
        }
    }
}
