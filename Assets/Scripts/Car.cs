using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

struct FrameInputs
{
    public Vector3 direction;
}

public class Car : PlayerObject
{

    [Header("Objects")]
    public Rigidbody rb;
    public Camera cam;
    public Transform tobor;
    public CheckpointUser checkpoints;

    [Header("Transform Movement")]
    public float toborRotSpeed = 5;
    public Vector3 normal = Vector3.up;
    public Vector3[] latestNormal = new Vector3[5];

    [Header("FOV")]
    public AnimationCurve fovCurve;
    public float fovSpeed = 5;
    private float currentFOV = 70f;

    [Header("Camera Movement")] 
    public float camSpeedMoveBack = 0.4f;
    public float camSpeed = 5;
    public float camPosSpeed = 6;
    public Vector2 camOffset;
    public float xRotation = 15;

    private Quaternion camRotation;


    [Header("Input Movement")] 
    public float inputMoveSpeed = 4f;
    public float inputRotSpeed = 270f;

    public float currentInputSpeed = 0;
    public Vector3 currentInputDirection = Vector3.forward;

    [Header("Physics Movements")]
    public AnimationCurve accelCurve;
    public float maxSpeed = 15, acceleration = 5;
    public float friction = 0.1f;
    public float gravity = 20;
    public float turnVelocityPersistence = 0.95f;
    public float airTurnMultiplier = 0.2f;

    public bool isGrounded = false;

    [Header("Extra Movement")]
    public float bumpForce = 50;

    [Header("Other")]
    public Text speedText;
    public CarController _controller;
    public Vector3 startRotation;

    #region Smooth Damp Variables
    private Vector3 toborDir = Vector3.forward, toborVel = Vector3.zero;
    private Vector3 camDir = Vector3.zero, camDirVel = Vector3.zero;
    private Vector3 carPos = Vector3.zero, camPosVel = Vector3.zero;
    private float rbSpeed = 0, speedVel = 0;
    #endregion


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        carPos = transform.position;

        normal = Vector3.up;

        camRotation = Quaternion.Euler(startRotation);
        tobor.rotation = Quaternion.Euler(startRotation);
        currentInputDirection = Quaternion.Euler(startRotation) * Vector3.forward;
        toborDir = currentInputDirection;

        checkpoints = GetComponent<CheckpointUser>();
    }

    private float turnVelocity = 0;
    void FixedUpdate()
    {
        if (isGrounded)
            normal = latestNormal[4];

        //var add = Quaternion.Euler(0, rotation.y, 0) * inputs.direction;

        // Inputs
        var accel = inputs.direction.z;
        var turnDir = inputs.direction.x;

        if (!RaceManager.Started)
        {
            accel = 0;
            turnDir = 0;
        }

        //var dir = inputVelocity.normalized;
        //var mag = inputVelocity.magnitude;

        // TODO: make tilt, based off angle difference between currentInputDirection and velocity?

        var velocity = rb.velocity;
        velocity.y = 0;

        if (!isGrounded)
            turnDir *= airTurnMultiplier;

        turnVelocity = Mathf.Lerp(turnVelocity, turnDir * Mathf.Sign(rb.velocity.sqrMagnitude < 0.1 ? 1 : Vector3.Dot(currentInputDirection, rb.velocity)), Time.fixedDeltaTime * 6);
        var turn = turnVelocity * (0.5f * rb.velocity.magnitude / maxSpeed + 0.5f);
        var turnAmount = Vector3.up * turn * inputRotSpeed * Time.fixedDeltaTime;
        currentInputDirection = Quaternion.Euler(turnAmount) * currentInputDirection;
        currentInputSpeed = Mathf.MoveTowards(currentInputSpeed, accel, inputMoveSpeed * Time.fixedDeltaTime);

        currentInputDirection.Normalize();
        var inputVelocity = Vector3.ClampMagnitude(currentInputDirection * currentInputSpeed, 1);

        var add = inputVelocity;

        //var add = Quaternion.Euler(0, rotation.y, 0) * inputs.inputVelocity;

        velocity = Quaternion.Euler(turnAmount * turnVelocityPersistence) * velocity;


        // Clamp max speed
        var finalVelocity = Vector3.ClampMagnitude(velocity + add * Time.fixedDeltaTime * acceleration, maxSpeed);

        // Drifting?
        //finalVelocity = Vector3.MoveTowards(finalVelocity, currentInputDirection * finalVelocity.magnitude, Time.fixedDeltaTime * 10);
        
        
        // Acceleration
        var velocityChange = acceleration * Time.fixedDeltaTime * accelCurve.Evaluate(rb.velocity.magnitude / maxSpeed);
        // Movement
        velocity = Vector3.MoveTowards(velocity, finalVelocity, velocityChange);
        // Friction
        velocity = Vector3.MoveTowards(velocity, Vector3.zero,velocity.magnitude * friction * Time.fixedDeltaTime);
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        // Gravity
        rb.AddForce(gravity * Vector3.down * Time.fixedDeltaTime, ForceMode.VelocityChange);

        if(speedText)
            speedText.text = $"Speed: {rb.velocity.magnitude:00.0} mph";

        isGrounded = false;
    }

    #region Input Handling
    private FrameInputs inputs = new FrameInputs();
    // Update is called once per frame
    void Update()
    {
        if (!HasController())
        {
            Debug.LogWarning("No Controller Assigned!");
            return;
        }
        
        var d = new Vector2(_controller.SteerInput, _controller.GasBreakInput);
        var dir = new Vector3(d.x, 0, d.y);
        //dir.Normalize();
        inputs.direction = dir;
    }
    #endregion

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.TryGetComponent<Bumpy>(out var bump))
        {
            var p = c.contacts[0].normal;
            rb.AddForce(p * bumpForce);
        }
    }

    void OnCollisionStay(Collision c)
    {
        //if (c.transform.CompareTag("Floor"))
        foreach (var v in c.contacts)
        {
            if (v.normal.y > 0.4f)
            {
                if (isGrounded)
                {
                    for (int i = 1; i < latestNormal.Length; i++)
                        latestNormal[i] = latestNormal[i - 1];
                }
                else
                {
                    for (int i = 1; i < latestNormal.Length; i++)
                        latestNormal[i] = v.normal;
                }
                latestNormal[0] = v.normal;

                isGrounded = true;
                break;
            }
        }
    }

    #region Camera
    void LateUpdate()
    {
        var d = currentInputDirection;
        //d.y = rb.velocity.normalized.y;
        toborDir = Vector3.SmoothDamp(toborDir, d, ref toborVel, 1 / toborRotSpeed);

        rbSpeed = Mathf.SmoothDamp(rbSpeed, rb.velocity.magnitude, ref speedVel, 1 / fovSpeed);

        // Tobor rotation
        //var rot = tobor.rotation;
        var dir = toborDir; //rb.velocity;
        if (dir.sqrMagnitude < 0.4f)
            dir = tobor.rotation * Vector3.forward;

        var n = isGrounded ? normal : Quaternion.LookRotation(rb.velocity, normal) * Vector3.up;
        var tar = Quaternion.FromToRotation(Vector3.up, n) * Quaternion.LookRotation(toborDir, Vector3.up);

        camDir = Vector3.SmoothDamp(camDir, currentInputDirection, ref camDirVel, 1 / camSpeed);
        carPos = Vector3.SmoothDamp(carPos, transform.position, ref camPosVel, 1 / camPosSpeed);
        //camY = Mathf.SmoothDampAngle(camY, Vector3.Angle(Vector3.forward, currentInputDirection), ref yVel, 1 / camSpeed);
        //camRotation = Quaternion.Euler(0, camY, 0) * Quaternion.Euler(xRotation, 0, 0);
        var rot = Quaternion.LookRotation(camDir, Vector3.up);
        camRotation = Quaternion.Slerp(camRotation, rot * Quaternion.Euler(xRotation, 0, 0), 1 - Mathf.Exp(Time.deltaTime * -camSpeed));

        var speedPercent = rbSpeed / maxSpeed;

        tobor.position = carPos;
        tobor.rotation = Quaternion.Slerp(tobor.rotation, tar, Time.deltaTime * toborRotSpeed);

        cam.transform.rotation = camRotation;
        cam.transform.position = carPos + camRotation * Vector3.back * (camOffset.x + speedPercent * camSpeedMoveBack) + Vector3.up * camOffset.y;

        currentFOV = Mathf.Lerp(currentFOV, fovCurve.Evaluate(speedPercent),
            1 - Mathf.Exp(Time.deltaTime * -fovSpeed));
        cam.fieldOfView = currentFOV;
    } 
    #endregion

    // returns true if was assigned, false otherwise
    public override bool AssignController(PlayerController playerController)
    {

        if (playerController as CarController == null)
            return false;
        
        _controller = (CarController) playerController;
        PlayerInput playerInput = playerController.gameObject.GetComponent<PlayerInput>();
        playerInput.camera = cam;
        
        // Only enable correct action Map
        foreach(InputActionMap actionMap in playerInput.actions.actionMaps)
            actionMap.Disable();
        playerInput.actions.FindActionMap("Racing").Enable();
        return true;
    }

    public override bool HasController()
    {
        return _controller != null;
    }
}