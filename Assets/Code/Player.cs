using UnityEngine;
using UnityEngine.InputSystem;

public class Player : LocomotionDriver
{
    private Vector3 _inputDirection;

    [Header("Player Locomotion Parameters")] [SerializeField]
    private float speed = 7.5f;

    [SerializeField] private float turnSpeed = 20f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 5f;

    [Header("Interaction Parameters")]
    [SerializeField] private HotspotDetector detector;

    private float _actualSpeed;

    protected override float Speed
    {
        get => _actualSpeed;
        set => speed = value;
    }

    protected override float TurnSpeed
    {
        get => turnSpeed;
        set => turnSpeed = value;
    }

    private void OnEnable()
    {
        base.Init();
    }

    private new void Update()
    {
        base.Update();

        if (Camera.main is null) return;

        var dir = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * _inputDirection;
        base.InputDirection = new Vector3(dir.x, 0, dir.z);

        if (base.InputDirection.magnitude > 1)
        {
            base.InputDirection.Normalize();
        }

        var input = base.InputDirection.magnitude > 0;
        _actualSpeed = Mathf.Lerp(_actualSpeed, input ? speed : 0,
            (input ? acceleration : deceleration) * Time.deltaTime);
    }

    protected override Vector3 GetGravity()
    {
        return new Vector3(0, -9.81f, 0);
    }

    public void OnMove(InputValue value)
    {
        if (base.MoveDirectionOverridden) return;

        var input = value.Get<Vector2>();

        // only normalise input if magnitude is over 1
        // this allows for more fine-grained analog input
        if (input.magnitude > 1)
        {
            input.Normalize();
        }

        _inputDirection = new Vector3(input.x, 0, input.y);
    }

    public async void OnInteract(InputValue value)
    {
        if (value.isPressed && InteractionEnabled)
        {
            await detector.Interact(this);
        }
    }
    
}