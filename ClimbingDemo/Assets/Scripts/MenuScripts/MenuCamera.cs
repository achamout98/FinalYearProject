using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
