using System.Collections;
using UnityEngine;
using Mirror;

public class OurNetworkManager : NetworkManager
{
    private int playerIndex = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // �����Ԥ����������ѡ���Ӧ��Ԥ����
        GameObject playerPrefab = spawnPrefabs[playerIndex++];
        Debug.Log(playerPrefab);

        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log(playerPrefab);
    }
}
