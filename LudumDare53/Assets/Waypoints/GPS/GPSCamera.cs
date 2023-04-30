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
        // Récupérer la rotation actuelle de l'objet
        Quaternion rotation = transform.rotation;

        // Convertir la rotation en angles d'Euler
        Vector3 euler = rotation.eulerAngles;

        // Modifier l'angle de rotation Y
        euler.y = player.transform.rotation.eulerAngles.y;

        // Convertir les angles d'Euler en rotation quaternion
        rotation = Quaternion.Euler(euler);

        // Appliquer la rotation à l'objet
        transform.rotation = rotation;
    }
}
