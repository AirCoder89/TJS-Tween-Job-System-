using AirCoder.TJ.Core.Ease;
using AirCoder.TJ.Core.Extensions;
using UnityEngine;

namespace AirCoder.Examples.Scripts
{
    public class TestUi : MonoBehaviour
    {
        [SerializeField] private RectTransform rt;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 targetScale;
        [SerializeField] private Easing.EaseType ease;
        
        [ContextMenu("Test")]
        private void Test()
        {
            rt.TweenScale(targetScale).SetEase(ease).OnComplete(() => { Debug.Log($"Tween Complete !"); }).Play();
        }
        
        [ContextMenu("Test 2")]
        private void Test2()
        {
            var job = rt.TweenScale(targetScale, speed)
                .SetEase(ease)
                .OnComplete(() => { Debug.Log($"Tween Complete !"); });

        }
    }
}
