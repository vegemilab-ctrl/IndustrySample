using UnityEngine;

public class TowerLampController : MonoBehaviour
{
    //각 램프의 게임오브젝트이름
    [Header("램프 이름")]
    public string redLampName;
    public string yellowLampName;
    public string greenLampName;
    //각 램프의 메쉬렌더러 
    public MeshRenderer redLamp;
    public MeshRenderer yellowLamp;
    public MeshRenderer greenLamp;

    private bool _isOnRed = false;
    private bool _isOnYellow = false;
    private bool _isOnGreen = false;

    //프로퍼티는 함수를 변수처럼 사용하고 정보의 은닉성을 지킬 수 있음.
    public bool IsOnRed
    {
        get => _isOnRed;
        set
        {
            if (_isOnRed == value)
                return;

            _isOnRed = value;
            //메터리얼.EnableKeyward("_EMISSION") 함수로 발광 효과 켜짐.
            //메터리얼.DisableKeyward("_EMISSION") 함수로 발광 효과 제거.
            if (value)
                redLamp.materials[0].EnableKeyword("_EMISSION");
            else
                redLamp.materials[0].DisableKeyword("_EMISSION");
        }
    }

    public bool IsOnYellow
    {
        get => _isOnYellow;
        set
        {
            if (_isOnYellow == value)
                return;

            _isOnYellow = value;
            if (value)
                yellowLamp.materials[0].EnableKeyword("_EMISSION");
            else
                yellowLamp.materials[0].DisableKeyword("_EMISSION");
        }
    }

    public bool IsOnGreen
    {
        get => _isOnGreen;
        set
        {
            if (_isOnGreen == value)
                return;

            _isOnGreen = value;
            if (value)
                greenLamp.materials[0].EnableKeyword("_EMISSION");
            else
                greenLamp.materials[0].DisableKeyword("_EMISSION");
        }
    }

    private void Start()
    {
        GameObject go = null;
        //GameObject.Find("이름") 씬안에 존재하는 활성화 되어 있는 게임오브젝트 중에서 해당 이름을 가진 
        //게임오브젝트를 찾아 반환한다.
        if (redLamp == null)
        {
            go = GameObject.Find(redLampName);
            if (go != null)
            {
                //gameObject.GetComponent<구성요소>() 게임오브젝트에 어태치되어 있는 해당 구성요소를 찾아서 
                //있으면 찾은 구성요소를 반환, 없으면 null 반환
                redLamp = go.GetComponent<MeshRenderer>();

                //material.IsKeywordEnabled("키워드")  해당 키워드가 활성화되어 있는지 확인해 활성화되어 있으면 true, 아니면 false 반환
                _isOnRed = redLamp.materials[0].IsKeywordEnabled("_EMISSION");
            }
        }

        if (yellowLamp == null)
        {
            go = GameObject.Find(yellowLampName);
            if (go != null)
            {
                yellowLamp = go.GetComponent<MeshRenderer>();
                _isOnYellow = yellowLamp.materials[0].IsKeywordEnabled("_EMISSION");
            }
        }

        if (greenLamp == null)
        {
            go = GameObject.Find(greenLampName);
            if (go != null)
            {
                greenLamp = go.GetComponent<MeshRenderer>();
                _isOnGreen = greenLamp.materials[0].IsKeywordEnabled("_EMISSION");
            }
        }

        //프로퍼티는 함수를 변수처럼 사용하고 정보의 은닉성을 지킬 수 있음.
        IsOnRed = false;
        IsOnYellow = false;
        IsOnGreen = false;
    }
}