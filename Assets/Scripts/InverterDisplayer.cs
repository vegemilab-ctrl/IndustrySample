using System;
using UnityEngine;
using UnityEngine.UI;

public class InverterDisplayer : MonoBehaviour
{
    public InverterController controller;
    public Text currentHz;
    public Text currentDirection;

    //매번 업데이트하는 함수, 최적화하려면 액션이 있을때만 응답하게 끔하면 좋지만 추가되는 문자열이 많다, 의존성이 커진다(?)
    void Update()
    {
        //mathf.Abs(값) 절대값을 구하는 함수, 음수든 양수든 모두 양수로 변환해줌.
        //ToString("표현식") 모든 값을 문자열로 변환시켜줌, F1 : 실수 표현 숫자만큼 소수점 아래 표시.
        //D5 : 5자리수보다 작은 수의 경우 앞에 0을 채워줌.
        //*Hz는 음수가 없으므로 Abs 절대값 함수 사용, F1은 소수점첫째짜리까지 표현한다.
        //currentHz.text = Mathf.Abs(controller.GetCurrentHZ).ToString("F1") + "Hz";
        //\n은 다음줄로 넘기는 기능
        currentDirection.text = $"{(controller.STF ? "STF" : "")}\n{(controller.STR ? "STR" : "")}";
    }
}