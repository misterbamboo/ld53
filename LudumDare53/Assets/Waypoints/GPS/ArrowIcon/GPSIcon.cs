using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSIcon : MonoBehaviour
{
    // ICON LINK FOR REF : https://www.flaticon.com/free-icon/north_9406934?term=gps&page=2&position=14&origin=tag&related_id=9406934

    [SerializeField]
    float arrowIconHeight = 40.0f;

    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        var playerPos = player.transform.position;
        var newPos = new Vector3(playerPos.x, arrowIconHeight, playerPos.z);
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
