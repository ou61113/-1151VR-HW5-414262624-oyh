using UnityEngine;
using System.Collections;

public class TrainerMotion : MonoBehaviour
{
    [Header("Idle Motion")]
    public float idleHeight = 0.05f;
    public float idleSpeed = 2f;

    [Header("Throw Motion")]
    public float jumpHeight = 0.5f;
    public float jumpDuration = 0.25f;

    private Vector3 startPosition;

    private bool isThrowing = false;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // 待機時微幅上下晃動
        if (!isThrowing)
        {
            float idleOffset = Mathf.Sin(Time.time * idleSpeed)
                              * idleHeight;

            transform.localPosition = startPosition +
                new Vector3(0, idleOffset, 0);
        }
    }

    public void ThrowMotion()
    {
        if (isThrowing)
            return;

        StartCoroutine(JumpMotion());
    }

    IEnumerator JumpMotion()
    {
        isThrowing = true;

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float t = timer / jumpDuration;

            // 跳起後平順回到原位
            float jumpOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.localPosition = startPosition +
                new Vector3(0, jumpOffset, 0);

            yield return null;
        }

        transform.localPosition = startPosition;

        isThrowing = false;
    }
}