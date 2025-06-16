using System.Collections;
using UnityEngine;

public class Jelly : MonoBehaviour
{
    public bool isSmaller;

    private void Start()
    {
        if(isSmaller)
        StartCoroutine(ScaleDown(10f));
    }

  private IEnumerator ScaleDown(float duration = 3f)
{
    Vector3 startScale = transform.localScale;
    Vector3 endScale = Vector3.zero;

    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration); // 0 ~ 1 사이 값
        transform.localScale = Vector3.Lerp(startScale, endScale, t);

        yield return null;
    }

    // 최종적으로 스케일 0으로 보정
    transform.localScale = endScale;

    // 필요 시 오브젝트 비활성화 또는 삭제
    gameObject.SetActive(false); 
    Destroy(gameObject);
}
}
