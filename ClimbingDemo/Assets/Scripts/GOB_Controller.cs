using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOB_Controller : MonoBehaviour
{
    private Animator anim;
    private bool is_idle = true;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) {
            is_idle = false;
        } else if (Input.GetKeyUp("space")) {
            is_idle = true;
        }
        anim.SetBool("is_idle", is_idle);
    }
}
