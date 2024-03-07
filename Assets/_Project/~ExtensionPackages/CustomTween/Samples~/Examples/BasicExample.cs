using PrimeTween;
using UnityEngine;

public class BasicExample : MonoBehaviour {
    [Tooltip("Tweak the animation that will be played in Awake() right from the Inspector.\n\n" +
             "Click with mouse to play the animation created from code.")]
    [SerializeField] TweenSettings<Vector3> tweenSettings = new TweenSettings<Vector3>(Vector3.zero, new Vector3(0, 5), 1);

    void Awake() {
        Tween.LocalPosition(transform, tweenSettings);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Tween.LocalPositionX(transform, 0, 3f, 1f, Ease.OutBounce, 2, CycleMode.Yoyo);
        }
    }
}