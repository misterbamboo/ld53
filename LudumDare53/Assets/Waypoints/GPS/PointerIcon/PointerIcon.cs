using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerIcon : MonoBehaviour
{
    GPS GPS;

    [SerializeField]
    float pointerIconHeight = 40.0f;

    GameObject player;

    void Awake()
    {
        GPS = GameObject.FindObjectOfType<GPS>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        var target = GPS.GetDestination();

        if (target == null)
        {
            return;
        }

        var targetPos = target.position;
        var newPos = new Vector3(targetPos.x, pointerIconHeight, targetPos.z);
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
