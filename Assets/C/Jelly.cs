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
        float t = Mathf.Clamp01(elapsed / duration); // 0 ~ 1 ���� ��
        transform.localScale = Vector3.Lerp(startScale, endScale, t);

        yield return null;
    }

    // ���������� ������ 0���� ����
    transform.localScale = endScale;

    // �ʿ� �� ������Ʈ ��Ȱ��ȭ �Ǵ� ����
    gameObject.SetActive(false); 
    Destroy(gameObject);
}
}
