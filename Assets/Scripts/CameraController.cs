using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movespeed;
    public float MinZoom;
    public float MaxZoom;
    public float ZoomSensitivity;
    float zoom;
    public BoundsInt bounds;
    public bool EnableMovement = true;
    public bool TechCamera = false;

    Ship following;
    private void Start()
    {
        EnableMovement = true;
    }
    Camera Camera { get { return GetComponent<Camera>(); } }

    Vector3 prevpos;
    private void Update()
    {
        
        if (EnableMovement)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(horizontal, vertical);
            if (Input.GetMouseButton(0))
            {
                movement = Camera.ScreenToWorldPoint(prevpos) - Camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (following == null)
            {
                if (!TechCamera)
                {
                    if (!Input.GetMouseButton(0)) transform.position += movement * movespeed * Time.deltaTime;
                    else transform.position += movement;
                    zoom += ZoomSensitivity * Input.GetAxis("Mouse ScrollWheel");
                    zoom = Mathf.Clamp(zoom, MinZoom, MaxZoom);
                    Camera.orthographicSize = zoom;
                    ClampPostion();
                }
                else
                {
                    Vector3 pos = transform.localPosition;
                    pos.y = 0;
                    pos.z = -10;
                    if(Input.GetMouseButton(0)) pos.x = Mathf.Clamp(pos.x + movement.x, bounds.position.x, bounds.size.x);
                    else pos.x = Mathf.Clamp((pos.x + movement.x) * movespeed, bounds.xMin, bounds.xMax);
                    transform.localPosition = pos;
                }
            }
            else
            {
                transform.position = new Vector3(following.transform.position.x, following.transform.position.y, -10);
                Camera.orthographicSize = 5;
                if (Input.GetKey(KeyCode.Escape))
                {
                    following.Canvas.gameObject.SetActive(false);
                    following.GetComponent<AudioSource>().Stop();
                    following = null;
                }
            }
            prevpos = Input.mousePosition;
        }
    }

    private void ClampPostion()
    {
        Vector3 pos = transform.position;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        pos.z = -10;
        if (pos.x + Camera.main.orthographicSize * Camera.main.aspect > max.x)
        {
            pos.x = max.x - Camera.main.orthographicSize * Camera.main.aspect;
        }
        if (pos.x - Camera.main.orthographicSize * Camera.main.aspect < min.x)
        {
            pos.x = min.x + Camera.main.orthographicSize * Camera.main.aspect;
        }
        if (pos.y + Camera.main.orthographicSize > max.y)
        {
            pos.y = max.y - Camera.main.orthographicSize;
        }
        if (pos.y - Camera.main.orthographicSize < min.y)
        {
            pos.y = min.y + Camera.main.orthographicSize;
        }
        transform.position = pos;
    }

    public void follow(Ship ship)
    {
        following = ship;
        ship.GetComponent<AudioSource>().loop = true;
        ship.GetComponent<AudioSource>().clip = ship.ShipTemplate.Propulsion.MoveClip;
        ship.GetComponent<AudioSource>().Play();
    }
}
