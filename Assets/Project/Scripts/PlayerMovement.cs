using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;

    private PlayerInputHandler _input;
    private CharacterController _controller;

    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //InputHandlerÇ©ÇÁílÇÇ‡ÇÁÇ§
        Vector2 input = _input.MoveInput;

        float currentSpeed = _input.IsSprinting ? sprintSpeed : moveSpeed;

        Vector3 move = transform.right * input.x + transform.forward * input.y;

        //èdóÕ
        move.y = -2f;

        _controller.Move(move * currentSpeed * Time.deltaTime);
    }
}