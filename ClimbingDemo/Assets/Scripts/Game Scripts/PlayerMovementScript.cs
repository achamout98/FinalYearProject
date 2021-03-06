﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour {
    [Header("Mechanics Parameters")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float grabDistance = 25f;
    [SerializeField] private float MaxTime = 10f;
    [SerializeField]
    [Range(1, 25)]
    private int CoolDownMult = 1;
    [SerializeField] private float penalty = 0.15f;

    [Header("References")]
    [SerializeField] private GameObject Cam;
    [SerializeField] private GameObject RightHand;
    [SerializeField]private GameObject LeftHand;

    [SerializeField] private StrengthMeter slider;
    private float timeRemaining;

    private GameObject SelectedHand;

    private Vector3 SelectedHandPos;
    private Vector3 StandbyPos;
    private Vector3 SelectedHandPosR = new Vector3(0.8f, 0.5f, 1.8f);
    private Vector3 SelectedHandPosL = new Vector3(-0.8f, 0.5f, 1.8f);
    private Vector3 HandClimbingPos = new Vector3(0, -1, 0);

    private Vector3 OriginalPosR;
    private Vector3 OriginalPosL;
    
    private bool isClimbing = false;
    private bool isGrounded = true;
    private bool switching = false;
    private bool mouseclicked = false;
    private bool inClimbingPos = false;


    private Rigidbody rb;
    private Ray ray;
    private RaycastHit hit;
    private Animator anim;


    private float CalculatSliderValue () {
        return (timeRemaining / MaxTime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //anim = RightHand.GetComponent<Animator>();

        OriginalPosL = LeftHand.transform.localPosition;
        OriginalPosR = RightHand.transform.localPosition;

        SelectedHand = RightHand;
        anim = SelectedHand.GetComponent<Animator>();
        StartCoroutine(SwitchHands());
        //GetHands();

        slider.SetMaxValue(MaxTime);
        timeRemaining = MaxTime;
    }

    void Update()
    {
        //Player rotation (camera and playerbody both rotate)
        if (!GameManager.is_paused) {

            float x = Cam.transform.forward.x;
            float y = Cam.transform.forward.y;
            float z = Cam.transform.forward.z;
            
            transform.rotation = Quaternion.LookRotation(new Vector3(x, y, z));
        }

        //switch between hands
        if (Input.GetKeyDown(KeyCode.Tab)) {
            //Check if hands are already switching so player cannot switch hands while hands are already switching
            if (!switching) {

                StartCoroutine(SwitchHands());
            }
        }

        if (Input.GetMouseButtonDown(0) && !switching) {
            //anim = SelectedHand.GetComponent<Animator>();
            anim.SetBool("grabbing", true);
        }
        if (Input.GetMouseButtonUp(0)) {
            anim.SetBool("grabbing", false);
        }

        //Grabbing rocks
        Grab();

        if (isGrounded) {
            timeRemaining = MaxTime;
            slider.SetSlider(timeRemaining);
        }
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    /************************************************************************************************************/
    /***************************************** PHYSICS RELATED FUNCTION *****************************************/
    /************************************************************************************************************/

    private void OnCollisionEnter ( Collision collision ) {
        //Checks if player is on the ground
        if (collision.gameObject.tag.Equals("Ground") || collision.gameObject.tag.Equals("Platform")) {
            //Sets player's hands back to the non-climbing position when the player hits the ground
            if (inClimbingPos) {
                StartCoroutine(SwitchHands());
            }
            isGrounded = true;
        }

        if (collision.gameObject.tag.Equals("Obstacle")) {
            timeRemaining = timeRemaining * (1 - penalty);
            slider.SetSlider(timeRemaining);
       }
    }

    private void OnCollisionExit ( Collision collision ) {
        //Checks if player has left the ground
        if (collision.gameObject.tag.Equals("Ground")) {
            isGrounded = false;
        }
    }
    /************************************************************************************************************/
    /********************************************* PLAYER MECHANICS *********************************************/
    /************************************************************************************************************/
    //private void GetHands () {
    //    LeftHand = transform.GetChild(1).gameObject;
    //    OriginalPosL = LeftHand.transform.localPosition;

    //    RightHand = transform.GetChild(2).gameObject;
    //    OriginalPosR = RightHand.transform.localPosition;

    //    SelectedHand = RightHand;
    //    StartCoroutine(SwitchHands());
    //}

    private IEnumerator SwitchHands () {
        //switched between selected hands
        switching = true;
        float time = 0.6f;
        float eta = 0f;

        if (SelectedHand.tag == "LeftHand") {
            /*
             * Checks if player is climbing to set the hands position accordingly:
             *      If player is climbing then:
             *          Selected hand position should not change
             *          Other hand should be positioned on the rock
             *      
             *      If the player is not climbing:
             *          Slected hand goes to the designated position
             *          Other hand positioned the the side of the player (it's original position)
             */
            if (!isClimbing) {
                inClimbingPos = false;
                StandbyPos = OriginalPosL;

            } else {
                inClimbingPos = true;
                StandbyPos = HandClimbingPos;
            }
            while (eta < time) {
                SelectedHand.transform.localPosition = Vector3.Lerp(SelectedHandPosL, StandbyPos, (eta / time));
                RightHand.transform.localPosition = Vector3.Lerp(OriginalPosR, SelectedHandPosR, (eta / time));
                eta += Time.deltaTime;
                yield return null;
            }
            SelectedHand = RightHand;
            SelectedHandPos = SelectedHandPosR;
        } else {
            if (!isClimbing) {
                inClimbingPos = false;
                StandbyPos = OriginalPosR;
            } else {
                inClimbingPos = true;
                StandbyPos = HandClimbingPos;
            }
            while (eta < time) {
                SelectedHand.transform.localPosition = Vector3.Lerp(SelectedHandPosR, StandbyPos, (eta / time));
                LeftHand.transform.localPosition = Vector3.Lerp(OriginalPosL, SelectedHandPosL, (eta / time));
                eta += Time.deltaTime;
                yield return null;
            }
            SelectedHand = LeftHand;
            SelectedHandPos = SelectedHandPosL;
        }
        anim.SetBool("grabbing", false);
        anim = SelectedHand.GetComponent<Animator>();
        switching = false;
        yield break;
    }

    private void Grab () {
        var child = SelectedHand.transform.GetChild(0);
        var origin = child.transform.position;
        var direction = child.transform.position + child.transform.forward;
        var rot = SelectedHand.transform.rotation;

        var child1 = SelectedHand.transform.GetChild(1);
        var origin1 = child1.transform.position;
        var direction1 = child1.transform.position + child1.transform.forward;

        var child2 = SelectedHand.transform.GetChild(2);
        var origin2 = child2.transform.position;
        var direction2 = child2.transform.position + child2.transform.forward;


        //Debug.Log(rot);
        //if (Physics.BoxCast(origin, new Vector3(1.5f, 2, 1), direction, out hit, rot, 1f)) {
        //    Debug.Log(hit.collider.tag);
        //    String tag = hit.collider.tag;
        //    if (tag.Equals("Rock")) {
        //        Transform sphere = hit.collider.gameObject.transform;
        //        if (Vector3.Distance(transform.position, sphere.position) <= grabDistance) {
        //            if (Input.GetMouseButtonDown(0)) {
        //                StopCoroutine("Stay");
        //                anim.SetBool("grabbing", true);
        //                mouseclicked = true;

        //                float x = sphere.position.x;
        //                float y = sphere.position.y;
        //                float z = sphere.position.z - 2f;

        //                Vector3 new_pos = new Vector3(x, y, z);
        //                StartCoroutine(Climb(new_pos, 0.60f));
        //            }
        //        }
        //    }
        //}

        if (Physics.SphereCast(origin, 0.5f, direction, out hit, 0.5f)
            || Physics.SphereCast(origin1, 0.5f, direction1, out hit, 0.5f)
            || Physics.SphereCast(origin2, 0.5f, direction2, out hit, 0.5f)) {
            String tag = hit.collider.tag;
            if (tag.Equals("Rock")) {
                Transform sphere = hit.collider.gameObject.transform;
                if (Vector3.Distance(transform.position, sphere.position) <= grabDistance) {
                    if (Input.GetMouseButtonDown(0)) {
                        StopCoroutine("Stay");
                        anim.SetBool("grabbing", true);
                        mouseclicked = true;

                        float x = sphere.position.x;
                        float y = sphere.position.y;
                        float z = sphere.position.z - 2f;

                        Vector3 new_pos = new Vector3(x, y, z);
                        StartCoroutine(Climb(new_pos, 0.60f));
                    }
                }
            }
        }
    }

    private IEnumerator Climb(Vector3 new_pos, float time) {
        StopCoroutine("Stay");
        if (!isClimbing) {
            isClimbing = true;
        }
        Time.timeScale = 1.0f;
        float eta = 0f;

        Vector3 start_pos = transform.position;

        //yield return new WaitForSeconds(0.25f);

        StartCoroutine(SwitchHands());
        while (eta < time) {
            if (Input.GetMouseButtonUp(0)) {
                mouseclicked = false;
                StopCoroutine("Climb");
                yield break;
            }
            transform.position = Vector3.Lerp(start_pos, new_pos, (eta / time));
            eta += Time.deltaTime;

            timeRemaining += CoolDownMult * Time.deltaTime;
            if (timeRemaining >= MaxTime) {
                timeRemaining = MaxTime;
            }
            slider.SetSlider(timeRemaining);

            yield return null;
        }
        StartCoroutine(Stay(new_pos));

        yield break;
    }

    private IEnumerator Stay (Vector3 new_pos) {
        /*Suspends player in the desired position after having climbed to it
         * If at any point the player releases the left click or the strength gauge runs out, 
         * the player will fall
         */
        while (mouseclicked) {
            transform.position = new_pos;

            timeRemaining -= Time.deltaTime;
            slider.SetSlider(timeRemaining);

            if (Input.GetMouseButtonUp(0) || timeRemaining <= 0) {
                //if (timeRemaining > 0) {
                //    //yield return new WaitForSeconds(0.25f);
                //    var t = 0.25f;

                //    while (t > 0) {
                //        transform.position = new_pos;
                //        t -= Time.deltaTime;
                //        yield return null;
                //    }
                //}
                break;
            }
            //if (Input.GetMouseButtonUp(0) && timeRemaining > 0) {
            //    //transform.position = new_pos;
            //    //yield return new WaitForSecondsRealtime(0.25f);
            //    StartCoroutine(Fall());
            //    yield break;
            //} else if (timeRemaining <= 0) {
            //    StartCoroutine(Fall());
            //}
            yield return null;
        }
        //transform.position = new_pos;
        StartCoroutine(Fall());
 
        yield break;
    }

    private IEnumerator Fall () {
        mouseclicked = false;
        if (isClimbing) {
            isClimbing = false;
        }
        float t = 0f;
        float duration = 0.2f;

        Time.timeScale = 0.2f;
        while (t < duration) {

            t += Time.deltaTime;

            yield return null;

        }
        Time.timeScale = 1.0f;
        yield break;
    }

    /************************************************************************************************************/
    /********************************************** BASIC MOVEMENT **********************************************/
    /************************************************************************************************************/

    private void Move()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(hAxis, 0, vAxis) * speed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + rb.transform.TransformDirection(movement);

        rb.MovePosition(newPosition);
    }
}
