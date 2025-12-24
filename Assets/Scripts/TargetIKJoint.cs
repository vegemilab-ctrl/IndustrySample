using UnityEngine;

public class TargetIKJoint : MonoBehaviour
{
    //타겟IK의 위치
    public Transform root;
    //타겟조인트의 위치
    public Transform joint;
    //타겟 조인트와의 유지 거리
    public float jointDistance = 0.3f;

    //타겟 조인트와의 유지 각도
    [Range(-90, 90)]
    public float angle = 0f;

    //유지 각도를 외부에서 제어하기 위해 만든 프로퍼티
    public float Angle
    {
        get => angle;
        set
        {
            angle = value;
        }
    }

    //현재 제품을 잡은 상태인지에 대한 여부
    public bool picked = false;

    private void Update()
    {
        //잡은 상태라면
        if (picked)
        {
            //조인트가 마스터가 되고 타겟IK가 슬레이브
            Vector3 forward = (transform.position - root.position).normalized;
            forward.y = 0;
            joint.rotation = transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.Rotate(new(angle, 0, 0));
            transform.position = transform.forward * jointDistance + joint.position;
        }
        else
        {
            //잡지 않은 상태이면 타겟IK가 마스터가 되고 조인트가 슬레이브가 되도록 만든다.
            Vector3 forward = (root.position - transform.position).normalized;
            forward.y = 0;
            joint.rotation = transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            joint.Rotate(new(-angle, 0, 0));
            joint.position = joint.forward * jointDistance + transform.position;
        }
    }
}