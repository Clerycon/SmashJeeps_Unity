using Unity.Netcode;
using UnityEngine;

public class SpikeDamageable : NetworkBehaviour, IDamageable
{
    public void Damage(PlayerController playerController)
    {
        playerController.CrashVehicle();
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
