using UnityEngine;

public class IKGripper : MonoBehaviour
{
    //잡은 제품의 리지디바디를 변수 놔둠
    public Rigidbody child;
    //현재 잡은 제품이 있는지 여부
    public bool hasItem = false;

    //제품을 잡아라
    public bool Pick()
    {
        if (child == null)
            return false;

        child.isKinematic = true;
        child.transform.SetParent(transform);
        hasItem = true;
        return true;
    }

    //제품을 놓아라
    public bool Drop()
    {
        if (child == null)
            return false;

        child.transform.SetParent(null);
        child.isKinematic = false;
        child = null;
        hasItem = false;
        return true;
    }

    //그리퍼 안에 들어온 제품이 있으면 추가
    private void OnTriggerEnter(Collider other)
    {
        if (child == null)
            child = other.attachedRigidbody;
    }

    //그리퍼 밖으로 나가는 제품이 있으면 제거
    private void OnTriggerExit(Collider other)
    {
        if (child == other.attachedRigidbody)
            child = null;
    }
}