using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public bool detected = false;

    private void OnTriggerEnter ( Collider other ) {
        if (other.tag.Equals("Rock")) {
            if (!detected) {
                detected = true;
            }
        }
    }

    private void OnTriggerExit ( Collider other ) {
        if (other.tag.Equals("Rock")) {
            detected = false;
        }
    }
}
