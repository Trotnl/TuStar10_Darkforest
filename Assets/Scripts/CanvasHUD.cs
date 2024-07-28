using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHUD : MonoBehaviour
{
    public GameObject panelStart;
    public GameObject panelStop;
    public Button btnHost, btnServer, btnClient, btnStop;

    public TMP_InputField inputFieldAddress;

    public TextMeshProUGUI serverText;
    public TextMeshProUGUI clientText;

    private void Start()
    {
        // Update the canvas text if you have manually changed network managers address from the game object before starting the game scene
        if (NetworkManager.singleton.networkAddress != "localhost")
        {
            inputFieldAddress.text = NetworkManager.singleton.networkAddress;
        }

        // Adds a listener to the main input field and invokes a method when the value changes.
        inputFieldAddress.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        // Make sure to attach these Buttons in the Inspector
        btnHost.onClick.AddListener(BtnHost);
        btnServer.onClick.AddListener(BtnServer);
        btnClient.onClick.AddListener(BtnClient);
        btnStop.onClick.AddListener(BtnStop);

        // This updates the Unity canvas, we have to manually call it every change, unlike legacy OnGUI.
        SetupCanvas();
    }

    //Invoked when the value of the text field changes.
    public void ValueChangeCheck()
    {
        NetworkManager.singleton.networkAddress = inputFieldAddress.text;
    }

    public void BtnHost()
    {
        NetworkManager.singleton.StartHost();
        SetupCanvas();
    }

    public void BtnServer()
    {
        NetworkManager.singleton.StartServer();
        SetupCanvas();
    }

    public void BtnClient()
    {
        NetworkManager.singleton.StartClient();
        SetupCanvas();
    }

    public void BtnStop()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        SetupCanvas();
    }

    public void SetupCanvas()
    {
        // Here we will dump majority of the canvas UI that may be changed.

        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                panelStart.SetActive(false);
                panelStop.SetActive(true);
                clientText.text = "Connecting to " + NetworkManager.singleton.networkAddress + "..";
            }
            else
            {
                panelStart.SetActive(true);
                panelStop.SetActive(false);
            }
        }
        else
        {
            panelStart.SetActive(false);
            panelStop.SetActive(true);

            // server / client status message
            if (NetworkServer.active)
            {
                serverText.text = "Server: active. Transport: " + Transport.active;
                // Note, older mirror versions use: Transport.activeTransport
            }
            if (NetworkClient.isConnected)
            {
                clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
            }
        }
    }
}