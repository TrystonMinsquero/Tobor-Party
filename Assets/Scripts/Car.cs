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
    public Vector2 lookRotation;
    public bool drift;
}

public enum CarState
{
    Small = 1,
    Normal = 2,
    Big = 4,
}

public class Car : PlayerObject
{
    public CarState State
    {
        get
        {
            var s = transform.localScale.x;
            if (s > 1.2f)
                return CarState.Big;
            else if (s < 0.9f)
                return CarState.Small;

            return CarState.Normal;
        }
    }

    [Header("Objects")]
    public Rigidbody rb;
    public CameraObject cam;
    public Transform tobor;
    public CheckpointUser checkpoints;
    public ToborParticles particles;
    public ItemHolder holder;

    [Header("Transform Movement")]
    public float toborRotSpeed = 5;
    public Vector3 groundNormal = Vector3.up;
    public Vector3[] latestNormal = new Vector3[5];

    [Header("FOV")]
    public AnimationCurve fovCurve;
    public float fovSpeed = 5;
    private float currentFOV = 70f;

    [Header("Drifting")] 
    public float minDriftSpeed = 4f;
    public float minDriftStartAngle = 0.1f;

    public AnimationCurve driftTurnSpeedCurve; // -1 = close, 1 = far

    private float driftDirection = 0;

    [Header("Drift Visuals")] 
    public AnimationCurve driftVisualAngleCurve; // -1 = close, 1 = far
    public float driftAngleVisualSpeed = 5f;
    public float driftTiltMultiplier = 0.6f;

    public float driftJumpRecoverTime = 0.14f;
    public float driftJumpSpeed = 3.5f;

    [Header("Camera Movement")] 
    public float lookAroundSpeed = 10f;
    public float camSpeedMoveBack = 0.4f;
    public float camSpeed = 5;
    public float camPosSpeed = 6;
    public Vector2 camOffset;
    public float xRotation = 15;

    private Quaternion camRotation;
    private Vector2 lookRotation;

    [Header("Input Movement")] 
    public float inputMoveSpeed = 4f;
    public float inputRotSpeed = 120f;

    public float currentInputSpeed = 0;
    public Vector3 currentInputDirection = Vector3.forward;

    [Header("Physics Movements")]
    public AnimationCurve accelCurve;
    public AnimationCurve turnSpeedCurve;
    public float maxGasSpeed = 15, acceleration = 5;
    public float defaultFriction= 0.3f;
    public float maxFrictionSpeed = 15;
    public float gravity = 20;
    public float turnVelocityPersistence = 0.95f;
    public float airTurnMultiplier = 0.2f;

    public bool isGrounded = false;
    public bool isDrifting = false;
    public bool isWipeout => wipeoutTime > 0;

    [Header("Boost")] 
    public float boostMaxSpeed = 30;
    public float boostAcceleration = 90;

    [Header("Wipeout")] 
    public float wipeoutRotateSpeed = 720;
    public float wipeoutDampTime = 0.1f;
    public float wipeoutFriction = 3f;
    public float wipeoutRecoverSpeed = 120;

    [Header("Extra Movement")]
    public float bumpForce = 5;
    public float carBumpForce = 1.5f;

    [Header("Other")]
    public Text speedText;
    public CarController _controller;
    public Vector3 startRotation;

    public float wipeoutTime = 0;

    #region Smooth Damp Variables
    private Vector3 dampedToborDirection = Vector3.forward, toborVel = Vector3.zero;
    private Vector3 dampedCameraInputDirection = Vector3.zero, camDirVel = Vector3.zero;
    private Vector3 dampedCarPosition = Vector3.zero, camPosVel = Vector3.zero;
    private Vector3 dampedLookInput = Vector3.zero, lookVel = Vector3.zero;
    private float dampedSpeed = 0, speedVel = 0;
    private float dampedJumpPos = 0, jumpVel = 0;
    private float dampedToborDriftAngle = 0, angleVel = 0;
    private float wipeoutPos = 0, wipeoutVel = 0;
    #endregion

    void Start()
    {
        particles = GetComponentInChildren<ToborParticles>();
        rb = GetComponent<Rigidbody>();
        dampedCarPosition = transform.position;

        groundNormal = Vector3.up;

        camRotation = Quaternion.Euler(startRotation);
        toborRotation = Quaternion.Euler(startRotation);
        currentInputDirection = Quaternion.Euler(startRotation) * Vector3.forward;
        dampedToborDirection = currentInputDirection;

        checkpoints = GetComponent<CheckpointUser>();

        cam.transform.parent = null;
        cam.transform.localScale = Vector3.one;
        cam.car = this;
    }

    void OnEnable()
    {
        cam.gameObject.SetActive(true);
    }

    #region Items: UsingItem, AddItem(), ActivateItem(), DiscardItem()
    public bool UsingItem { get; set; } = false;

    public void AddItem(Item item)
    {
        if (holder.Item == null)
        {
            UsingItem = false;
            holder.Equip(item);
        }
    }

    public void ActivateItem()
    {
        if (!UsingItem)
        {
            holder.Item.Activate(this);
        }
    }

    public void DiscardItem()
    {
        if (holder.Item != null)
        {
            Destroy(holder.Item.gameObject);
            UsingItem = false;
        }
    } 
    #endregion

    private float boostTime = 0;
    public void Boost(float amount)
    {
        if (boostTime < 0)
            boostTime = 0;
        boostTime += amount;
    }

    public void WipeOut(float amount)
    {
        boostTime = 0;

        isDrifting = false;
        driftDirection = 0;
        lastDrift = false;

        // Todo: check if player can be wiped out
        if (wipeoutTime < 0)
            wipeoutTime = 0;
        wipeoutTime += amount;
    }

    private bool lastDrift = false;
    private float turnVelocity = 0;
    void FixedUpdate()
    {
        if (isGrounded)
            groundNormal = latestNormal[4];

        // Inputs
        var accelInput = inputs.direction.z;
        var turnInput = inputs.direction.x;

        if (isWipeout)
        {
            accelInput = 0;
            turnInput = 0;
            wipeoutTime -= Time.fixedDeltaTime;
        }

        // Prevent car from moving before game started
        if (!RaceManager.Started)
        {
            accelInput = 0;
            turnInput = 0;
        }
        
        // XZ velocity
        var velocity = rb.velocity;
        velocity.y = 0;

        // Angle between: input direction and movement
        var signedAngle = Vector3.SignedAngle(velocity, currentInputDirection, Vector3.up);
        var angle = Mathf.Abs(signedAngle);
        
        // Check drifting: grounded, going fast enough, turning in a direction, and not going backwards
        var canDrift = isGrounded && velocity.magnitude > minDriftSpeed && angle > minDriftStartAngle && currentInputSpeed > 0.05f;
        var drifting = inputs.drift && canDrift || driftDirection != 0;

        // Changing move variables
        float turnAmount = 0;
        float velocityTurn = 0;

        // Drifting starting conditions
        if (drifting && !lastDrift && Mathf.Abs(turnInput) > 0.05f)
        {
            isDrifting = true;
            jumpVel = driftJumpSpeed;
            driftDirection = turnInput > 0 ? 1 : -1;
            lastDrift = true;
        }
        else if (!drifting || !inputs.drift)
        {
            lastDrift = false;
            driftDirection = 0;
            isDrifting = false;
        }

        //float driftMult = 1;
        if (isDrifting)
        {
            //turnDir *= driftTurnSpeedMultiplier;
            //Debug.Log($"{(turnDir > 0 && signedAngle > 0 || turnDir < 0 && signedAngle < 0 ? "Same" : "Opposite")}");
            // rotating in same dir
            //if (turnDir > 0 && signedAngle > 0 || turnDir < 0 && signedAngle < 0)
            //    turnDir *= driftTurnSpeedSame.Evaluate(angle);
            // Trying to rotate in opposite direction
            //else
            //    turnDir *= driftTurnSpeedOpposite.Evaluate(angle);
            

            // driftTurnPersistence
            //driftMult = driftTurnVelocityPercentage;
            //driftMult *= driftTurnPersistence.Evaluate(angle);

            // Todo: Smooth turnInput!
            turnAmount = driftTurnSpeedCurve.Evaluate(turnInput * driftDirection) * driftDirection * Time.fixedDeltaTime;
            velocityTurn = turnAmount;
        }

        if (!isGrounded)
            turnInput *= airTurnMultiplier;

        if (!isDrifting)
        {
            // Direction to turn (based on forward/back movement)
            var turnDirection =
                Mathf.Sign(rb.velocity.sqrMagnitude < 0.1 ? 1 : Vector3.Dot(currentInputDirection, rb.velocity));

            turnVelocity = Mathf.Lerp(turnVelocity,
                turnInput * turnDirection, Time.fixedDeltaTime * 6);

            var turn = turnVelocity * turnSpeedCurve.Evaluate(velocity.magnitude / maxGasSpeed);

            turnAmount = turn * inputRotSpeed * Time.fixedDeltaTime;
            velocityTurn = turnAmount * turnVelocityPersistence;
        }

        // Change input speed and direction
        currentInputDirection = Quaternion.Euler(Vector3.up * turnAmount) * currentInputDirection;
        currentInputDirection.Normalize();
        currentInputSpeed = Mathf.MoveTowards(currentInputSpeed, accelInput, inputMoveSpeed * Time.fixedDeltaTime);
        
        var inputVelocity = Vector3.ClampMagnitude(currentInputDirection * currentInputSpeed, 1);
        
        velocity = Quaternion.Euler(Vector3.up * velocityTurn) * velocity;

        var targetAccelerationAdd = inputVelocity * Time.fixedDeltaTime * acceleration;
        var targetAcceleration = acceleration;
        var friction = isWipeout ? wipeoutFriction : defaultFriction;

        var maxSpeed = maxGasSpeed;
        if (boostTime > 0)
        {
            boostTime -= Time.fixedDeltaTime;
            maxSpeed = boostMaxSpeed;

            var boostDir = Vector3.Dot(inputVelocity, currentInputDirection) > 0 ? inputVelocity : -inputVelocity;
            if (boostDir.sqrMagnitude < 0.2f)
                boostDir = currentInputDirection;
            boostDir.Normalize();

            targetAccelerationAdd = boostDir * Time.fixedDeltaTime * boostAcceleration;
            targetAcceleration = boostAcceleration;
        }
        else
        {
            targetAccelerationAdd *= accelCurve.Evaluate(velocity.magnitude / maxSpeed);
        }

        // Clamp max speed
        var finalVelocity = Vector3.ClampMagnitude(velocity + targetAccelerationAdd, maxSpeed);
        velocity = Vector3.MoveTowards(velocity, finalVelocity, targetAcceleration * Time.fixedDeltaTime);

        // Friction
        var mag = Mathf.Clamp(velocity.magnitude, 0, maxFrictionSpeed);
        velocity = Vector3.MoveTowards(velocity, Vector3.zero,mag * friction * Time.fixedDeltaTime);

        var speed = velocity.magnitude;
        if (Vector3.Dot(velocity, currentInputDirection) < 0)
            speed = 0;
        particles.UpdateTrails(speed);

        velocity.y = rb.velocity.y;
        rb.velocity = velocity;

        // Gravity
        rb.AddForce(gravity * Vector3.down * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Drifting?
        // finalVelocity = Vector3.MoveTowards(velocity, currentInputDirection * finalVelocity.magnitude, Time.fixedDeltaTime * 10);

        isGrounded = false;
    }

    #region Input Handling
    private FrameInputs inputs = new FrameInputs();
    private bool lastUseItemInput = false;
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

        inputs.lookRotation = _controller.LookInput;
        inputs.drift = _controller.DriftInput;

        if (holder.Item != null && _controller.UseItemInput && !lastUseItemInput)
        {
            lastUseItemInput = true;
            ActivateItem();
        }

        if (!_controller.UseItemInput)
            lastUseItemInput = false;
    }
    #endregion

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.TryGetComponent<Car>(out var car))
        {
            var thisState = this.State;
            var otherState = car.State;
            float mult = 1;

            if (thisState == otherState)
            {
                if (thisState == CarState.Big)
                    mult = 1.5f;
            }
            else if (thisState > otherState)
            {
                mult = 0.8f;
            }
            else
            {
                mult = 1.5f;
                WipeOut(1f);
            }

            var vel = car.rb.position - rb.position;
            car.rb.AddForce(vel * carBumpForce * mult, ForceMode.VelocityChange);
        }

        if (c.transform.TryGetComponent<Bumpy>(out var bump))
        {
            var p = c.contacts[0].normal;
            rb.AddForce(p * bumpForce, ForceMode.VelocityChange);
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

    private float wipeoutAngle = 0;
    private Quaternion toborRotation;
    void LateUpdate()
    {
        // SmoothDamp for no jaggedness
        dampedToborDirection = Vector3.SmoothDamp(dampedToborDirection, currentInputDirection, ref toborVel, 1 / toborRotSpeed);
        dampedSpeed = Mathf.SmoothDamp(dampedSpeed, rb.velocity.magnitude, ref speedVel, 1 / fovSpeed);
        dampedCameraInputDirection = Vector3.SmoothDamp(dampedCameraInputDirection, currentInputDirection, ref camDirVel, 1 / camSpeed);
        dampedCarPosition = Vector3.SmoothDamp(dampedCarPosition, transform.position, ref camPosVel, 1 / camPosSpeed);
        dampedLookInput = Vector3.SmoothDamp(dampedLookInput, inputs.lookRotation, ref lookVel, 1 / lookAroundSpeed);
        dampedJumpPos = Mathf.SmoothDamp(dampedJumpPos, 0, ref jumpVel, driftJumpRecoverTime);

        // Calculate toborRotation from ground normal and rotation!
        var normal = isGrounded || rb.velocity.sqrMagnitude < 0.4f ? groundNormal : Quaternion.LookRotation(rb.velocity, groundNormal) * Vector3.up;
        var targetRotation = Quaternion.FromToRotation(Vector3.up, normal) * Quaternion.LookRotation(dampedToborDirection, Vector3.up);

        var camVelocityRotation = Quaternion.LookRotation(dampedCameraInputDirection, Vector3.up);
        camRotation = Quaternion.Slerp(camRotation, camVelocityRotation * Quaternion.Euler(xRotation, 0, 0), 1 - Mathf.Exp(Time.deltaTime * -camSpeed));

        var speedPercent = dampedSpeed / maxGasSpeed;
        currentFOV = Mathf.Lerp(currentFOV, fovCurve.Evaluate(speedPercent),
            1 - Mathf.Exp(Time.deltaTime * -fovSpeed));
        cam.fieldOfView = currentFOV;

        var look = Vector3.Scale(new Vector3(-dampedLookInput.y, dampedLookInput.x), new Vector3(20, 90));
        var finalCamRot = Quaternion.Euler(0, look.y, 0) * camRotation * Quaternion.Euler(look.x, 0, 0);
        cam.transform.rotation = finalCamRot;
        var camRotForward = Quaternion.LookRotation(Vector3.Scale(finalCamRot * Vector3.forward, new Vector3(1, 0, 1)).normalized, Vector3.up);
        cam.transform.position = dampedCarPosition + finalCamRot * Vector3.back * (camOffset.x + speedPercent * camSpeedMoveBack) + camRotForward * Quaternion.Euler(look.x, 0, 0) * Vector3.up * camOffset.y;

        // Tobor Transform
        tobor.position = dampedCarPosition + Vector3.up * (dampedJumpPos);
        toborRotation = Quaternion.Slerp(toborRotation, targetRotation, Time.deltaTime * toborRotSpeed);

        // Wipeouts!
        if (isWipeout)
        {
            wipeoutAngle += Time.deltaTime * wipeoutRotateSpeed;
        }
        else
        {
            wipeoutAngle %= 360;
            wipeoutAngle = Mathf.MoveTowardsAngle(wipeoutAngle, 0, wipeoutRecoverSpeed);
        }
        wipeoutPos = Mathf.SmoothDamp(wipeoutPos, wipeoutAngle, ref wipeoutVel, wipeoutDampTime);

        float angleTarget = driftDirection * driftVisualAngleCurve.Evaluate(inputs.direction.x * driftDirection);
        dampedToborDriftAngle = Mathf.SmoothDamp(dampedToborDriftAngle, angleTarget, ref angleVel, 1 / driftAngleVisualSpeed);
        tobor.rotation = toborRotation * Quaternion.Euler(0, dampedToborDriftAngle + wipeoutPos, 0) * Quaternion.Euler(0, 0, dampedToborDriftAngle * driftTiltMultiplier);

        if (isDrifting && isGrounded) particles.StartDrift();
        else particles.StopDrift();
    }
    #endregion

    // returns true if was assigned, false otherwise
    public override bool AssignController(PlayerController playerController)
    {

        if (playerController as CarController == null)
            return false;
        
        _controller = (CarController) playerController;
        PlayerInput playerInput = playerController.gameObject.GetComponent<PlayerInput>();
        playerInput.camera = cam.camera;
        
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