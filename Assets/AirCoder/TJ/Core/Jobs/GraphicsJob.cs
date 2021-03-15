using AirCoder.TJ.Core.Ease;
using AirCoder.TJ.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace AirCoder.TJ.Core.Jobs
{
    public class GraphicsJob: TweenJobBase
    {
        private Graphic _graphic;
        private float _startFloat;
        private float _targetFloat;
        private Color _targetColor;
        private Color _startColor;
        private Image _image;

        public override void ReleaseReferences()
        {
            base.ReleaseReferences();
            _graphic = null;
            //TODO
        }

        public override ITweenJob TweenTo<T>(T targetInstance, JObType job, params object[] parameters)
        {
            CurrentJobType = job;
            _graphic = targetInstance as Graphic;
            if(SelectedEase == null) SelectedEase = Easing.GetEase(Easing.EaseType.Linear);
            
            switch (CurrentJobType) //setup
            {
                case JObType.Color:       JobAction = () => SetupTweenColor((Color) parameters[0]);       break;
                case JObType.Opacity:     JobAction = () => SetupTweenOpacity((float) parameters[0]);     break;
                case JObType.FillAmount:  JobAction = () => SetupTweenFillAmount((float) parameters[0]);  break;
                default: ThrowInvalidJobType();                                                           break;
            }
            Duration = (float) parameters[1];
            return this;
        }

        public override void Tick(float deltaTime)
        {
            if(!IsPlaying) return;
            if(_graphic == null) ThrowMissingReferenceException(_graphic);
            CurrentTime += deltaTime;
            this.normalizedTime = CurrentTime / Duration;
            if (CurrentTime >= Duration) CurrentTime = Duration;

            switch (CurrentJobType)
            {
                case JObType.Color:      InterpolateColor();      break;
                case JObType.Opacity:    InterpolateOpacity();    break;
                case JObType.FillAmount: InterpolateFillAmount(); break;
                default:                 ThrowInvalidJobType();   break;
            }
            
            base.RaiseOnTick(); //raise the onUpdate event
            base.CheckJobInterpolationComplete();
        }
        

        #region Setup
        private void SetupTweenColor(Color targetColor)
        {
                _startColor = _graphic.color;
                _targetColor = targetColor - _startColor;
            }
    
            private void SetupTweenOpacity(float opacityAmount)
            {
                _startFloat = _graphic.color.a;
                _targetFloat = opacityAmount - _startFloat;
            }
    
            private void SetupTweenFillAmount(float targetAmount)
            {
                _image = (Image)_graphic;
                _startFloat = _image.fillAmount;
                _targetFloat = targetAmount - _startFloat;
            }
        #endregion

        #region Interpolation
            private void InterpolateColor()
            {
                var color = TweenColor(_startColor, _targetColor, CurrentTime, Duration);
                _graphic.color = color;
            }
            
            private void InterpolateFillAmount()
            {
                var fill = InterpolateFloat(_startFloat, _targetFloat, CurrentTime, Duration);
                _image.fillAmount = fill;
            }
                
            private void InterpolateOpacity()
            {
                var alpha = InterpolateFloat(_startFloat, _targetFloat, CurrentTime, Duration);
                _graphic.SetAlpha(alpha);
            }
        #endregion
       
    }
}