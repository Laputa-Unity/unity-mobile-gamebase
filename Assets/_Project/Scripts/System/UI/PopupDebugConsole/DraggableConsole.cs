using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableConsole : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private RectTransform anchorRectTransform;
    
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;

    private void Start()
    {
        _rectTransform = GetComponentInParent<RectTransform>();
        _canvasGroup = GetComponentInParent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        

        if (Input.touchCount == 1)
        {
            anchorRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        else if (Input.touchCount == 2)
        {
            // Handle pinch zoom
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float newScale = Mathf.Clamp(_rectTransform.localScale.x - deltaMagnitudeDiff * 0.005f, minScale, maxScale);
            _rectTransform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;
    }
}