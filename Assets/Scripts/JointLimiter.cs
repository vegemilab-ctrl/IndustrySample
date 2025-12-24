using UnityEngine;

public class JointLimiter : MonoBehaviour
{
    public enum Axis { X, Y, Z }


    [Range(0.0f, 1.0f)]
    public float weight = 1.0f;
    public Transform jointTarget;

    [Tooltip("이 관절이 회전할 축을 선택하세요")]
    public Axis limitAxis = Axis.Y;

    [Tooltip("최소 회전 각도")]
    public float minAngle = -45f;

    [Tooltip("최대 회전 각도")]
    public float maxAngle = -45f;

    // IK가 계산된 후에 실행되어야 하기 때문에 LateUpdate를 통해 갱신하도록 해야 한다
    private void LateUpdate()
    {
        //1. IK가 계산한 현재의 로컬 회전값을 사원수로 가져온다.
        Quaternion currentLocalRotation = jointTarget.localRotation;

        //2. 사원수를 오일러각으로 변환해서 우리가 제한하고 싶은 축의 각도만 추출한다.
        Vector3 euler = currentLocalRotation.eulerAngles;
        Vector3 origin = transform.localRotation.eulerAngles;

        float angleToLimit = 0f;
        float originAngle = 0f;

        switch (limitAxis)
        {
            case Axis.X:
                angleToLimit = euler.x;
                originAngle = origin.x;
                break;
            case Axis.Y:
                angleToLimit = euler.y;
                originAngle = origin.y;
                break;
            case Axis.Z:
                angleToLimit = euler.z;
                originAngle = origin.z;
                break;
        }

        //3. 추출한 각도를 ClampAngle 함수를 써서 제한한다.
        float clampedAngle = (1 - weight) * originAngle + weight * ClampAngle(angleToLimit);

        //4. 제한된 축과 제한된 각도만을 가진 새로운 사원수를 만들어서 적용한다.
        Quaternion limitedRotation = Quaternion.identity;
        switch (limitAxis)
        {
            case Axis.X:
                limitedRotation = Quaternion.AngleAxis(clampedAngle, Vector3.right);
                break;
            case Axis.Y:
                limitedRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up);
                break;
            case Axis.Z:
                limitedRotation = Quaternion.AngleAxis(clampedAngle, Vector3.forward);
                break;
        }

        transform.localRotation = limitedRotation;
    }

    private float ClampAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, minAngle, maxAngle);
    }
}