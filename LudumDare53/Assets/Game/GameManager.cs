using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    Guid DropZoneId { get; }

    bool IsCarEmpty();

    void SubscribeDropZone(Guid id);

    void DefineNextDropZone();

    float GetSpeed();

    Vector3 GetFlatDirection();
}

public class GameManager : MonoBehaviour, IGameState
{
    [SerializeField] TukTukController playerController;

    [SerializeField] GPS GPS;

    private static GameManager instance;
    internal static IGameState GetGameState()
    {
        return instance;
    }

    public void SubscribeDropZone(Guid id) => dropZones.Add(id);

    public void DefineNextDropZone()
    {
        var index = UnityEngine.Random.Range(0, dropZones.Count);
        var selectedGuid = dropZones[index];
        DropZoneId = selectedGuid;
        print("New drop zone id: " + DropZoneId);
    }

    private List<Guid> dropZones = new List<Guid>();

    public Guid DropZoneId { get; private set; }

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

    public float GetSpeed()
    {
        return playerController.GetSpeed();
    }

    public Vector3 GetFlatDirection()
    {
        var direction = playerController.GetDirection();
        direction.y = 0;
        return direction;
    }
}
