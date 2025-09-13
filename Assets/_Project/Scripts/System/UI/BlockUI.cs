using System;
using CustomTween;
using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{   
    [Header("Config")]
    [SerializeField] private Color blackColor;
    [SerializeField] private Color transparentColor;
    [Header("Attachment")]
    [SerializeField] private Image blockImg;
    [SerializeField] private Image fetchImg;
    [SerializeField] private Image frameImg;
    [SerializeField] private Image handImg;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Image topLeftCornerImg;
    [SerializeField] private Image topRightCornerImg;
    [SerializeField] private Image botRightCornerImg;
    [SerializeField] private Image botLeftCornerImg;

    private BlockUIType _blockUIType;
    private Material _blockMaterial;
    private float _rectangleOffSet = 15f;
    
    private void Awake()
    {
        _blockMaterial = Instantiate(blockImg.material);
        blockImg.material = _blockMaterial;   
    }

    public void SetTargetUI(bool isActive, TargetUIType targetUIType = TargetUIType.Frame, RectTransform targetTransform = null, float timeDelay = 0)
    {
        frameImg.gameObject.SetActive(false);
        handImg.gameObject.SetActive(false);
        if (!isActive || targetTransform == null)
        {
            SetRectangle(null); 
            return;
        }

        Tween.Delay(timeDelay, () =>
        {
            fetchImg.gameObject.SetActive(false);
            frameImg.gameObject.SetActive((targetUIType == TargetUIType.Frame ||
                                           targetUIType == TargetUIType.FrameAndHand));
            handImg.gameObject.SetActive((targetUIType == TargetUIType.Hand ||
                                          targetUIType == TargetUIType.FrameAndHand));
            switch (targetUIType)
            {
                case TargetUIType.Frame:
                    frameImg.transform.position = targetTransform.position;
                    break;
                case TargetUIType.Hand:
                    handImg.transform.position = targetTransform.position;
                    break;
                case TargetUIType.FrameAndHand:
                    frameImg.transform.position = targetTransform.position;
                    handImg.transform.position = targetTransform.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetUIType), targetUIType, null);
            }

            SetRectangle(targetTransform);
            SetupCorners(targetTransform);
        });
    }
    
    private void SetRectangle(RectTransform targetTransform)
    {
        if (_blockMaterial == null) return;
        if (targetTransform == null)
        {
            _blockMaterial.SetVector("_HoleScreenRect", new Vector4(0, 0, 0, 0));
            return;
        }

        Vector3[] corners = new Vector3[4];
        targetTransform.GetWorldCorners(corners);

        // --- Use uiCamera for correct screen conversion ---
        Vector2 screenCorner0 = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[0]); // bottom-left
        Vector2 screenCorner2 = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[2]); // top-right

        float xMin = screenCorner0.x;
        float yMin = screenCorner0.y;
        float width = screenCorner2.x - screenCorner0.x;
        float height = screenCorner2.y - screenCorner0.y;

        float halfOffset = _rectangleOffSet * 0.5f;

        // 🔥 Expand position outwards
        xMin -= halfOffset;
        yMin -= halfOffset;
        width += _rectangleOffSet;
        height += _rectangleOffSet;

        _blockMaterial.SetVector("_HoleScreenRect", new Vector4(xMin, yMin, width, height));
    }

    private void SetupCorners(RectTransform targetTransform)
    {
        Vector3[] corners = new Vector3[4];
        targetTransform.GetWorldCorners(corners);
        
        float halfOffset = _rectangleOffSet * 0.04f;

        // Move corners outward
        var topLeftOffSet = corners[1] + new Vector3(-halfOffset, halfOffset);
        var topRightOffset = corners[2] + new Vector3(halfOffset, halfOffset);
        var bottomRightOffset = corners[3] + new Vector3(halfOffset, -halfOffset);
        var bottomLeftOffset = corners[0] + new Vector3(-halfOffset, -halfOffset);
        
        topLeftCornerImg.transform.position = corners[1];
        topRightCornerImg.transform.position = corners[2];
        botRightCornerImg.transform.position = corners[3];
        botLeftCornerImg.transform.position = corners[0];
        
        Tween.Position(topLeftCornerImg.rectTransform, topLeftOffSet, corners[1],.4f,  Ease.Linear);
        Tween.Position(topRightCornerImg.rectTransform, topRightOffset, corners[2],.4f,  Ease.Linear);
        Tween.Position(botRightCornerImg.rectTransform, bottomRightOffset, corners[3],.4f,  Ease.Linear);
        Tween.Position(botLeftCornerImg.rectTransform, bottomLeftOffset, corners[0],.4f,  Ease.Linear);
    }
    
    public void SetBlockUIState(bool isActive, BlockUIType blockUIType = BlockUIType.Black, bool useFetching = false, float timeDelay = 0,  Action onComplete = null)
    {
        frameImg.gameObject.SetActive(false);
        handImg.gameObject.SetActive(false);
        fetchImg.gameObject.SetActive(useFetching);
        if (isActive)
        {
            gameObject.SetActive(true);
            if (timeDelay == 0)
            {
                switch (blockUIType)
                {
                    case BlockUIType.Transparent:
                        blockImg.color = transparentColor;
                        break;
                    case BlockUIType.Black:
                        blockImg.color = blackColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(blockUIType), blockUIType, null);
                }
                onComplete?.Invoke();
            }
            else
            {
                switch (blockUIType)
                {
                    case BlockUIType.Transparent:
                        Tween.Color(blockImg, transparentColor, timeDelay).OnComplete(()=>
                        {
                            onComplete?.Invoke();
                        });
                        break;
                    case BlockUIType.Black:
                        Tween.Color(blockImg, blackColor, timeDelay).OnComplete(()=>
                        {
                            onComplete?.Invoke();
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(blockUIType), blockUIType, null);
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

public enum BlockUIType
{
    Black,
    Transparent,
}

public enum TargetUIType
{
    Frame,
    Hand,
    FrameAndHand,
}