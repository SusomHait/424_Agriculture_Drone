using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BeginnerDroneController : MonoBehaviour
{
    [Header("Drone Movement")]
    public float max_speed = 20f;
    public float max_vertical_speed = 20f;
    public float max_rotation_speed = 50f;
    public float movement_smoothing_factor = 5f;

    [Header("Tilt Values")]
    public float max_tilt_deg = 25f;
    public float tilt_smoothing_factor = 5f;

    public List<Transform> waypoints = new List<Transform>();
    public float waypoint_tolerance = 1.5f;
    public float xz_tolerance = 0.5f;
    public float y_tolerance = 0.5f;
    public bool loop_waypoints = true;

    private Rigidbody rb;

    private Vector3 target_vel;
    private Quaternion target_rotation;

    private float UD_input;
    private float FB_input;
    private float LR_input;
    private float rotation_input;

    private float rotation_angle;

    private int current_waypoint_index = 0;

    private int flight_mode;
    private int image_capture_mode;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        rotation_angle = transform.eulerAngles.y; // get the current yaw
        target_rotation = rb.rotation; // store the current rotation 

        flight_mode = PlayerPrefs.GetInt("FlightMode", 0);
        image_capture_mode = PlayerPrefs.GetInt("ImageCaptureMode", 0);
    }

    private void HandleInputs()
    {
        // up (space) and down (shift) movement
        if (Keyboard.current.spaceKey.isPressed) 
            UD_input = 1f;
        if (Keyboard.current.leftShiftKey.isPressed) 
            UD_input = -1f;

        // forward (W) and backward (S)
        if (Keyboard.current.wKey.isPressed) 
            FB_input = 1f;
        if (Keyboard.current.sKey.isPressed) 
            FB_input = -1f;

        // left (A) and right (D) movement
        if (Keyboard.current.aKey.isPressed)
        {
            rotation_input = -1f;
            LR_input = -1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            LR_input = 1f;
            rotation_input = 1f;
        }
            
        // allows the drone to turn in mid-air without movement
        // left (Q) and right (E)
        if (Keyboard.current.qKey.isPressed) 
            rotation_input = -1f;
        if (Keyboard.current.eKey.isPressed) 
            rotation_input = 1f;
    }

    private void HandleWaypoint()
    {
        Transform target = waypoints[current_waypoint_index]; 
        Vector3 toTarget = target.position - transform.position; 

        // get the simulated vertical input
        float verticalDistToTarget = toTarget.y;
        if (Mathf.Abs(verticalDistToTarget) > y_tolerance)
        {
            UD_input = Mathf.Clamp(verticalDistToTarget, -1f, 1f);
        } else
        {
            UD_input = 0;
        }

        // now try to get the drone to the right place in the xy field
        Vector3 xzTargetVec = new Vector3(toTarget.x, 0f, toTarget.z);
        float lineDist = xzTargetVec.magnitude;

        // if we aren't close enough do some math to get there
        if (lineDist > xz_tolerance)
        {
            xzTargetVec.Normalize(); 

            float targetRot = Mathf.Atan2(xzTargetVec.x, xzTargetVec.z) * Mathf.Rad2Deg; // get the angle to the target vector
            float angleToTarget = Mathf.DeltaAngle(rotation_angle, targetRot); // get the degrees needed to turn from the current angle to reach the new one
        
            rotation_input = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

            FB_input = Mathf.Clamp01(1f - Mathf.Abs(angleToTarget) / 45f);
            LR_input = 0f;
        } else
        {
            FB_input = 0f; 
            LR_input = 0f; 
            rotation_input = 0f;
        }

        // check if we are close enough to the waypoint to then move to the next one
        if (toTarget.magnitude <= waypoint_tolerance)
        {
            NextWaypoint();
        }

    }

    private void NextWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0) return; 

        if (current_waypoint_index < waypoints.Count - 1)
        {
            current_waypoint_index++;
        } else if (loop_waypoints)
        {
            current_waypoint_index = 0;
        }
    }

    void Update()
    {
        // reset the values when re-entering the update
        UD_input = 0f;
        FB_input = 0f;
        LR_input = 0f;
        rotation_input = 0f;

        if (flight_mode == 0)
        {
            HandleInputs();
        } else if (flight_mode == 1)
        {
            HandleWaypoint();
        }
    }

    void FixedUpdate()
    {
        // get the drone's rotation
        rotation_angle += rotation_input * max_rotation_speed * Time.fixedDeltaTime;
        Quaternion yawRotation = Quaternion.Euler(0f, rotation_angle, 0f); // make a rotation value

        // get a vector pointing in the movment direction
        Vector3 move =
            yawRotation * Vector3.forward * FB_input + // use rotaiton when calculating to avoid weird movement
            yawRotation * Vector3.right * LR_input +
            Vector3.up * UD_input;

        // velocity is speed given a direction
        target_vel = move * max_speed;

        // now we linearly move to the new vel from the current one 
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, target_vel, movement_smoothing_factor * Time.fixedDeltaTime);

        // get the pitch and the roll
        // these are based on the max tilt chosen by the user
        float pitchTilt = FB_input * max_tilt_deg;
        float rollTilt = -LR_input * max_tilt_deg;

        Quaternion tiltRotation = Quaternion.Euler(pitchTilt, 0f, rollTilt);

        // perform Quaternion mutliplication
        // basically create a rotation object that yaws then pitches and rolls
        target_rotation = yawRotation * tiltRotation; 

        // now linearly move between the current rotation to the new rotation
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, target_rotation, tilt_smoothing_factor * Time.fixedDeltaTime));   
    }
}