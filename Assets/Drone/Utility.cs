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

    private Rigidbody rb;

    private Vector3 target_vel;
    private Quaternion target_rotation;

    private float UD_input;
    private float FB_input;
    private float LR_input;
    private float rotation_input;

    private float rotation_angle;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        rotation_angle = transform.eulerAngles.y; // get the current yaw
        target_rotation = rb.rotation; // store the current rotation 
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

    void Update()
    {
        // reset the values when re-entering the update
        UD_input = 0f;
        FB_input = 0f;
        LR_input = 0f;
        rotation_input = 0f;

        HandleInputs();
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