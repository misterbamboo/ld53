using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    event Action BoxCountChanged;
    Guid CurrentZoneId { get; }
    int TotalBoxes { get; }

    bool IsCarEmpty();

    void SubscribeDropZone(Guid id);

    void SubscribeWarehouse(Guid id);

    void DefineNextDropZone();

    void DefineNextWarehouse();

    float GetSpeed();

    Vector3 GetFlatDirection();

    void AddBox();
}

public class GameManager : MonoBehaviour, IGameState
{
    [SerializeField] TukTukController playerController;

    [SerializeField] float timerBeforeFirstCall = 5.0f;

    private static GameManager instance;
    internal static IGameState GetGameState()
    {
        return instance;
    }

    public void SubscribeDropZone(Guid id) => dropZones.Add(id);

    public void SubscribeWarehouse(Guid id) => warehouses.Add(id);

    public void DefineNextDropZone()
    {
        var index = UnityEngine.Random.Range(0, dropZones.Count);
        var selectedGuid = dropZones[index];
        CurrentZoneId = selectedGuid;
    }

    public void DefineNextWarehouse()
    {
        var index = UnityEngine.Random.Range(0, warehouses.Count);
        var selectedGuid = warehouses[index];
        CurrentZoneId = selectedGuid;
    }

    private List<Guid> dropZones = new List<Guid>();

    private List<Guid> warehouses = new List<Guid>();

    public event Action BoxCountChanged;

    public Guid CurrentZoneId { get; private set; }

    public int TotalBoxes { get; private set; }

    public bool IsCarEmpty()
    {
        return playerController.IsEmpty;
    }

    private void Awake()
    {
        instance = this;
        StartCoroutine(StartFirstWarehouseCall(timerBeforeFirstCall));
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

    private IEnumerator StartFirstWarehouseCall(float timer)
    {
        yield return new WaitForSecondsRealtime(timer);
        DefineNextWarehouse();
    }

    public void AddBox()
    {
        TotalBoxes++;
        BoxCountChanged?.Invoke();
    }
}
