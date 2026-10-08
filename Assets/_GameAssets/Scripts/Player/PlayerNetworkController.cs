using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _playerCamera;

    [SerializeField] private TMP_Text _playerNameText;
    
    private PlayerController _playerController;
    private PlayerSkillController _playerSkillController;
    private PlayerInteractionController _playerInteractionController;

    private NetworkVariable<FixedString32Bytes> _playerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        _playerCamera.gameObject.SetActive(IsOwner);

        if (IsServer)
        {
            UserData userData = HostSingleton.Instance.HostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            _playerName.Value = userData.UserName;
            SetPlayerNameRpc();
        }

        if(!IsOwner) { return; }

        _playerController = GetComponent<PlayerController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
    }

    public void OnPlayerRespawned()
    {
        _playerController.OnPlayerRespawned();
        _playerSkillController.OnPlayerRespawned();
        _playerInteractionController.OnPlayerRespawned();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerNameRpc()
    {
        _playerNameText.text = _playerName.Value.ToString();
    }
}
