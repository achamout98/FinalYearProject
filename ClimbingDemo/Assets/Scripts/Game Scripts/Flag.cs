using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{

    [SerializeField] private Material winColor;
    [SerializeField] private GameObject flag;

    private void OnTriggerEnter ( Collider other ) {
        if (other.gameObject.tag.Equals("Player")) {
            flag.GetComponent<SkinnedMeshRenderer>().material = winColor;
            GameManager.instance.mountainConquered = true;
        }
    }
}
