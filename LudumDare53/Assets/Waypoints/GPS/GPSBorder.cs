using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSBorder : MonoBehaviour
{
    GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.rotation.eulerAngles.y);
    }
}
