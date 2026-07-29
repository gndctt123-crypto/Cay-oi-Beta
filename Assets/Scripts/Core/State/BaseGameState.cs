using UnityEngine;

public abstract class BaseGameState : MonoBehaviour
{
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
