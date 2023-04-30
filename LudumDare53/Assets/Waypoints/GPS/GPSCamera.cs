using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSCamera : MonoBehaviour
{
    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        var playerPos = player.transform.position;
        var newPos = new Vector3(playerPos.x, transform.position.y, playerPos.z);
        transform.position = newPos;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(90f, player.transform.rotation.eulerAngles.y - 180, 0f);
    }
}
