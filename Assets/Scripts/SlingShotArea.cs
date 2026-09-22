using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trước đây mỗi frame khi kéo dây ná đều tạo mới List&lt;Collider2D&gt; và
/// ContactFilter2D -> gây rác bộ nhớ (GC spike) liên tục, dễ giật lag trên
/// Android. Giờ tái sử dụng buffer + cache Camera.main.
/// </summary>
public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask slingShotAreaMask;

    private Camera mainCamera;
    private readonly List<Collider2D> resultsBuffer = new List<Collider2D>(4);
    private ContactFilter2D filter;

    private void Awake()
    {
        mainCamera = Camera.main;

        filter = new ContactFilter2D();
        filter.SetLayerMask(slingShotAreaMask);
        filter.useTriggers = true;
    }

    public bool isWithinSlingShotArea()
    {
        Vector3 mousePos = InputManager.MousePosition;
        mousePos.z = -mainCamera.transform.position.z;

        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePos);

        resultsBuffer.Clear();
        int count = Physics2D.OverlapPoint(worldPosition, filter, resultsBuffer);
        return count > 0;
    }
}