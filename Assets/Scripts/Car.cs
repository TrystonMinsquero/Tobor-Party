using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public struct FrameInputs
{
    public Vector2 lookRotation;

    public float steerInput;
    public float gasInput;
    public bool drift;
    public bool useItem;
}

public enum CarState
{
    Small = 1,
    Normal = 2,
    Big = 4,
    Tobor = 8,
}

public class Car : PlayerObject
{
    public CarState State
    {
        get
        {
            var s = transform.localScale.x;
            if (s > 2.5f)
                return CarState.Tobor;
            if (s > 1.2f)
                return CarState.Big;
            else if (s < 0.9f)
                return CarState.Small;

            return CarState.Normal;
        }
    }

    public float Scale = 1;

    [Header("Bot")]
    public bool isBot = false;
    public bool deleteCamera = false;

    private BotInput botInput;

    private BotInput BotInput
    {
        get
        {
            if (botInput != null)
                return botInput;
            botInput = GetComponent<BotInput>();
            if (botInput != null)
                return botInput;
            botInput = gameObject.AddComponent<BotInput>();
            return botInput;
        }
        set => botInput = value;
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
    public float minScale = 0.5f;

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

    public AnimationCurve scaleDistanceMultiplierCurve;

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
    public bool isDeactivated = false;

    public float maxMiniGasSpeed = 20f;

    public float minYPosition = -15f;

    [Header("Boost")] 
    public float boostMaxSpeed = 30;
    public float boostAcceleration = 90;

    [Header("Swift Boost")] 
    public float swiftBoostPerTier = 0.1f;

    public const int MaxSwiftTiers = 3;
    public float swiftBoostTierTime = 0.8f;
    public AnimationCurve driftBoostCurve;

    private float swiftBoostPercentage; 

    [Header("Wipeout")] 
    public float wipeoutRotateSpeed = 720;
    public float wipeoutDampTime = 0.1f;
    public float wipeoutFriction = 3f;
    public float wipeoutRecoverSpeed = 120;
    public float defaultWipeoutTime = 1.2f;

    [Header("Comet")] 
    public float cometSpeed = 20f;
    public float cometAccelerationMultiplier = 5f;
    public float cometTurnSpeedMultiplier = 1.2f;
    public bool cometEnabled = false;

    [Header("Extra Movement")]
    public float bumpForce = 5;
    public float carBumpForce = 1.5f;

    public AnimationCurve katsupCurve; // 0 = 1, 1 = 1 lap behind

    [Header("Other")]
    private CarController controller;
    public Vector3 startRotation;
    public AudioSource whirring;

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
        Scale = transform.localScale.x;

        if (isBot)
        {
            BotInput = GetComponent<BotInput>();
            if (deleteCamera)
            {
                Destroy(cam.gameObject);
                cam = null;
            }
        }

        particles = GetComponentInChildren<ToborParticles>();
        rb = GetComponent<Rigidbody>();
        dampedCarPosition = transform.position;

        groundNormal = Vector3.up;

        camRotation = Quaternion.Euler(startRotation);
        toborRotation = Quaternion.Euler(startRotation);
        currentInputDirection = Quaternion.Euler(startRotation) * Vector3.forward;
        dampedToborDirection = currentInputDirection;

        checkpoints = GetComponent<CheckpointUser>();

        if (cam != null)
        {
            cam.transform.parent = null;
            cam.transform.localScale = Vector3.one;
            cam.car = this;
        }
    }

    void OnEnable()
    {
        if (cam != null)
            cam.gameObject.SetActive(true);
    }

    #region Items: UsingItem, AddItem(), ActivateItem(), DiscardItem()
    public bool UsingItem { get; set; } = false;

    public void AddItem(Item item)
    {
        holder.AddItem(item);
    }

    public void ActivateItem()
    {
        if (!UsingItem)
        {
            var item = holder.Items[0];
            item.Activate(this);
        }
    }

    public void DiscardItem()
    {
        if (holder.Items.Count > 0)
        {
            var item = holder.Items[0];
            Destroy(item.gameObject);

            holder.Items.RemoveAt(0);
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
        wipeoutTime = Mathf.Max(amount, wipeoutTime);
        SFXManager.Play("WAH");
    }

    // Deactivates player input, replaces with Bot input
    public void Deactivate()
    {
        isDeactivated = false;

        BotInput = gameObject.AddComponent<BotInput>();
        isBot = true;
    }

    private bool lastDrift = false;
    private float turnVelocity = 0;
    void FixedUpdate()
    {
        FixScalingCollider();

        if (rb.position.y < minYPosition)
        {
            WipeOut(1f);
            var p = (checkpoints.currentCheckpoint.leftGoal.position + checkpoints.currentCheckpoint.rightGoal.position) / 2f;
            rb.position = p + Vector3.up * 4;
            rb.velocity = Vector3.zero;
        }

        if (isGrounded)
            groundNormal = latestNormal[4];

        // Inputs
        var accelInput = Mathf.Clamp(inputs.gasInput, -1, 1);
        var turnInput = Mathf.Clamp(inputs.steerInput, -1, 1);

        if (isDeactivated)
        {
            accelInput = 0;
            turnInput = 0;
        }

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

            // Todo: swift boost drifting
            var tiers = Mathf.FloorToInt(swiftBoostPercentage);
            if (tiers > MaxSwiftTiers)
                tiers = MaxSwiftTiers;

            Boost(tiers * swiftBoostPerTier);
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
            var turn = turnInput * driftDirection;
            turnAmount = driftTurnSpeedCurve.Evaluate(turn) * driftDirection * Time.fixedDeltaTime;
            velocityTurn = turnAmount;

            swiftBoostPercentage += Mathf.Abs(turnAmount) / driftTurnSpeedCurve.Evaluate(1) * driftBoostCurve.Evaluate(turn) / swiftBoostTierTime;
        }

        if (!isGrounded)
            turnInput *= airTurnMultiplier;

        if (!isDrifting)
        {
            swiftBoostPercentage = 0;
            // Direction to turn (based on forward/back movement)
            var turnDirection =
                Mathf.Sign(rb.velocity.sqrMagnitude < 0.1 ? 1 : Vector3.Dot(currentInputDirection, rb.velocity));

            turnVelocity = Mathf.Lerp(turnVelocity,
                turnInput * turnDirection, Time.fixedDeltaTime * 6);

            var turn = turnVelocity * turnSpeedCurve.Evaluate(velocity.magnitude / maxGasSpeed);

            turnAmount = turn * inputRotSpeed * Time.fixedDeltaTime;
            velocityTurn = turnAmount * turnVelocityPersistence;
        }

        if (cometEnabled)
            turnAmount *= cometTurnSpeedMultiplier;

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

        if (cometEnabled)
        {
            maxSpeed = cometSpeed;
            targetAcceleration *= cometAccelerationMultiplier;
        }

        if (transform.localScale.x < 0.9f)
        {
            maxSpeed = maxMiniGasSpeed;
        }

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

        if (RaceManager.FirstPlace != null)
        {
            var progressChange = RaceManager.FirstPlace.Progress - checkpoints.Progress;
            maxSpeed *= katsupCurve.Evaluate(progressChange);
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

        if (!isBot)
        {
            if (boostTime > 0)
            {
                particles.PlaySpeedLines();
            }
            else
            {
                particles.StopSpeedLines();
            }
        }

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
    // UpdateInput is called once per frame
    void Update()
    {
        if (!HasController())
        {
            Debug.LogWarning("No Controller Assigned!");
            return;
        }

        // User inputs
        if (!isBot)
        {
            inputs.lookRotation = controller.LookInput;

            inputs.steerInput = controller.SteerInput;
            inputs.gasInput = controller.GasBreakInput;
            inputs.drift = controller.DriftInput;
            inputs.useItem = controller.UseItemInput;

        }
        else
        {
            BotInput.UpdateInput(ref inputs, checkpoints);
        }

        if (wipeoutTime <= 0 && holder.Items.Count > 0 && inputs.useItem && !lastUseItemInput)
        {
            lastUseItemInput = true;
            ActivateItem();
        }

        if (!inputs.useItem)
            lastUseItemInput = false;
        
        // Audio
        float volume = rb.velocity.magnitude / (maxGasSpeed * 40);
        if (volume > .04f) volume = .04f;
        whirring.volume = volume * Time.timeScale;

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
                if (otherState == CarState.Tobor)
                    mult = 5f;
                WipeOut(defaultWipeoutTime);
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
        FixScalingCollider();
        FixScalingRenderer();

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

        var carPos = dampedCarPosition;

        if (Scale < minScale)
        {
            carPos += Vector3.down * (minScale - Scale) / 2;
        }

        if (cam != null)
        {
            var speedPercent = dampedSpeed / maxGasSpeed;
            currentFOV = Mathf.Lerp(currentFOV, fovCurve.Evaluate(speedPercent),
                1 - Mathf.Exp(Time.deltaTime * -fovSpeed));
            cam.fieldOfView = currentFOV;

            var look = Vector3.Scale(new Vector3(-dampedLookInput.y, dampedLookInput.x), new Vector3(30, 120));
            var finalCamRot = Quaternion.Euler(0, look.y, 0) * camRotation * Quaternion.Euler(look.x, 0, 0);
            cam.transform.rotation = finalCamRot;
            var camRotForward =
                Quaternion.LookRotation(Vector3.Scale(finalCamRot * Vector3.forward, new Vector3(1, 0, 1)).normalized,
                    Vector3.up);
            cam.transform.position = carPos +
                                     (finalCamRot * Vector3.back * (camOffset.x + speedPercent * camSpeedMoveBack) +
                                      camRotForward * Quaternion.Euler(look.x, 0, 0) * Vector3.up * camOffset.y) *
                                     scaleDistanceMultiplierCurve.Evaluate(transform.localScale.x);
        }

        // Tobor Transform
        
        tobor.position = carPos + Vector3.up * (dampedJumpPos);
        toborRotation = Quaternion.Slerp(toborRotation, targetRotation, Time.deltaTime * toborRotSpeed);

        // Wipeouts!
        if (isWipeout)
        {
            wipeoutAngle += Time.deltaTime * wipeoutRotateSpeed;
        }
        else
        {
            if (wipeoutAngle > 0)
            {
                wipeoutAngle %= 360;
                wipeoutAngle -= 360;
            }

            if (wipeoutPos > 360)
            {
                wipeoutPos %= 360;
                wipeoutPos -= 360;
            }

            wipeoutAngle = Mathf.MoveTowards(wipeoutAngle, 0, wipeoutRecoverSpeed);
        }
        wipeoutPos = Mathf.SmoothDamp(wipeoutPos, wipeoutAngle, ref wipeoutVel, wipeoutDampTime);

        // Todo: check inputs.steerInput, change to fixedUpdate version?
        float angleTarget = driftDirection * driftVisualAngleCurve.Evaluate(inputs.steerInput * driftDirection);
        dampedToborDriftAngle = Mathf.SmoothDamp(dampedToborDriftAngle, angleTarget, ref angleVel, 1 / driftAngleVisualSpeed);
        tobor.rotation = toborRotation * Quaternion.Euler(0, dampedToborDriftAngle + wipeoutPos, 0) * Quaternion.Euler(0, 0, dampedToborDriftAngle * driftTiltMultiplier);

        particles.UpdateSwiftBoostColor(swiftBoostPercentage / MaxSwiftTiers);
        if (isDrifting && isGrounded) particles.StartDrift();
        else particles.StopDrift();
    }

    private void FixScalingRenderer()
    {
        if (Scale < minScale)
        {
            tobor.localScale = Vector3.one * (Scale / minScale);
        }
        else
        {
            tobor.localScale = Vector3.one;
        }
    }

    private void FixScalingCollider()
    {
        if (Scale < minScale)
        {
            transform.localScale = Vector3.one * minScale;
        }
        else
        {
            transform.localScale = Vector3.one * Scale;
        }
    }

    #endregion

    // returns true if was assigned, false otherwise
    public override bool AssignController(PlayerController playerController)
    {

        if (playerController as CarController == null)
            return false;
        
        controller = (CarController) playerController;
        PlayerInput playerInput = playerController.gameObject.GetComponent<PlayerInput>();
        playerInput.camera = cam.camera;
        // InitializeAdvertySDK.Initialize(cam.camera);

        // Only enable correct action Map
        foreach(InputActionMap actionMap in playerInput.actions.actionMaps)
            actionMap.Disable();
        playerInput.actions.FindActionMap("Racing").Enable();
        return true;
    }

    public override bool HasController()
    {
        return controller != null || isBot;
    }
}