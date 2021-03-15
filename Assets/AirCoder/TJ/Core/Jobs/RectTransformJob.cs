using AirCoder.TJ.Core.Ease;
using UnityEngine;

namespace AirCoder.TJ.Core.Jobs
{
    public class RectTransformJob : TweenJobBase
    {
        private RectTransform _rectTransform;

        private Vector3 _startVector3;
        private Vector3 _targetVector3;

        public override void ReleaseReferences()
        {
            base.ReleaseReferences();
            _rectTransform = null;
            //TODO
        }

        //convention parameters : 0[targetValue] / 1[duration]
        public override ITweenJob TweenTo<T>(T targetInstance, JObType job, params object[] parameters)
        {
            CurrentJobType = job;
            _rectTransform = targetInstance as RectTransform;
            if(SelectedEase == null) SelectedEase = Easing.GetEase(Easing.EaseType.Linear);
            
            switch (CurrentJobType)
            {
                case JObType.Position:  JobAction = () => SetupTweenPosition((Vector3) parameters[0]); break;
                case JObType.Scale:     JobAction = () => SetupTweenScale((Vector3) parameters[0]);    break;
                case JObType.Rotation:  JobAction = () => SetupTweenRotation((Vector3) parameters[0]); break;
                default: ThrowInvalidJobType();                                                        break;
            }
            
            Duration = (float) parameters[1];
            return this;
        }

        public override void Tick(float deltaTime)
        {
            if(!IsPlaying) return;
            if(_rectTransform == null) ThrowMissingReferenceException(_rectTransform);
            CurrentTime += deltaTime;
            this.normalizedTime = CurrentTime / Duration;
            if (CurrentTime >= Duration) CurrentTime = Duration;

            switch (CurrentJobType)
            {
                case JObType.Scale:    InterpolateScale();    break;
                case JObType.Position: InterpolatePosition(); break;
                case JObType.Rotation: InterpolateRotation(); break;
                default:               ThrowInvalidJobType(); break;
            }
            
            base.RaiseOnTick(); //raise the onUpdate event
            base.CheckJobInterpolationComplete();
        }
        
        #region Setup
            private void SetupTweenPosition(Vector3 targetPosition)
            {
                _startVector3 = _rectTransform.anchoredPosition;
                _targetVector3 = targetPosition - _startVector3;
            }
                
            private void SetupTweenScale(Vector3 targetScale)
            {
                _startVector3 = _rectTransform.localScale;
                _targetVector3 = targetScale - _startVector3;
            }
            
            private void SetupTweenRotation(Vector3 targetRotation)
            {
                _startVector3 = _rectTransform.eulerAngles;
                _targetVector3 = targetRotation - _startVector3;
            }
        #endregion
        
        #region Interpolation
            private void InterpolatePosition()
            {
                var pos = TweenVector3(_startVector3, _targetVector3, this.CurrentTime, Duration);
                _rectTransform.anchoredPosition = pos;
            }
                
            private void InterpolateScale()
            {
                var scale = TweenVector3(_startVector3, _targetVector3, this.CurrentTime, Duration);
                _rectTransform.localScale = scale;
            }
                
            private void InterpolateRotation()
            {
                var rotation = TweenVector3(_startVector3, _targetVector3, this.CurrentTime, Duration);
                _rectTransform.eulerAngles = rotation;
            }
        #endregion
    }
}