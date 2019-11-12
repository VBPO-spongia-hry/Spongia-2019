using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public ShipTemplate ShipTemplate;
    public float MoveSpeed;
    public float RotateSpeed;
    public float height;
    public delegate void Reached();
    public event Reached ReachedEndOfPath;

    [HideInInspector]
    public float distance = 0;

    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public int WaypointIndex = 1;
    private Vector3[] Waypoints;
    private Vector3 CurrentPos;
    private Quaternion CurrentRot;
    float timer;
    float rotTimer;
    bool End = false;

    private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
    [HideInInspector]
    public bool inTrigger;

    public void OnWaypointsAssigned(Vector3[] waypoints)
    {
        Waypoints = waypoints;
        CurrentPos = transform.position;
        CurrentRot = transform.rotation;
        timer = 0;
        rotTimer = 0;
    }

    void Start()
    {
        MoveSpeed = ShipTemplate.CalculateSpeed();
    }

    private void FixedUpdate()
    {
        rb.isKinematic = false;
        timer += Time.fixedDeltaTime * MoveSpeed / Vector3.Distance(CurrentPos, Waypoints[WaypointIndex]);
        rotTimer += Time.fixedDeltaTime * RotateSpeed / Vector3.Distance(CurrentPos, Waypoints[WaypointIndex]);
        if (nextWaypoint())
        {
            if (WaypointIndex < Waypoints.Length - 1)
            {
                CurrentPos = Waypoints[WaypointIndex];
                CurrentRot = transform.rotation;
                WaypointIndex++;
                timer = 0;
                rotTimer = 0;
            }
            else
            {
                ReachedEndOfPath += ended;
                ReachedEndOfPath();
            }
            inTrigger = false;
        }
        else
        {
            Vector3 movement = Waypoints[WaypointIndex] - rb.position;
            rb.velocity = movement.normalized * MoveSpeed;
            Quaternion slerp = Quaternion.Slerp(CurrentRot, Quaternion.LookRotation(rb.velocity), rotTimer);
            transform.rotation = slerp;
            distance += rb.velocity.magnitude;
        }
    }

    void ended()
    {
        End = true;
    }

    private bool nextWaypoint()
    {
        Vector3 vector3 = transform.position;
        Vector3 waypoint = Waypoints[WaypointIndex];

        return CloseEnough(vector3.x, waypoint.x) && CloseEnough(vector3.y, waypoint.y) && CloseEnough(vector3.z, waypoint.z);
    }

    bool CloseEnough(float value1, float value2, float acceptableDifference = 0.3f)
    {
        return Mathf.Abs(value1 - value2) <= acceptableDifference;
    }
}
