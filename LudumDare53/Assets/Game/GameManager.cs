using UnityEngine;

public interface IGameState
{
    bool IsCarEmpty();
}

public class GameManager : MonoBehaviour, IGameState
{
    [SerializeField] TukTukController playerController;

    private static GameManager instance;
    internal static IGameState GetGameState()
    {
        return instance;
    }

    public bool IsCarEmpty()
    {
        return playerController.IsEmpty;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
