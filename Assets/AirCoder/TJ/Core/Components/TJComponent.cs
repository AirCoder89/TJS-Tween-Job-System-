using AirCoder.TJ.Core.Jobs;
using UnityEngine;
using UnityEngine.Events;

namespace AirCoder.TJ.Core.Components
{
    
   
    [System.Serializable]
    public struct TweenEventHandler
    {
        public UnityEvent onComplete;
        public TweenJobEvent onUpdate;
        public UnityEvent onPlay;
        public UnityEvent onPause;
        public UnityEvent onResume;
        public UnityEvent onKill;
        
        public void ReleaseReferences()
        {
            onComplete = null;
            onUpdate = null;
            onPlay = null;
            onPause = null;
            onResume = null;
            onKill = null;
        }
    }
    public abstract class TJComponent: BaseMonoBehaviour
    {
        [Space(10)] 
        public float tweenProgression;

        [Space(20)] 
        
        public bool autoPlay;
        
        public float duration = 1f;
        

        public ITweenJob Job { get; set; }

        public TJComponent nextJob;
        public TweenEventHandler events;

        protected override void ReleaseReferences()
        {
            events.ReleaseReferences();
            Job = null;
            nextJob = null;
        }

        public void TestBtn()
        {
            print("Btn Clicked !!");
        }
        public abstract void Play();

        public void Pause()
        {
            Job?.Pause();
        }

        public void Resume()
        {
            Job?.Resume();
        }

        public void Kill()
        {
            Job?.Kill();
        }

        protected virtual void TweenComplete()
        {
            tweenProgression = 0f;
            PlayNext();
            events.onComplete?.Invoke();
        }

        private void PlayNext()
        {
            if(nextJob == null) return;
            nextJob.Play();
        }
        
    }
}