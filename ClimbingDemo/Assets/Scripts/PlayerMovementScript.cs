using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float grabDistance;
    [SerializeField] private GameObject Child;

    [SerializeField] private StrengthMeter slider;
    [SerializeField] private float MaxTime;
    [SerializeField] private int CoolDownMult;
    private float timeRemaining;

    private Rigidbody rb;
    private Ray ray;
    private RaycastHit hit;

    private bool mouseclicked;

    private float CalculatSliderValue () {
        return (timeRemaining / MaxTime);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mouseclicked = false;
        slider.SetMaxValue(MaxTime);
        timeRemaining = MaxTime;
    }

    void Update()
    {
        //Player rotation (camera and playerbody both rotate)
        if (!GameManager.is_paused) {
            transform.rotation = Quaternion.LookRotation(Child.transform.forward);
        }
        //Grabbing rocks
        Grab();

        
    }

    private void FixedUpdate()
    {
        Move();
    }

    /************************************************************************************************************/

    private void Grab()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            String tag = hit.collider.tag;
            if (tag.Equals("Rock"))
            {
                Transform sphere = hit.collider.gameObject.transform;
                if (Vector3.Distance(transform.position, sphere.position) <= grabDistance)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
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
        Time.timeScale = 1.0f;

        float eta = 0f;

        Vector3 start_pos = transform.position;

        while (mouseclicked) {
            while(eta < time) {
                transform.position = Vector3.Lerp(start_pos, new_pos, (eta / time));
                eta += Time.deltaTime;

                timeRemaining += CoolDownMult * Time.deltaTime;
                if (timeRemaining >= MaxTime) {
                    timeRemaining = MaxTime;
                }
                slider.SetSlider(timeRemaining);

                if (Input.GetMouseButtonUp(0)) {
                    mouseclicked = false;
                    StopCoroutine("Climb");
                    yield break;
                }

                yield return null;
            }
            StartCoroutine(Stay(new_pos));
            yield return null;
        }
        yield return null;
    }

    private IEnumerator Stay (Vector3 new_pos) {
        while (mouseclicked) {
            transform.position = new_pos;
            //**
            timeRemaining -= Time.deltaTime;
            slider.SetSlider(timeRemaining);
            //**
            if (Input.GetMouseButtonUp(0) || timeRemaining <= 0) {
                mouseclicked = false;

                float t = 0f;
                float duration = 0.2f;

                Time.timeScale = 0.2f;
                while(t < duration) {

                    t += Time.deltaTime;

                    yield return null;

                }
                Time.timeScale = 1.0f;

                yield break;
            }
            yield return null;
        }
        
        if (timeRemaining <= 0) {
            while(timeRemaining < MaxTime) {
                timeRemaining += 80 * Time.deltaTime;
                slider.SetSlider(timeRemaining);
                yield return null;
            }
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
