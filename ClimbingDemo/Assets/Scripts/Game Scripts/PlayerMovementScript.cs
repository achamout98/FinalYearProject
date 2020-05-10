using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float grabDistance;
    [SerializeField] private GameObject Cam;

    [SerializeField] private StrengthMeter slider;
    [SerializeField] private float MaxTime;
    [SerializeField]
    [Range(1, 25)]
    private int CoolDownMult = 1;
    private float timeRemaining;

    private Rigidbody rb;

    private GameObject RightHand;
    private GameObject LeftHand;
    private GameObject SelectedHand;

    private Vector3 SelectedHandPos;
    private Vector3 StandbyPos;
    private Vector3 SelectedHandPosR = new Vector3(0.3f, 0.7f, 2);
    private Vector3 SelectedHandPosL = new Vector3(-0.3f, 0.7f, 2);
    private Vector3 HandClimbingPos = Vector3.zero;
    

    private Vector3 OriginalPosR;
    private Vector3 OriginalPosL;

    private Ray ray;
    private RaycastHit hit;
    private bool isGrounded = true;

    private bool mouseclicked = false;

    private bool isClimbing = false;
    private bool switching = false;
    private bool inClimbingPos = false;

    private float CalculatSliderValue () {
        return (timeRemaining / MaxTime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GetHands();

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

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (!switching) {

                StartCoroutine(SwitchHands());
            }
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

    private void OnCollisionEnter ( Collision collision ) {
        if (collision.gameObject.tag.Equals("Ground")) {
            if (inClimbingPos) {
                StartCoroutine(SwitchHands());
            }
            isGrounded = true;
        }
    }

    private void OnCollisionExit ( Collision collision ) {
        if (collision.gameObject.tag.Equals("Ground")) {
            isGrounded = false;
        }
    }
    /************************************************************************************************************/
    private void GetHands () {
        LeftHand = transform.GetChild(1).gameObject;
        OriginalPosL = LeftHand.transform.localPosition;

        RightHand = transform.GetChild(2).gameObject;
        OriginalPosR = RightHand.transform.localPosition;

        SelectedHand = RightHand;
        StartCoroutine(SwitchHands());
    }

    private IEnumerator SwitchHands () {
        switching = true;
        float time = 0.6f;
        float eta = 0f;

        if (SelectedHand.name == "Left_Hand") {
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
        switching = false;
        yield break;
    }

    private void Grab () {
        var origin = SelectedHand.transform.position;
        var direction = SelectedHand.transform.forward;
        var rot = SelectedHand.transform.rotation;

        if (Physics.BoxCast(origin, new Vector3(0.15f, 0.15f, 0.15f), direction, out hit, rot)) {
            String tag = hit.collider.tag;
            if (tag.Equals("Rock")) {
                Transform sphere = hit.collider.gameObject.transform;
                if (Vector3.Distance(transform.position, sphere.position) <= grabDistance) {
                    if (Input.GetMouseButtonDown(0)) {
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
        if (!isClimbing) {
            isClimbing = true;
        }
        StopCoroutine("Stay");
        Time.timeScale = 1.0f;
        float eta = 0f;

        Vector3 start_pos = transform.position;

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
        //SwitchHands();
        while (mouseclicked) {
            transform.position = new_pos;

            timeRemaining -= Time.deltaTime;
            slider.SetSlider(timeRemaining);

            if (Input.GetMouseButtonUp(0) || timeRemaining <= 0) {
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
            yield return null;
        }
 
        yield break;
    }

    private void Move()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(hAxis, 0, vAxis) * speed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + rb.transform.TransformDirection(movement);

        rb.MovePosition(newPosition);
    }
}
