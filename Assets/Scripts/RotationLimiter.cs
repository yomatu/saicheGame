using UnityEngine;

public class RotationLimiter : MonoBehaviour
{
    public float maxRotationAngle = 45f;

    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;

        // 限制X轴旋转角度
        if (currentRotation.x > maxRotationAngle && currentRotation.x < 180f)
        {
            currentRotation.x = maxRotationAngle;
        }
        else if (currentRotation.x < 360f - maxRotationAngle && currentRotation.x > 180f)
        {
            currentRotation.x = 360f - maxRotationAngle;
        }

        // 限制Y轴旋转角度
        if (currentRotation.y > maxRotationAngle && currentRotation.y < 180f)
        {
            currentRotation.y = maxRotationAngle;
        }
        else if (currentRotation.y < 360f - maxRotationAngle && currentRotation.y > 180f)
        {
            currentRotation.y = 360f - maxRotationAngle;
        }

        // 限制Z轴旋转角度
        if (currentRotation.z > maxRotationAngle && currentRotation.z < 180f)
        {
            currentRotation.z = 0;
        }
        else if (currentRotation.z < 360f - maxRotationAngle && currentRotation.z > 180f)
        {
            currentRotation.z = 0;
        }

        transform.eulerAngles = currentRotation;
    }
}   