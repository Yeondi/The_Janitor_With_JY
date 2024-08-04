using System.Collections;
using UnityEngine;

public class TwinkleEffect : MonoBehaviour
{
    private Transform tr;
    [SerializeField]
    private Vector2 maxSize;
    [SerializeField]
    private Vector2 minSize;
    [SerializeField]
    private float duration = 1.0f; // 애니메이션의 지속 시간
    [SerializeField]
    private float rotationSpeed = 90.0f; // 회전 속도 (초당 90도)

    public void Start()
    {
        tr = GetComponent<Transform>();
        tr.localScale = minSize;

        StartCoroutine(Twinkle());
    }

    IEnumerator Twinkle()
    {
        while (true)
        {
            // minSize에서 maxSize로
            yield return StartCoroutine(ScaleOverTime(minSize, maxSize, duration));
            // maxSize에서 minSize로
            yield return StartCoroutine(ScaleOverTime(maxSize, minSize, duration));
        }
    }

    IEnumerator ScaleOverTime(Vector2 startSize, Vector2 endSize, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            tr.localScale = Vector2.Lerp(startSize, endSize, elapsed / duration);
            tr.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime); // 시계방향으로 회전
            elapsed += Time.deltaTime;
            yield return null;
        }
        tr.localScale = endSize; // 최종 스케일 설정
    }
}
