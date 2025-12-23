using UnityEngine;

public class ProductSpawner : MonoBehaviour
{
    //생산할 제품 오브젝트
    public GameObject product;
    //생산 위치
    public Transform[] spawnPositions;
    //첫 상품 제조 시간
    public float startSpawnTime = 0f;
    //삼품 제조 시간 간격
    public float spawnInterval = 1f;
    //상품 상태들(비워두면 적용안함)
    public string[] tagList;

    //상품 제조 예정 시간
    private float _spawnTime = 0f;




    private void OnEnable()
    {
        //상품 제조기에 전원이 켜질 때마다 첫 상품 제조시간을 현재 시간에 더해
        //제조 예정시간에 기록한다.
        _spawnTime = Time.time + startSpawnTime;
    }

    //인풋에 연결된 Product액션에 호출됨.
    public void OnProduct()
    {
        //생산 위치 배열 수만큼 반복해서 생산한다.
        for (int i = 0; i < spawnPositions.Length; ++i)
        {
            GameObject newProduct = Instantiate(product, spawnPositions[i].position, spawnPositions[i].rotation);
            //태그리스트가 1개 이상이면 
            if (tagList.Length > 0)
            {
                //상품에 태그를 랜덤하게 부여한다. 최소값과 최대값(최대값의 -1) 사이의 값이 반환 = Random.Range(최소값, 최대값);
                newProduct.tag = tagList[Random.Range(0, tagList.Length)];
            }
        }
    }

    private void Update()
    {
        //상품 제조 예정 시간이 아직 안되었으면 돌아가라.
        if (_spawnTime > Time.time)
            return;

        //다음 상품 제조 예정 시간을 갱신
        _spawnTime += spawnInterval;
        //상품 제조
        OnProduct();
    }
}