using Unity.Netcode;
using UnityEngine;

public class RocketDamageable : NetworkBehaviour, IDamageable
{
    public void Damage(PlayerController playerController)
    {
        playerController.CrashVehicle();
        DestroyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }
}
