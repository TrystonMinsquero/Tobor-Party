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

    [Header("Transform Movement")]
    public float rotSpeed = 5;
    public float camSpeed = 5;
    public float fovSpeed = 5;
    public Quaternion camRotation;
    public float currentFOV = 70f;


    [Header("Input Movement")] 
    public float inputMoveSpeed = 4f;
    public float inputRotSpeed = 270f;

    [Header("Physics Movements")]
    public AnimationCurve accelCurve;
    public float maxSpeed = 15, acceleration = 5;
    public float friction = 0.1f;
    public float gravity = 20;
    public float turnVelocityPersistence = 0.95f;
    
    [Header("Extra Movement")]
    public float bumpForce = 50;
    public AnimationCurve fovCurve;

    public Vector3 normal;
    
    public float inputSpeed = 0;
    public Vector3 inputDirection = Vector3.forward;

    public Vector2 rotation;

    public Text speedText;

    public CarController _controller;

  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private float turnVelocity = 0;
    void FixedUpdate()
    {
        //var add = Quaternion.Euler(0, rotation.y, 0) * inputs.direction;

        // Inputs
        var accel = inputs.direction.z;
        var turnDir = inputs.direction.x;

        //var dir = inputVelocity.normalized;
        //var mag = inputVelocity.magnitude;

        // TODO: make tilt, based off angle difference between inputDirection and velocity?

        var velocity = rb.velocity;
        velocity.y = 0;

        turnVelocity = Mathf.Lerp(turnVelocity, turnDir * Mathf.Sign(Vector3.Dot(inputDirection, rb.velocity)), Time.fixedDeltaTime * 6);
        var turn = turnVelocity * (0.5f * rb.velocity.magnitude / maxSpeed + 0.5f);
        var turnAmount = Vector3.up * turn * inputRotSpeed * Time.fixedDeltaTime;
        inputDirection = Quaternion.Euler(turnAmount) * inputDirection;
        inputSpeed = Mathf.MoveTowards(inputSpeed, accel, inputMoveSpeed * Time.fixedDeltaTime);

        inputDirection.Normalize();
        var inputVelocity = Vector3.ClampMagnitude(inputDirection * inputSpeed, 1);

        var add = inputVelocity;

        //var add = Quaternion.Euler(0, rotation.y, 0) * inputs.inputVelocity;

        velocity = Quaternion.Euler(turnAmount * turnVelocityPersistence) * velocity;


        // Clamp max speed
        var finalVelocity = Vector3.ClampMagnitude(velocity + add * Time.fixedDeltaTime * acceleration, maxSpeed);

        // Drifting?
        //finalVelocity = Vector3.MoveTowards(finalVelocity, inputDirection * finalVelocity.magnitude, Time.fixedDeltaTime * 10);
        
        
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
        var d = _controller.MoveInput;
        var dir = new Vector3(d.x, 0, d.y);
        dir.Normalize();
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
        if (c.transform.CompareTag("Floor"))
        {
            foreach (var v in c.contacts)
            {
                if (v.normal.y > 0.4f)
                {
                    normal = v.normal;
                    break;
                }
            }
        }
    }

    #region Camera
    void LateUpdate()
    {
        // Tobor rotation
        //var rot = tobor.rotation;
        //var dir = rb.velocity;
        //if (dir.sqrMagnitude < 0.4f)
        //    dir = tobor.rotation * Vector3.forward;
        var tar = Quaternion.LookRotation(inputDirection, normal);
        tobor.rotation = Quaternion.Slerp(tobor.rotation, tar, Time.deltaTime * rotSpeed);

        var rot = Quaternion.FromToRotation(Vector3.forward, inputDirection);
        camRotation = Quaternion.Slerp(camRotation, rot, 1 - Mathf.Exp(Time.deltaTime * -camSpeed));
        cam.transform.rotation = camRotation * Quaternion.Euler(15, 0, 0);
        cam.transform.position = transform.position + camRotation * Vector3.back * (5) + Vector3.up * 2;

        currentFOV = Mathf.Lerp(currentFOV, fovCurve.Evaluate(rb.velocity.magnitude / maxSpeed),
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
        playerInput.actions.FindActionMap("Gameplay").Enable();
        return true;
    }

    public override bool HasController()
    {
        return _controller != null;
    }
}