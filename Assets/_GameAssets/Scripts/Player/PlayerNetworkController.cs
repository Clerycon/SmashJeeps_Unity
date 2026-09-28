using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _playerCamera;
    
    private PlayerController _playerController;
    private PlayerSkillController _playerSkillController;
    private PlayerInteractionController _playerInteractionController;
    public override void OnNetworkSpawn()
    {
        _playerCamera.gameObject.SetActive(IsOwner);

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
}
