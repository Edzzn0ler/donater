using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private float rotationX;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f) * 0.15f;

        Vector2 delta = Mouse.current.delta.ReadValue();

        rotationX -= delta.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * delta.x * sensitivity);
    }
}
