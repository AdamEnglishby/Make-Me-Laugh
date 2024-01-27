using System.Threading.Tasks;
using UnityEngine;

namespace _Morrigan.Scripts.Character.LocomotionSystem
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class LocomotionDriver : MonoBehaviour
    {
        private CharacterController _characterController;

        protected Vector3 InputDirection;
        protected Vector3 MoveDirection, TurnDirection;

        private Vector3 _moveVelocity;
        protected bool TurnDirectionOverridden, MoveDirectionOverridden;

        [Header("Navigation Parameters")] 
        public float navigationTolerance = 0.25f;

        [Header("Animation Parameters")]
        public Animator animator;
        public AnimationCurve moveMagnitudeToAnimationParam = new(
            new Keyframe(0, 0, 8.333f, 8.333f),
            new Keyframe(0.015f, 0.125f, 6, 6),
            new Keyframe(0.08f, 0.35f, 1, 1),
            new Keyframe(1f, 0.95f, 0.25f, 0.25f));

        public string speedParameterName = "Speed";
        private int _speedParam;

        public bool GravityEnabled { get; set; }
        public bool MovementEnabled { get; set; }
        public bool InteractionEnabled { get; set; }

        protected void Init()
        {
            _speedParam = Animator.StringToHash(speedParameterName);
            
            GravityEnabled = true;
            MovementEnabled = true;
            InteractionEnabled = true;
            _characterController = GetComponent<CharacterController>();
        }

        public void Update()
        {
            if (MovementEnabled)
            {
                PollMoveDirection();

                if (!TurnDirectionOverridden)
                {
                    PollTurnDirection();
                }
            }
            
            Move();
            Turn();
        }

        private void Move()
        {
            if (GravityEnabled)
            {
                if (!_characterController.isGrounded)
                {
                    _moveVelocity += GetGravity() * Time.deltaTime;
                }
                else
                {
                    _moveVelocity = Vector3.zero;
                }
            }

            if (!_characterController.enabled) return;

            var motion = (MoveDirection + _moveVelocity) * Speed;
            animator.SetFloat(_speedParam, MovementEnabled ? moveMagnitudeToAnimationParam.Evaluate(MoveDirection.magnitude) : 0);
            
            if (MovementEnabled)
            {
                _characterController.Move(motion * Time.deltaTime);
            }
        }

        private void Turn()
        {
            if (TurnDirection.magnitude == 0f) return;
            
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.LookRotation(new Vector3(TurnDirection.x, 0, TurnDirection.z)), 
                Time.deltaTime * TurnSpeed
            );
        }

        public async Task OverrideTurnDirection(Vector3 direction)
        {
            TurnDirectionOverridden = true;
            TurnDirection = direction;

            var lookRotation = Quaternion.LookRotation(new Vector3(TurnDirection.x, 0, TurnDirection.z));
            while ((Quaternion.Inverse(lookRotation) * transform.rotation).y > 0.01f)
            {
                await Awaitable.NextFrameAsync();
            }
            
            TurnDirectionOverridden = false;
        }

        private void PollMoveDirection()
        {
            this.MoveDirection = this.InputDirection;
        }

        private void PollTurnDirection()
        {
            this.TurnDirection = this.InputDirection;
        }
        protected abstract Vector3 GetGravity();
        
        protected abstract float Speed { get; set; }
        protected abstract float TurnSpeed { get; set; }
    }
}