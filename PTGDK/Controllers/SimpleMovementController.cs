using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMovementController : MonoBehaviour
{
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Transform transform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDetectDistance = 0.4f;
    
    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = true;

    private void OnEnable()
    {
        InputMappingWrapper.Enable();

        moveInputAction = InputMappingWrapper.Get().Player.Move;
        jumpInputAction = InputMappingWrapper.Get().Player.Jump;
    }

    private void OnDisable()
    {
        InputMappingWrapper.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        jumpInputAction.performed += context =>
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDetectDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        Vector2 position = moveInputAction.ReadValue<Vector2>();

        Vector3 move = (transform.right * position.x) + (transform.forward * position.y);
        
        characterController.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
