public interface IPlayerState
{
    void Enter(Player player);
    void Update(Player player);
    void FixedUpdate(Player player);  // 물리 로직
    void Exit(Player player);
}
