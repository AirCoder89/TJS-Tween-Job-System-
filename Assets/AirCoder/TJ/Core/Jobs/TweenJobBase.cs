using System;
using System.ComponentModel;
using AirCoder.TJ.Core.Ease;
using UnityEngine;

namespace AirCoder.TJ.Core.Jobs
{
   public abstract class TweenJobBase : ITweenJob
    {
        //- events
        public event Action onKill;
        public event Action onComplete;
        public event Action onPlay;
        public event Action onPause;
        public event Action onResume;
        public event onUpdateEvent onUpdate;
        
        //- properties
        public float normalizedTime { get; set; }

        public float jobDuration
        {
            get => Duration;
            set => Duration = value;
        }

        public bool isPlaying {
            get => IsPlaying;
            set => IsPlaying = value;
        }

        public bool isBelongsToSequence { get; set; }
        public Type currentType { get; set; }

        //- protected
        protected Easing.Function SelectedEase;
        protected float CurrentTime;
        protected bool IsPlaying;
        protected float Duration;
        protected JObType CurrentJobType;
        protected Action JobAction;

        public void Initialize(Type type)
        {
            currentType = type;
        }

        public abstract ITweenJob TweenTo<T>(T targetInstance, JObType job, params object[] parameters);

        public abstract void Tick(float deltaTime);
        
        public void Play()
        {
            if(isPlaying) return;
            CurrentTime = 0f;
            JobAction?.Invoke();
            isPlaying = true;
            onPlay?.Invoke();
        }

        public void Kill()
        {
            isPlaying = false;
            CurrentTime = 0f;
            onKill?.Invoke();
        }

        public void Reset()
        {
            RemoveEventListeners();
            CurrentTime = 0f;
            isPlaying = false;
            isBelongsToSequence = false;
        }

        public void SetDuration(float duration)
        {
            Duration = duration;
        }

        public virtual void ReleaseReferences()
        {
            //TODO : complete the reset
            RemoveEventListeners();
        }
        
        private void RemoveEventListeners()
        {
            onComplete = null;
            onKill = null;
            onPause = null;
            onResume = null;
            onUpdate = null;
            onPlay = null;
        }
        
        #region Event Invoker
            protected void RaiseOnTick()
            {
                onUpdate?.Invoke(this.normalizedTime);
            }
            protected void RaiseOnComplete()
            {
                CurrentTime = 0f;
                isPlaying = false;
                onComplete?.Invoke();
            }
            protected void RaiseOnPlay()
            {
                onPlay?.Invoke();
            }
            protected void RaiseOnKill()
            {
                onKill?.Invoke();
            }
        #endregion
        
        #region Helper Methods

         protected void CheckJobInterpolationComplete()
         {
             if (this.normalizedTime >= 1f)
             {
                 this.normalizedTime = 1f;
                 IsPlaying = false;
                 RaiseOnComplete();
             }
         }
            protected Color TweenColor(Color from, Color to, float time, float duration)
            {
                return new Color(
                    InterpolateFloat(from.r, to.r, time, duration),
                    InterpolateFloat(from.g, to.g, time, duration),
                    InterpolateFloat(from.b, to.b, time, duration),
                    InterpolateFloat(from.a, to.a, time, duration)
                );
            }
            
            protected Vector3 TweenVector3(Vector3 from, Vector3 to, float time, float duration)
            {
                return new Vector3(
                    InterpolateFloat(from.x, to.x, time, duration),
                    InterpolateFloat(from.y, to.y, time, duration),
                    InterpolateFloat(from.z, to.z, time, duration)
                );
            }
            
            protected float InterpolateFloat(float from, float to, float time, float duration)
            {
                if(SelectedEase == null) throw new Exception("SelectedEase is null");
                return SelectedEase(time, from, to, duration);
            }

            protected void ThrowInvalidJobType()
            {
                throw new InvalidEnumArgumentException($"CurrentJobType", (int)CurrentJobType, typeof(JObType));
            }

            protected void ThrowMissingReferenceException<T>(T targetType)
            {
                throw new MissingReferenceException($"{typeof(T)} it's no a valid type");
            }
        #endregion
        
        #region Chained Methods
        
        public ITweenJob OnKill(Action callback)
        {
            this.onKill += callback;
            return this;
        }

        public ITweenJob OnPlay(Action callback)
        {
            this.onPlay += callback;
            return this;
        }

        public ITweenJob OnUpdate(onUpdateEvent callback)
        {
            this.onUpdate += callback;
            return this;
        }

        public ITweenJob OnComplete(Action callback)
        {
            this.onComplete += callback;
            return this;
        }
        
        public ITweenJob OnComplete(ITweenJob nextJob)
        {
            if (nextJob == null) throw new NullReferenceException($"next tweenJob must be not null!");
            return OnComplete(nextJob.Play);
        }
        
        public ITweenJob SetEase(Easing.EaseType easeType)
        {
            this.SelectedEase = Easing.GetEase(easeType);
            return this;
        }
        public TweenJobBase OnPause(Action callback)
        {
            this.onPause += callback;
            return this;
        }
        public TweenJobBase OnResume(Action callback)
        {
            this.onResume += callback;
            return this;
        }
      
        #endregion
    }
}