using AirCoder.TJ.Core.Ease;
using AirCoder.TJ.Core.Extensions;
using UnityEngine;

namespace AirCoder.TJ.Core.Jobs
{
    public class SpriteRendererJob: TweenJobBase
    {
        private SpriteRenderer _spriteRenderer;
        private float _startFloat;
        private float _targetFloat;
        private Color _targetColor;
        private Color _startColor;
        
        public override void ReleaseReferences()
        {
            base.ReleaseReferences();
            _spriteRenderer = null;
            //TODO
        }

        public override ITweenJob TweenTo<T>(T targetInstance, JObType job, params object[] parameters)
        {
            CurrentJobType = job;
            _spriteRenderer = targetInstance as SpriteRenderer;
            if(SelectedEase == null) SelectedEase = Easing.GetEase(Easing.EaseType.Linear);
            
            switch (CurrentJobType)
            {
                case JObType.Color:    JobAction = () => SetupTweenColor((Color) parameters[0]);     break;
                case JObType.Opacity:  JobAction = () => SetupTweenOpacity((float) parameters[0]);   break;
                default: ThrowInvalidJobType();                                                      break;
            }
            Duration = (float) parameters[1];
            return this;
        }

        public override void Tick(float deltaTime)
        {
            if(!IsPlaying) return;
            if(_spriteRenderer == null) ThrowMissingReferenceException(_spriteRenderer);
            CurrentTime += deltaTime;
            this.normalizedTime = CurrentTime / Duration;
            if (CurrentTime >= Duration) CurrentTime = Duration;

            
            switch (CurrentJobType)
            {
                case JObType.Color:   InterpolateColor();    break;
                case JObType.Opacity: InterpolateOpacity();  break;
                default:              ThrowInvalidJobType(); break;
            }
            
            base.RaiseOnTick(); //raise the onUpdate event
            base.CheckJobInterpolationComplete();
        }

        #region Setup
            private void SetupTweenOpacity(float opacityAmount)
            {
                _startFloat = _spriteRenderer.color.a;
                _targetFloat = opacityAmount - _startFloat;
            }
    
            private void SetupTweenColor(Color targetColor)
            {
                _startColor = _spriteRenderer.color;
                _targetColor = targetColor - _startColor;
            }
        #endregion

        #region Interpolation

            private void InterpolateColor()
            {
                var color = TweenColor(_startColor, _targetColor, CurrentTime, Duration);
                _spriteRenderer.color = color;
            }
            private void InterpolateOpacity()
            {
                var alpha = InterpolateFloat(_startFloat, _targetFloat, CurrentTime, Duration);
                _spriteRenderer.SetAlpha(alpha);
            }

        #endregion
    }
}