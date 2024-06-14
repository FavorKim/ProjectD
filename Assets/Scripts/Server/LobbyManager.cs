using Mirror;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkManager
{
    public void Login()
    {
        StartClient();
        SceneManager.LoadScene("MainScene");
    }
}
