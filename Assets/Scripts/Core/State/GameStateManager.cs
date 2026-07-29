using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private BaseGameState initialState;
    private BaseGameState currentState;
    
    // Lưu trữ danh sách các state có sẵn trong scene
    private Dictionary<string, BaseGameState> states = new Dictionary<string, BaseGameState>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (initialState != null)
        {
            ChangeState(initialState);
        }
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    public void RegisterState(string stateName, BaseGameState state)
    {
        if (!states.ContainsKey(stateName))
        {
            states.Add(stateName, state);
        }
    }

    public void ChangeState(string stateName)
    {
        if (states.TryGetValue(stateName, out BaseGameState nextState))
        {
            ChangeState(nextState);
        }
        else
        {
            Debug.LogError("Không tìm thấy State: " + stateName);
        }
    }

    private void ChangeState(BaseGameState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;
        currentState.EnterState();
    }
}
