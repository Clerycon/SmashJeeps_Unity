using System;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;
    private PlayerController _playerController;
    private PlayerHealthController _playerHealthController;

    private bool _isCrashed;
    private bool _isShieldActive;
    private bool _isSpikeActive;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerController = GetComponent<PlayerController>();
        _playerHealthController = GetComponent<PlayerHealthController>();

        _playerController.OnVehicleCrashed += PlayerController_OnVehicleCrashed;
    }

    private void PlayerController_OnVehicleCrashed()
    {
        _isCrashed = true;
        enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckCollision(other);
    }

    private void CheckCollision(Collider other)
    {
        if(!IsOwner) { return; }
        if(_isCrashed) { return; }
        if(GameManager.Instance.GetGameState() != GameState.Playing) { return; }

        CheckCollectibleCollision(other);
        CheckDamageableCollision(other);
    }


    private void CheckCollectibleCollision(Collider other)
    {
        if(other.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect(_playerSkillController);
        }
    }

    private void CheckDamageableCollision(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            if (_isShieldActive)
            {
                Debug.Log("Shield Active: Damage Blocked");
                return;
            }

            CrashTheVehicle(damageable);   
        }
    }

    private void CrashTheVehicle(IDamageable damageable)
    {
        damageable.Damage(_playerController);
        _playerHealthController.TakeDamage(damageable.GetDamageAmount());
        SetKillerUIRpc(damageable.GetKillerClientId(), 
            RpcTarget.Single(damageable.GetKillerClientId(), RpcTargetUse.Temp));
        SpawnerManager.Instance.RespawnPlayer(damageable.GetRespawnTimer(), OwnerClientId);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetKillerUIRpc(ulong killerClientId, RpcParams rpcParams)
    {
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI("Clerycon");
        }
    }

    public void OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
        _playerHealthController.RestartHealth();
    }

    public void SetShieldActive(bool active) => _isShieldActive = active;
    public void SetSpikeActive(bool active) => _isSpikeActive = active;  
}
