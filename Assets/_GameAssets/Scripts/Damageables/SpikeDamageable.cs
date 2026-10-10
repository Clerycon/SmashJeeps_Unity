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
    public void Damage(PlayerController playerController, string playerName)
    {
        playerController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI(playerName, _mysteryBoxSkill.SkillData.RespawnTimer);
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

    public string GetKillerName()
    {
        ulong killerClientId = GetKillerClientId();
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            string playerName = killerClient.PlayerObject.GetComponent<PlayerNetworkController>().PlayerName.Value.ToString();
            return playerName;
        }

        return string.Empty;
    }
}
