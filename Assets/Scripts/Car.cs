using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

struct FrameInputs
{
    public Vector3 direction;
}

[RequireComponent(typeof(CarController))]
public class Car : MonoBehaviour
{

    public Rigidbody rb;
    public Transform cam;
    public Transform tobor;

    public AnimationCurve accelCurve;
    public float maxSpeed = 15, acceleration = 5;
    public float rotSpeed = 5;
    public float bumpForce = 50;
    public float friction = 0.1f;
    public float gravity = -10;

    public Vector3 normal;

    public Vector3 velocity;

    public Vector2 rotation;

    public Text speedText;

    private CarController _controller;

  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
    }

    void FixedUpdate()
    {
        var add = Quaternion.Euler(0, rotation.y, 0) * inputs.direction;
        rb.velocity = Vector3.MoveTowards(rb.velocity, 
            Vector3.ClampMagnitude(
                rb.velocity + add * Time.fixedDeltaTime * acceleration, maxSpeed), 
            acceleration * Time.fixedDeltaTime * accelCurve.Evaluate(rb.velocity.magnitude / maxSpeed));
        rb.AddForce(gravity * Vector3.down * Time.fixedDeltaTime);
        rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero,
            rb.velocity.magnitude * friction * Time.fixedDeltaTime);

        if(speedText)
            speedText.text = $"Speed: {rb.velocity.magnitude:00.0} mph";
    }

    #region Input Handling
    private FrameInputs inputs = new FrameInputs();
    // Update is called once per frame
    void Update()
    {
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
                if (v.normal.y > 0.5f)
                {
                    normal = c.contacts[0].normal;
                    break;
                }
            }
        }
    }

    #region Camera
    void LateUpdate()
    {
        var rot = tobor.rotation;
        var dir = rb.velocity;
        if (dir.sqrMagnitude < 0.4f)
            dir = tobor.rotation * Vector3.forward;
        var tar = Quaternion.LookRotation(dir, normal);
        tobor.rotation = Quaternion.Slerp(rot, tar, Time.deltaTime * rotSpeed);
    } 
    #endregion
}