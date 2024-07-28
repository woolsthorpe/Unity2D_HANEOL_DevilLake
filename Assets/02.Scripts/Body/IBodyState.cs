public interface IBodyState
{
    void Enter(Body body);
    void Update(Body body);
    void FixedUpdate(Body body);  // 물리 로직
    void Exit(Body body);
}