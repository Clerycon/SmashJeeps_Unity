using System;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;
    private PlayerController _playerController;

    private bool _isCrashed;
    private bool _isShieldActive;
    private bool _isSpikeActive;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerController = GetComponent<PlayerController>();

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
            damageable.Damage(_playerController);
            SetKillerUIRpc(damageable.GetKillerClientId(), 
                RpcTarget.Single(damageable.GetKillerClientId(), RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetKillerUIRpc(ulong killerClientId, RpcParams rpcParams)
    {
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI("Clerycon");
        }
    }

    public void SetShieldActive(bool active) => _isShieldActive = active;
    public void SetSpikeActive(bool active) => _isSpikeActive = active;  
}
