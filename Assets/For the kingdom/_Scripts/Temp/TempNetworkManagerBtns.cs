using Unity.Netcode;
using UnityEngine;

public class TempNetworkManagerBtns : MonoBehaviour
{
    public void Server()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }
}
