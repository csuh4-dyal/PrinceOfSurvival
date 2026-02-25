using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class HingeMotorToggle : MonoBehaviour
{
    public float motorSpeed = 90f;   // degrees per second
    public float motorForce = 100f;

    private HingeJoint hinge;
    private int direction = 1;
    private bool togglePressed = false;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();

        JointMotor motor = hinge.motor;
        motor.force = motorForce;
        motor.targetVelocity = motorSpeed * direction;

        hinge.motor = motor;
        hinge.useMotor = true;
    }

    void Update()
    {
        // detect input
        if (Input.GetKeyDown(KeyCode.I))
        {
            togglePressed = true;   // flag to change motor in FixedUpdate
        }
    }

    void FixedUpdate()
    {
        if (togglePressed)
        {
            direction *= -1;

            JointMotor motor = hinge.motor;
            motor.targetVelocity = motorSpeed * direction;
            motor.force = motorForce;

            hinge.motor = motor;

            togglePressed = false;
        }
    }
}
