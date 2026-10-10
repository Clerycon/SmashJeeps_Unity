public interface IDamageable
{
    void Damage(PlayerController playerController, string playerName);
    ulong GetKillerClientId();
    int GetRespawnTimer();
    int GetDamageAmount();
    string GetKillerName();
}
