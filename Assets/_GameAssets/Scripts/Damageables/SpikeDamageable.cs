using Unity.Netcode;
using UnityEngine;

public class SpikeDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkill;
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
        KillScreenUI.Instance.SetSmashedUI("Clerycon", _mysteryBoxSkill.SkillData.RespawnTimer);
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

    public ulong GetKillerClientId()
    {
        return OwnerClientId;
    }
    public int GetRespawnTimer()
    {
        return _mysteryBoxSkill.SkillData.RespawnTimer;
    }

    public int GetDamageAmount()
    {
        return _mysteryBoxSkill.SkillData.DamageAmount;
    }
}
