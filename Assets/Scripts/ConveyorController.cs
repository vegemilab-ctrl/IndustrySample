using System.Collections.Generic; //<-- List, Dictionary 사용하려면 적어줘야 함.
using realvirtual;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    public ConveyorBelt belt;
    public List<Collider> enterList;

    private void Awake()
    {
        //_material = GetComponent<MeshRenderer>().material;

        if (belt == null)
            belt = GetComponent<ConveyorBelt>();
    }

    //트리거 영역에 다른 콜라이더가 들어왔을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        enterList.Add(other);
    }

    //트리거 영역 밖으로 다른 콜라이더가 나갔을 때 호출
    private void OnTriggerExit(Collider other)
    {
        enterList.Remove(other);
    }

    private void FixedUpdate()
    {
        //벨트의 이동속도 * 지나간 시간 * 이동 방향 => 앞으로 이동할 방향과 거리
        Vector3 moveDelta = belt.speed * Time.fixedDeltaTime * transform.forward;
        foreach (Collider col in enterList)
        {
            //현재 위치 + 앞으로 이동할 방향과 거리 => 있어야 할 위치
            col.attachedRigidbody?.MovePosition(col.attachedRigidbody.position + moveDelta);
        }
    }





    //public Vector2 direction = Vector2.up;
    //public float speedPerSec = 1f;

    //private Material _material;
    //private float _uvOffset;


    //private void Update()
    //{
    //    _uvOffset += speedPerSec * Time.deltaTime;
    //    if(_uvOffset > 1f)
    //    {
    //        _uvOffset -= 1f;
    //    }

    //    _material.SetTextureOffset("_BaseMap", direction * _uvOffset);
    //    _material.SetTextureOffset("_BumpMap", direction * _uvOffset);
    //}

    //private void FixedUpdate()
    //{
    //    Vector3 moveDelta = belt.speed * Time.fixedDeltaTime * transform.forward;
    //    foreach(var c in enterList)
    //    {
    //        c.attachedRigidbody?.MovePosition(c.attachedRigidbody.position + moveDelta);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    enterList.Add(other);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    enterList.Remove(other);
    //}
}