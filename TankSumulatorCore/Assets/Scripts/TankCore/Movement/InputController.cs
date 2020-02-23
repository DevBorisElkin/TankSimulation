using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class InputController : MonoBehaviour
{
    TankController tankController;

    public float scrollWheelSensitivity = 1f;
    private void Start()
    {
        tankController = GetComponent<TankController>();
    }

    void Update()
    {
        ReadInputs();
    }

    void ReadInputs()
    {
        ReadKeyboardInputs();
        ReadMouseInputs();
    }

    void ReadKeyboardInputs()
    {
        tankController.turnLeft      = Input.GetKey(KeyCode.A) ? true : false;
        tankController.turnRight     = Input.GetKey(KeyCode.D) ? true : false;
        tankController.moveForward   = Input.GetKey(KeyCode.W) ? true : false;
        tankController.moveBackwards = Input.GetKey(KeyCode.S) ? true : false;
    }

    void ReadMouseInputs()
    {
        tankController.mouseInputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        tankController.SetAimCrossPosition(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) tankController.ShootAtTarget();
        tankController.ChangeMinHeight(Input.GetAxisRaw("Mouse ScrollWheel") * scrollWheelSensitivity);
    }
}
