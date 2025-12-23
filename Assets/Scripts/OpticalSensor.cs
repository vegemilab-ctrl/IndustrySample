using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class OpticalSensor : MonoBehaviour
{
    public enum DetectAxis
    {
        None = 0,
        XAxis,
        XAxisMinus,
        YAxis,
        YAxisMinus,
        ZAxis,
        ZAxisMinus,
        Max
    }

    //검출할 게임오브젝트의 레이어들
    public LayerMask detectLayer;
    //검출 가능 거리
    public float detectableDistance = 0.3f;
    //센서 방향
    public DetectAxis axis = DetectAxis.ZAxis;
    //검출할 수 있는 종류
    public string detectableTag = string.Empty;

    //검출 여부를 콜백받을 함수들을 넣어둘 수 있다.
    public UnityEvent<bool> onChangedDetect;
    //검출된 게임오브젝트에 대한 콜백받을 함수들을 넣을 수 있다.
    public UnityEvent<GameObject> onDetectedObject;

    //검출 여부
    private bool _hasDetected = false;
    //검출 위치
    private Vector3 _detectedPoint;
    //검출된 게임오브젝트
    private GameObject _detectedObject;

    public bool HasDetected
    {
        get => _hasDetected;
        set
        {
            if (_hasDetected == value)
                return;

            _hasDetected = value;
            onChangedDetect?.Invoke(value);
        }
    }


    private void Update()
    {
        //레이저나 적외선을 발사해서 충돌한 애가 누구냐
        Vector3 direction = axis switch
        {
            DetectAxis.XAxis => transform.right,
            DetectAxis.XAxisMinus => -transform.right,
            DetectAxis.YAxis => transform.up,
            DetectAxis.YAxisMinus => -transform.up,
            DetectAxis.ZAxis => transform.forward,
            DetectAxis.ZAxisMinus => -transform.forward,
            _ => Vector3.zero
        };

        if (direction == Vector3.zero)
            return;

        //발사할 레이저를 생성한다. new Ray(레이저 시작위치, 레이저 발사 방향)
        Ray ray = new Ray(transform.position, direction);
        //충돌한게 있으면 true = Physics.Raycast(레이저, out RaycastHit 충돌한 객체 정보, 레이저 유지 거리, 레이저와 충돌할 수 있는 레이어들)
        if (Physics.Raycast(ray, out RaycastHit hit, detectableDistance, detectLayer))
        {
            //검사해야 할 태그가 있지만 검출된 게임오브젝트의 태그가 다를 경우 검출하지 못함.
            if (!string.IsNullOrEmpty(detectableTag) && hit.transform.gameObject.tag != detectableTag)
            {
                HasDetected = false;
                //검출된 오브젝트가 없기 때문에 비워둠.
                _detectedObject = null;
                return;
            }

            //hit.transform.gameObject.SendMessage("OnDetected");

            HasDetected = true;
            _detectedPoint = hit.point;
            //검출되어있는 오브젝트와 현재 검출된 오브젝트가 다르면
            if (_detectedObject != hit.transform.gameObject)
            {
                //새로 검출된 오브젝트로 교체하고 
                _detectedObject = hit.transform.gameObject;
                //등록된 콜백함수들에게 알린다.
                onDetectedObject?.Invoke(_detectedObject);
            }
        }
        else
        {
            HasDetected = false;
            //검출된 오브젝트가 없기 때문에 비워둠.
            _detectedObject = null;
        }
    }

    //씬뷰에서 그리고 싶은 것들이 있을 때 이 함수를 사용
    private void OnDrawGizmos()
    {
        Vector3 direction = axis switch
        {
            DetectAxis.XAxis => transform.right,
            DetectAxis.XAxisMinus => -transform.right,
            DetectAxis.YAxis => transform.up,
            DetectAxis.YAxisMinus => -transform.up,
            DetectAxis.ZAxis => transform.forward,
            DetectAxis.ZAxisMinus => -transform.forward,
            _ => Vector3.zero
        };

        if (direction == Vector3.zero)
            return;


        if (_hasDetected)
        {
            //검출될 경우 붉은 색 라인 그리기
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, _detectedPoint);
        }
        else
        {
            //검출 안될 경우 녹색 라인 그리기
            Handles.color = Color.green;
            Handles.DrawLine(transform.position, transform.position + direction * detectableDistance);
        }

    }
}