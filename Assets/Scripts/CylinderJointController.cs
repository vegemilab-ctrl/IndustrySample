using UnityEngine;

public class CylinderJointController : MonoBehaviour
{
    //제어할 조인트 변수
    public ConfigurableJoint joint;
    //전진 위치값
    public Vector3 forwardPosition;

    void Start()
    {
        //제어할 조인트가 비어있으면 알아서 찾아서 넣어라.
        if (joint == null)
            joint = GetComponent<ConfigurableJoint>();
    }

    //전진시키고 싶을 때 호출하는 함수
    public void OnForward()
    {
        //조인트의 타겟위치값에 전진해야하는 위치값을 넣어준다.
        joint.targetPosition = forwardPosition;
    }

    //후퇴시키고 싶을 때 호출하는 함수
    public void OnBackward()
    {
        //조인트의 타겟위치값에 0,0,0값을 넣어 원점으로 되돌아오게 한다.
        joint.targetPosition = Vector3.zero;
    }
}