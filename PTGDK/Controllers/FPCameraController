using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform transform;
    [SerializeField] private bool invertXAxis = false; 
    [SerializeField] private bool invertYAxis = true;
    [SerializeField] private float sensitivity = 1.0f;
    private InputAction lookInputAction;
    private float xRotation = 0f;
    private float yRotation = 0f;
    

    void OnEnable()
    {
        InputMappingWrapper.Enable();

        lookInputAction = InputMappingWrapper.Get().Player.Look;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        InputMappingWrapper.Disable();
        
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        Vector2 vector2 = lookInputAction.ReadValue<Vector2>();
        xRotation += (vector2.x * sensitivity);
        yRotation += (vector2.y * sensitivity);

        camera.transform.localEulerAngles = new Vector3(yRotation * (invertYAxis ? -1 : 1), 0);
        transform.localEulerAngles = new Vector3(0, xRotation * (invertXAxis ? -1 : 1));

    }
}
