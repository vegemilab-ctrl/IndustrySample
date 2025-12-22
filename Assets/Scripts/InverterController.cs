using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class InverterController : MonoBehaviour
{
    public enum RotateAxis
    {
        None,
        XAxis,
        YAxis,
        ZAxis,
        Max
    }

    [Header("인버터 파라미터(Settings)")]
    public RotateAxis axis = RotateAxis.YAxis;
    [Delayed]
    public float maxFrequency = 60f;        //최대 주파수
    [Delayed]
    public float maxRPM = 1800f;             //정격 회전수
    [Delayed]
    public float accelTime = 1.0f;              //가속시간(0 -> MAX 도달하는 걸리는 시간)
    [Delayed]
    public float decelTime = 1.0f;              //감속 시간(MAX -> 0 도달하는데 걸리는 시간)

    [Header("제어 입력(PLC Input)")]
    public bool STF = false;                    //정회전 신호
    public bool STR = false;                    //역회전 신호
    [Delayed]
    public float targetHz = 0.0f;              //지령 주파수(아날로그 입력)

    public UnityEvent<bool> onChangedSTF;
    public UnityEvent<bool> onChangedSTR;

    [Header("모니터링(Read Only)")]
    [SerializeField] float _currentHz = 0.0f;       //현재 주파수
    [SerializeField] float _currentRPM = 0.0f;     //현재 RPM

    public float GetCurrentHZ => _currentHz;
    public float GetCurrentRPM => _currentRPM;


    public Rigidbody shaft = null;

    private void Awake()
    {
        shaft = GetComponentInChildren<Rigidbody>();
        shaft.maxAngularVelocity = 1000f;
        shaft.constraints = RigidbodyConstraints.FreezePosition;

        switch (axis)
        {
            case RotateAxis.XAxis:
                shaft.constraints |= RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                break;
            case RotateAxis.YAxis:
                shaft.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                break;
            case RotateAxis.ZAxis:
                shaft.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                break;
            default:
                break;
        }

        shaft.useGravity = false;
        shaft.automaticCenterOfMass = false;
        shaft.automaticInertiaTensor = false;
        shaft.inertiaTensorRotation = Quaternion.identity;
        shaft.inertiaTensor = Vector3.one;
    }

    private void FixedUpdate()
    {
        //타겟 주파수를 알아내기
        float finalTargetHz = 0.0f;

        if (STF && !STR) finalTargetHz = targetHz;
        else if (!STF && STR) finalTargetHz = -targetHz;
        else finalTargetHz = 0f;

        //가감속 로직을 짜야지.
        float rampRate = maxFrequency / (finalTargetHz != 0 ? accelTime : decelTime);
        _currentHz = Mathf.MoveTowards(_currentHz, finalTargetHz, rampRate * Time.fixedDeltaTime);

        //Hz -> RPM -> Rad/s 변환
        //공식:RPM = (120 * Hz) / 극수
        _currentRPM = (_currentHz / maxFrequency) * maxRPM;

        //RPM을 각속도로 변환 RPM * 0.10472 = rad/s
        float radPerSec = _currentRPM * 0.10472f;

        //현재 회전해야 하는 축의 방향을 알아내기
        Vector3 rotationAxis = axis switch
        {
            RotateAxis.XAxis => transform.right,
            RotateAxis.YAxis => transform.up,
            RotateAxis.ZAxis => transform.forward,
            _ => Vector3.zero
        };

        //초당 회전 각도 적용하기.
        shaft.angularVelocity = rotationAxis * radPerSec;
        Debug.Log(rotationAxis * radPerSec);
    }


    public bool IsOnSTF
    {
        get => STF;
        set
        {
            if (STF == value)
                return;

            if (STF = value)
            {
                STR = false;
                onChangedSTR?.Invoke(STR);
            }

            onChangedSTF?.Invoke(STF);
        }
    }

    public bool IsOnSTR
    {
        get => STR;
        set
        {
            if (STR == value)
                return;

            if (STR = value)
            {
                STF = false;
                onChangedSTF?.Invoke(STF);
            }

            onChangedSTR?.Invoke(STR);
        }
    }

    public void ChangeTargetHz(float value)
    {
        if (value < 0f || value > maxFrequency)
            return;

        targetHz = value;
    }

    public void IncreaseTargetHz(float value)
    {
        targetHz = Mathf.Clamp(targetHz + value, 0, maxFrequency);
    }

    public void DecreaseTargetHz(float value)
    {
        targetHz = Mathf.Clamp(targetHz - value, 0, maxFrequency);
    }

    public void PressSTF()
    {
        IsOnSTF = !IsOnSTF;
    }

    public void PressSTR()
    {
        IsOnSTR = !IsOnSTR;
    }
}