using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string controlUIName;                         //UI 게임오브젝트 이름
    public string normalLayerName = "Default";
    public string outlineLayerName = "Outline";
    public Camera eventCam;                             //이벤트 발생되는 카메라
    public RectTransform controlUI;                     //사용할 UI
    public Transform uiCenter;                            //UI 표시 위치 정보
    public bool needUpdateUIPosition;                 //UI 위치 업데이트할 필요가 있냐
    public float duration = 0.5f;                          //등장 애니메이션 길이
    public Ease easingType = Ease.OutBounce;    //애니메이션 타입

    private bool _isUIDisplayed = false;              //현재 UI가 표시되고 있는지
    private bool _isEntered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //string.IsNullOrEmpty(문자열 변수)는 그 안에 문자열이 비어있거나
        //아무 내용도 없으면 True, 내용이 하나라도 들어있으면 false 반환
        if (controlUI == null && !string.IsNullOrEmpty(controlUIName))
        {
            GameObject go = GameObject.Find(controlUIName);
            if (go != null)
            {
                controlUI = go.GetComponent<RectTransform>();
            }
        }

        if (controlUI != null)
        {
            //controlUI.localScale = Vector3.zero;
            //controlUI.gameObject.SetActive(false);
        }

        if (eventCam == null)
        {
            eventCam = Camera.main;
        }

        if (uiCenter == null)
        {
            //GetComponentInChildren<구성요소>() 직계 자손 게임오브젝트안에 있는 구성요소를 가져온다.
            //()안에 true를 넣으면 비활성화 된 구성요소도 가져올 수 있다.
            //controlUI = GetComponentInChildren<RectTransform>(true);

            //transform.Find("자식의 이름") 자식 게임오브젝트 중에서 이름이 같은 게임오브젝트 하나를 리턴한다.        
            uiCenter = transform.Find("UICenter");
        }
    }

    void Update()
    {
        if (!_isUIDisplayed || !needUpdateUIPosition || controlUI == null)
            return;

        //현재 게임오브젝트의 위치에 맞게 UI 스크린 위치 조정.
        controlUI.position = eventCam.WorldToScreenPoint(uiCenter.position);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        //이미 UI가 표시되고 있으면 켜지는 애니메이션을 중복 실행되지 않게 하기 위해 중단.
        if (_isUIDisplayed)
            return;

        //UI가 없으면 애니메이션 동작을 실행할 수 있기 때문에 중단.
        if (controlUI == null)
            return;

        //UI가 표시되는 상태로 변경
        _isUIDisplayed = true;

        //게임오브젝트를 아웃라인 레이어로 변경
        SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(outlineLayerName));

        //transform.SetParent(부모가 될 게임오브젝트의 트랜스폼) 해당 트랜스폼의 부모를 결정할 수 있다.
        //null로 넣으면 부모가 없는 상태로 만들수 있다.
        controlUI.SetParent(GameObject.Find("Canvas").transform);

        //Camera.main.WorldToScreenPoint(씬 내 게임오브젝트 좌표)로 UI 스크린에 해당하는 좌표를 얻을 수 있다.
        controlUI.position = Camera.main.WorldToScreenPoint(uiCenter.position);

        //gameObject.SetActive(온오프) 해당 게임오브젝트의 활성/비활성 변경함수
        controlUI.gameObject.SetActive(true);

        //등장 애니메이션
        controlUI.DOScale(Vector3.one, duration)
            .SetEase(easingType);
    }

    public void CloseUI()
    {
        //UI가 사라질 때 게임오브젝트의 아웃라인도 사라지게 한다.
        SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(normalLayerName));

        //퇴장 애니메이션
        controlUI.DOScale(Vector3.zero, duration)
            .SetEase(easingType)
            .OnComplete(CompleteOffUI);
    }

    //닫기 애니메이션이 완료될 때 호출하는 함수
    private void CompleteOffUI()
    {
        controlUI.gameObject.SetActive(false);
        _isUIDisplayed = false;

        //만약 마우스 포인터가 안에 들어있는 상태라면
        //레이어를 아웃라인으로 변경
        if (_isEntered)
            SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(outlineLayerName));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //마우스 포인터가 안으로 들어온 상태로 변경
        _isEntered = true;
        //UI가 이미 표시되고 있다면 중단
        if (_isUIDisplayed)
            return;

        //게임오브젝트의 레이어를 아웃라인으로 변경
        SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(outlineLayerName));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //마우스 포인터가 밖으로 나간 상태로 변경
        _isEntered = false;
        //UI가 이미 표시되고 있다면 중단
        if (_isUIDisplayed)
            return;

        //게임오브젝트의 레이어를 디폴트로 변경
        SetLayerRecursively(this.gameObject, LayerMask.NameToLayer(normalLayerName));
    }


    //게임오브젝트의 모든 하위 자식들을 지정한 레이어로 변경하는 재귀함수
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        for (int i = 0; i < obj.transform.childCount; ++i)
        {
            SetLayerRecursively(obj.transform.GetChild(i).gameObject, newLayer);
        }
    }
}