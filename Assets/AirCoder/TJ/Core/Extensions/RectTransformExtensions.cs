using AirCoder.TJ.Core.Jobs;
using UnityEngine;

namespace AirCoder.TJ.Core.Extensions
{
    public static class RectTransformExtensions
    {
        public static ITweenJob TweenScale(this RectTransform rt, Vector3 targetScale, float duration = 1f)
        {
            return TJSystem.Tween(rt, JObType.Scale, targetScale, duration);
        }
        public static ITweenJob TweenAnchorPosition(this RectTransform rt, Vector3 targetPosition, float duration = 1f)
        {
            return TJSystem.Tween(rt, JObType.Position, targetPosition, duration);
        }
        public static ITweenJob TweenRotation(this RectTransform rt, Vector3 targetRotation, float duration = 1f)
        {
            return TJSystem.Tween(rt, JObType.Rotation, targetRotation, duration);
        }
    }
}
