using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMotionTracker : MonoBehaviour
{
    public static Vector3 LeftHandDirection { get; private set; }
    public static float LeftHandSpeed { get; private set; }
    public static Vector3 RightHandDirection { get; private set; }
    public static float RightHandSpeed { get; private set; }
    public static string SimplifiedLeftHandDirection { get; private set; }
    public static string SimplifiedRightHandDirection { get; private set; }

    private void Update()
    {
        // Get the velocity of the left and right hands from the OVRInput
        Vector3 leftHandVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        Vector3 rightHandVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        // Calculate the direction (normalized velocity) of the left and right hands
        LeftHandDirection = leftHandVelocity.normalized;
        RightHandDirection = rightHandVelocity.normalized;

        // Calculate the speed (magnitude of the velocity) of the left and right hands
        LeftHandSpeed = leftHandVelocity.magnitude;
        RightHandSpeed = rightHandVelocity.magnitude;

        // Simplify the direction to a combined direction string
        SimplifiedLeftHandDirection = SimplifyDirection(LeftHandDirection);
        SimplifiedRightHandDirection = SimplifyDirection(RightHandDirection);
    }

    private string SimplifyDirection(Vector3 normalizedVector)
    {
        // Define threshold for considering a component significant
        float threshold = 0.5f; // Adjust this threshold based on your needs

        string direction = "";

        // Check each component of the vector and concatenate direction strings
        direction += Mathf.Abs(normalizedVector.x) > threshold ? (normalizedVector.x > 0 ? "Right" : "Left") : "";
        direction += Mathf.Abs(normalizedVector.y) > threshold ? (normalizedVector.y > 0 ? "Up" : "Down") : "";
        direction += Mathf.Abs(normalizedVector.z) > threshold ? (normalizedVector.z > 0 ? "Forward" : "Backward") : "";

        // If no significant direction is found, return "Stationary" or an equivalent term
        return string.IsNullOrEmpty(direction) ? "Stationary" : direction;
    }
}
