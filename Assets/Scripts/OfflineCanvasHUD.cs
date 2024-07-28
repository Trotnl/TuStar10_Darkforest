using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class OfflineCanvasHUD : MonoBehaviour
{
    private Button hostBtn, clientBtn;

    private void Start()
    {
        hostBtn = transform.Find("/Canvas/ButtonHost").GetComponent<Button>();
        clientBtn = transform.Find("/Canvas/ButtonClient").GetComponent<Button>();
        hostBtn.onClick.AddListener(OnClickHostBtn);
        clientBtn.onClick.AddListener(OnClickClientBtn);
    }

    private void OnClickHostBtn()
    {
        NetworkManager.singleton.StartHost();
    }

    private void OnClickClientBtn()
    {
        NetworkManager.singleton.StartClient();
    }
}
