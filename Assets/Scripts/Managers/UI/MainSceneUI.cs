using Mirror;
using UnityEngine;

public class MainSceneUI : NetworkBehaviour
{
    [SerializeField] GameObject SurvivorPrefab;
    [SerializeField] GameObject KillerPrefab;
    public void OnClick_KillerStart()
    {
        NetworkManager.singleton.playerPrefab = KillerPrefab;
        NetworkManager.singleton.StartHost();
    }
    public void OnClick_SurvivorStart()
    {
        NetworkManager.singleton.playerPrefab = SurvivorPrefab;
        NetworkManager.singleton.StartClient();
    }
}
