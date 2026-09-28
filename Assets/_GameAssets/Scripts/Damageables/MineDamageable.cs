using Unity.Netcode;
using UnityEngine;

public class MineDamageable : NetworkBehaviour, IDamageable
{
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetworkObject = client.PlayerObject;
            PlayerController playerController = ownerNetworkObject.GetComponent<PlayerController>();
            playerController.OnVehicleCrashed += PlayerController_OnVehicleCrashed;
        }
    }

    private void PlayerController_OnVehicleCrashed()
    {
        DestroyRpc();
    }
    public void Damage(PlayerController playerController)
    {
        playerController.CrashVehicle();
        DestroyRpc();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out ShieldController shieldController))
        {
            DestroyRpc();
        }    
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) { return; }

        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetworkObject = client.PlayerObject;
            PlayerController playerController = ownerNetworkObject.GetComponent<PlayerController>();
            playerController.OnVehicleCrashed -= PlayerController_OnVehicleCrashed;
        }
    }
}
