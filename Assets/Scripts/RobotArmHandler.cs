using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

public class RobotArmHandler : MonoBehaviour
{
    //따라다닐 목표물(자동 추적을 해야 되는 경우)
    public Transform target = null;
    //로봇암의 그리퍼의 위치
    public Transform targetIK;
    //로봇암의 그리퍼 장착 위치
    public Transform jointIK;
    //자유 이동시 사용할 임시타겟
    public Transform freeTarget;

    //IK 적용 속도 스케일값(값이 빠를 수록 동작이 민첩해짐.  너무 빠르면 어색함)
    public float scale = 1f;
    //잡을 수 있는 최대 거리
    public float pickableDistance = 2f;
    //로봇암의 다른 목적지로 이동시 걸리는 시간
    public float moveDuration = 2f;

    //IK Settings
    public MultiAimConstraint root;
    public TwoBoneIKConstraint arm;
    public MultiAimConstraint tip;

    //로봇 암에 장착된 손
    public IKGripper gripper;
    public TargetIKJoint targetJoint;

    private Vector3 origin;


    //추적할 대상 설정 함수(null이면 원점복귀, freeTarget을 넣으면 자유 이동)
    public void SetTarget(Transform target)
    {
        //만약에 타겟이 자유이동타겟이거나 타겟이 비어있거나 같은 타겟일 경우
        if (target == null || target == freeTarget || this.target == target)
        {
            this.target = target;
            return;
        }

        freeTarget.position = targetIK.position;
        this.target = freeTarget;
        this.target.DOMove(target.position, moveDuration)
            .OnComplete(() =>
            {
                this.target = target;
                freeTarget.position = origin;
            });
    }
    //버튼에 연결시 타겟을 비우려면 에러가 나기 때문에 UI버튼용 함수
    public void ResetTarget()
    {
        SetTarget(null);
    }

    //목적지로 위치 이동하는 함수
    public void MoveTarget(Transform destination)
    {
        //그리퍼가 제품을 들고 있을 경우
        if (gripper.hasItem)
        {
            //현재 진행중인 모든 이동 애니메이션을 중지하고
            target.DOKill();
            jointIK.DOKill();
            //조인트에게 새로운 목적지로 이동하는 애니메이션 시작.
            jointIK.DOMove(destination.position, moveDuration);
        }
        else
        {
            //아니면 현재 진행중인 모든 이동 애니메이션을 중지하고
            freeTarget.position = targetIK.position;
            target = freeTarget;
            jointIK.DOKill();
            target.DOKill();
            //타겟에게 새로운 목적지로 이동하는 애니메이션 시작.
            target.DOMove(destination.position, moveDuration);
        }
    }

    //줏어라
    public void Pick()
    {
        if (gripper.Pick())
        {
            targetJoint.picked = true;
        }
    }

    //놔라
    public void Drop()
    {
        if (gripper.Drop())
        {
            target = null;
            targetJoint.picked = false;
        }
    }

    private void Awake()
    {
        //시작시 원점복귀 지점 기억하기
        origin = freeTarget.position;
    }


    private void Update()
    {
        //추적할 대상이 없으면
        if (target == null)
        {
            //로봇암의 원점으로 복귀
            root.weight = Mathf.Clamp01(root.weight - Time.deltaTime * scale);
            arm.weight = Mathf.Clamp01(arm.weight - Time.deltaTime * scale);
            tip.weight = Mathf.Clamp01(tip.weight - Time.deltaTime * scale);
            return;
        }
        //추적할 대상이 있으면서 거리가 잡을 수 있는 거리 내에 있으면
        if (Vector3.Distance(target.position, transform.position) < pickableDistance)
        {
            //로봇암이 타겟을 향해 움직일 수 있게 애니메이션 Weight를 올린다.
            root.weight = Mathf.Clamp01(root.weight + Time.deltaTime * scale);
            arm.weight = Mathf.Clamp01(arm.weight + Time.deltaTime * scale);
            tip.weight = Mathf.Clamp01(tip.weight + Time.deltaTime * scale);
        }
        else
        {
            //추적할 대상이 멀어서 애니메이션 Weight를 내려서 원점으로 복귀
            root.weight = Mathf.Clamp01(root.weight - Time.deltaTime * scale);
            arm.weight = Mathf.Clamp01(arm.weight - Time.deltaTime * scale);
            tip.weight = Mathf.Clamp01(tip.weight - Time.deltaTime * scale);
        }

        //로봇 암의 끝위치를 추적할 대상의 위치로 변경한다.
        targetIK.position = target.position;
    }
}