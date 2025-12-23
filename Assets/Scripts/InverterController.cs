using UnityEngine;
using UnityEngine.Events;

public class InverterController : MonoBehaviour
{
    #region Variables
    [Tooltip("\"모터 회전에 사용할 샤프트 리지드바디를 연결해 주세요. 연결하지 않을 시 \n게임오브젝트 안에 어태치되어 있는 리지드바디를 자동으로 가져옵니다.")]
    public Rigidbody shaft;
    [Tooltip("\"모터 회전에 사용할 샤프트 컨피규러블조인트를 연결해 주세요. 연결하지 않을 시 \n게임오브젝트 안에 어태치되어 있는 컨피규러블조인트를 자동으로 가져옵니다.")]
    public ConfigurableJoint joint;

    [Header("모터 기본 성능")]
    [Delayed]
    public float maxRPM = 1800f;             //정격 회전수
    [Delayed]
    public float maxFrequency = 60f;        //최대 주파수    
    [Delayed]
    public float accelTime = 1.0f;              //가속시간(0 -> Max까지 도달하는데 걸리는 시간
    [Delayed]
    public float decelTime = 1.0f;              //감속시간(Max -> 0까지 도달하는데 걸리는 시간

    [Header("다단 속도 설정")]
    public bool useStep = false;
    public float[] stepFrequencies = new float[8]
    {
        0f, 10f, 30f, 0f, 60f, 0f, 0f, 0f
    };

    [Header("아날로그 입력 설정")]
    public bool useAnalogInput = false;
    public int analogMaxResolution = 4000; //분해능(미쯔비시 : 4000, LS : 16000)

    [Header("상태 모니터링")]
    [Delayed, Range(0f, 240f), SerializeField]
    private float _targetHz = 0.0f;                //지령 주파수
    private int _analogInputValue = 0;          //아날로그 지령
    [SerializeField] float _currentHz = 0.0f;       //현재 주파수
    [SerializeField] float _currentRPM = 0.0f;     //현재 RPM

    private bool _isRun = false;
    private bool _isOnForward = false;      //정회전
    private bool _isOnReverse = false;      //역회전
    private bool _isOnLow = false;          //저속
    private bool _isOnMiddle = false;       //중속
    private bool _isOnHigh = false;         //고속

    public UnityEvent<bool> onChangedRun;
    public UnityEvent<bool> onChangedForward;
    public UnityEvent<bool> onChangedReverse;
    public UnityEvent<bool> onChangedRL;
    public UnityEvent<bool> onChangedRM;
    public UnityEvent<bool> onChangedRH;
    public UnityEvent<int> onChangedAnalog;
    public UnityEvent<float> onChangedTargetHz;
    public UnityEvent<float> onChangedCurrentHz;
    public UnityEvent<float> onChangedCurrentRPM;

    #endregion

    #region Property
    public float GetCurrentHz => _currentHz;
    public float GetCurrentRPM => _currentRPM;

    //모터의 운전 상태 변화(On/Off)에 대한 프로퍼티
    public bool IsRun
    {
        get => _isRun;
        private set
        {
            if (_isRun == value)
                return;

            _isRun = value;
            onChangedRun?.Invoke(value);
        }
    }
    //정방향 회전 상태 변화에 대한 프로퍼티
    public bool STF
    {
        get => _isOnForward;
        set
        {
            if (_isOnForward == value)
                return;

            if (_isOnForward = value)
            {
                onChangedReverse?.Invoke(_isOnReverse = false);
            }

            onChangedForward?.Invoke(value);
        }
    }

    //역방향 회전 상태 변화에 대한 프로퍼티
    public bool STR
    {
        get => _isOnReverse;
        set
        {
            if (_isOnReverse == value)
                return;

            if (_isOnReverse = value)
            {
                onChangedForward?.Invoke(_isOnForward = false);
            }

            onChangedReverse?.Invoke(value);
        }
    }
    //저속 회전 상태 변화에 대한 프로퍼티
    public bool RL
    {
        get => _isOnLow;
        set
        {
            if (!useStep)
                return;

            if (_isOnLow == value)
                return;

            _isOnLow = value;
            onChangedRL?.Invoke(value);
        }
    }

    //중속 회전 상태 변화에 대한 프로퍼티
    public bool RM
    {
        get => _isOnMiddle;
        set
        {
            if (!useStep)
                return;

            if (useStep && _isOnMiddle == value)
                return;

            _isOnMiddle = value;
            onChangedRM?.Invoke(value);
        }
    }

    //고속 회전 상태 변화에 대한 프로퍼티
    public bool RH
    {
        get => _isOnHigh;
        set
        {
            if (!useStep)
                return;

            if (_isOnHigh == value)
                return;

            _isOnHigh = value;
            onChangedRH?.Invoke(value);
        }
    }

    //아날로그 입력값의 변화에 대한 프로퍼티
    public int AnalogInput
    {
        get => _analogInputValue;
        set
        {
            if (_analogInputValue == value)
                return;

            if (!useAnalogInput)
                return;

            _analogInputValue = value;
            onChangedAnalog?.Invoke(value);

            _targetHz = (float)value / analogMaxResolution * maxFrequency;
            onChangedTargetHz?.Invoke(_targetHz);
        }
    }

    public float CurrentHz
    {
        get => Mathf.Abs(_currentHz);
        private set
        {
            _currentHz = value;
            onChangedCurrentHz?.Invoke(value);
        }
    }

    public float CurrentRPM
    {
        get => Mathf.Abs(_currentRPM);
        private set
        {
            _currentRPM = value;
            onChangedCurrentRPM?.Invoke(value);
        }
    }
    #endregion

    #region Private Method
    private void Awake()
    {
        if (shaft == null)
        {
            shaft = GetComponent<Rigidbody>();
        }

        if (shaft != null)
        {
            shaft.automaticCenterOfMass = false;
            shaft.automaticInertiaTensor = false;
            shaft.useGravity = false;
        }

        if (joint == null)
        {
            joint = GetComponent<ConfigurableJoint>();
        }

        if (joint != null)
        {
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Free;
        }
    }

    private void FixedUpdate()
    {
        float finalTargetHz = GetFinalTargetHz();
        shaft.angularVelocity = transform.forward * CalculateCurrentSpeed(finalTargetHz);
    }

    private float GetFinalTargetHz()
    {
        //운전신호가 없으면 정지
        if (!STF && !STR)
        {
            IsRun = false;

            return 0f;
        }

        IsRun = true;
        float direction = STF ? 1f : -1f;

        //다단 속도 확인
        int hzStep = 0;
        if (RL) hzStep += 1;
        if (RM) hzStep += 2;
        if (RH) hzStep += 4;

        if (hzStep > 0)
        {
            Debug.Log(hzStep);
            return direction * stepFrequencies[hzStep];
        }

        return direction * _targetHz;
    }
    private float CalculateCurrentSpeed(float targetHz)
    {
        float rampRate = maxFrequency / (targetHz != 0 ? accelTime : decelTime);
        CurrentHz = Mathf.MoveTowards(_currentHz, targetHz, rampRate * Time.fixedDeltaTime);
        CurrentRPM = (_currentHz / maxFrequency) * maxRPM;

        return _currentRPM * 0.10472f;
    }
    #endregion

    #region Public Method
    public void SetTargetHz(float target)
    {
        if (useAnalogInput)
            return;

        if (_targetHz == target)
            return;

        _targetHz = target;
        onChangedTargetHz?.Invoke(target);
    }
    public void IncreaseTargetHz(float increase)
    {
        if (useAnalogInput)
            return;

        _targetHz = Mathf.Clamp(_targetHz + increase, 0f, maxFrequency);
        onChangedTargetHz?.Invoke(_targetHz);
    }
    public void DecreaseTargetHz(float decrease)
    {
        if (useAnalogInput)
            return;

        _targetHz = Mathf.Clamp(_targetHz - decrease, 0f, maxFrequency);
        onChangedTargetHz?.Invoke(_targetHz);
    }
    public void SetAnalogInputValue(float value)
    {
        AnalogInput = (int)value;
    }
    #endregion
}