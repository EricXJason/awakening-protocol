using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: TransformAnimator.cs
/// 所屬模組: Infrastructure/Animation
/// 責任描述: 物件上下浮動、自體旋轉或縮放呼吸之輕量動畫驅動器。
/// 架構分層: Infrastructure / Utility
/// ============================================================================
/// </summary>
public class TransformAnimator : MonoBehaviour
{
    [Header("Float Movement")]
    [SerializeField] private bool enableMove = false;
    [SerializeField] private float moveOffset = 1f;
    [SerializeField] private float moveSpeed = 1f;

    [Header("Rotation")]
    [SerializeField] private bool enableRotate = false;
    [SerializeField] private Vector3 rotateSpeed = new Vector3(0f, 0f, 90f);

    [Header("Breathing Scale")]
    [SerializeField] private bool enableScale = false;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float scaleSpeed = 1f;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    private void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (enableMove)
        {
            Vector3 pos = transform.position;
            pos.y = initialPosition.y + (Mathf.Sin(Time.time * moveSpeed) * moveOffset);
            transform.position = pos;
        }

        if (enableRotate)
        {
            transform.Rotate(rotateSpeed * Time.deltaTime);
        }

        if (enableScale)
        {
            float t = (Mathf.Sin(Time.time * scaleSpeed) + 1f) * 0.5f;
            float s = Mathf.Lerp(minScale, maxScale, t);
            transform.localScale = initialScale * s;
        }
    }
}
