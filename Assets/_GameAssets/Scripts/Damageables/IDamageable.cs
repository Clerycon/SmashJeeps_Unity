public interface IDamageable
{
    void Damage(PlayerController playerController);
    ulong GetKillerClientId();
    int GetRespawnTimer();
    int GetDamageAmount();
}
