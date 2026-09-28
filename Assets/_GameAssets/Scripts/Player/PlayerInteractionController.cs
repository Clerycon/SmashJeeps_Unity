using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;
    private PlayerController _playerController;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerController = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) { return; }

        if(other.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect(_playerSkillController);
        }

        if(other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(_playerController);
        }
    }
}
