using System.Collections.Generic;  //List, Dictionary같은 배열을 사용할 때 필요한 라이브러리
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class MagneticSensor : MonoBehaviour
{
    //센서 레이어 이름
    public string layerName = "Sensor";
    //센서의 감지 여부가 변경될 때마다 호출할 콜백함수가 들어있는 델리게이트
    public UnityEvent<bool> onChangedDetected;

    //현재 감지 여부
    private bool _hasDetectedPrev = false;
    private bool _hasDetected = false;
    public List<Collider> _detectedList;

    private void Start()
    {
        //활성화시 게임오브젝트의 레이어를 센서로 변경한다.
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    //콜라이더에 다른 콜라이더가 들어왔을 경우 호출됨.(IsTrigger 체크 되어 있어야 만 함.)
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Sensor Entered ->{other.gameObject.name}");
        if (other.attachedRigidbody.isKinematic)
            return;

        _detectedList.Add(other);

        _hasDetected = _detectedList.Count > 0;
        if (_hasDetectedPrev != _hasDetected)
        {
            //Debug.Log("Enter Callback");
            onChangedDetected?.Invoke(_hasDetected);
        }

        _hasDetectedPrev = _hasDetected;
    }

    //콜라이더에 들어와 있던 다른 콜라이더가 나갔을 경우 호출됨.(IsTrigger 체크 되어 있어야 만 함.)
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"Sensor Exit -> {other.gameObject.name}");
        _detectedList.Remove(other);
        _hasDetected = _detectedList.Count > 0;

        if (_hasDetectedPrev != _hasDetected)
        {
            //Debug.Log("Exit Callback");
            onChangedDetected?.Invoke(_hasDetected);
        }

        _hasDetectedPrev = _hasDetected;
    }
}