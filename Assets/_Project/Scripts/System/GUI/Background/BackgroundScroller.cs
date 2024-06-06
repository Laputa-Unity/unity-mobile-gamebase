using UnityEngine;
using UnityEngine.UI;
 
public class BackgroundScroller : MonoBehaviour {
    [SerializeField] private RawImage patternImg;
    [SerializeField] private float x = .02f;
    [SerializeField] private float y = .02f;
    void Update()
    {
        patternImg.uvRect = new Rect(patternImg.uvRect.position + new Vector2(x,y) * Time.deltaTime,patternImg.uvRect.size);
    }
}