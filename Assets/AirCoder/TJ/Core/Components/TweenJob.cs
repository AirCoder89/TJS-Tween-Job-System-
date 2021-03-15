using System;
using System.Collections.Generic;
using AirCoder.TJ.Core.Ease;
using AirCoder.TJ.Core.Extensions;
using AirCoder.TJ.Core.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AirCoder.TJ.Core.Components
{
    public enum TJEvents
    {
        OnComplete, OnPlay, OnPause, OnResume, OnKill, OnUpdate
    }
    public enum FieldsName
    {
        None, Vector2, Vector3, Float, Color, String
    }
    public enum AvailableTypes
    {
        RectTransform, Image, Transform, SpriteRenderer, Material
    }
    public enum TweenJobList
    {
        Scale, Position, Rotation, Opacity, Color, Material
    }

    [System.Serializable]
    public struct TweenFieldStates
    {
        public bool targetIsVector2;
        public bool targetIsVector3;
        public bool targetIsFloat;
        public bool targetIsColor;
        public bool targetString;
        public string targetStringLabel;

        public void ResetStates()
        {
            targetIsVector2 = false;
            targetIsVector3 = false;
            targetIsFloat = false;
            targetIsColor = false;
            targetString = false;
            targetStringLabel = string.Empty;
        }
    }
    
    [System.Serializable]
    public struct TweenEventsStates
    {
        public bool enable;
        public bool onComplete;
        public bool onPlay;
        public bool onPause;
        public bool onResume;
        public bool onKill;
        public bool onUpdate;

        public void ResetStates()
        {
            onComplete = false;
            onPlay = false;
            onPause = false;
            onResume = false;
            onKill = false;
            onUpdate = false;
        }

        public bool IsEnabled()
        {
            return onComplete || onPlay || onPause || onResume || onKill || onUpdate;
        }
    }
    
    [System.Serializable]
    public class TweenJobEvent : UnityEvent<JobTrackingData>{}
    
    [ExecuteInEditMode]
    public class TweenJob:MonoBehaviour
    {
        public bool rewind;
        public float duration = 1f;
        public EaseType ease;

        public float targetFloat;
        public Vector2 targetVector2;
        public Vector3 targetVector3;
        public Color targetColor;
        public string targetString;
        
        public List<Type> components;
        private ITweenJob _job;

        public TweenFieldStates fieldsStates;
        public TweenEventsStates eventsStates;

        public UnityEvent onComplete;
        public TweenJobEvent onUpdate;
        public UnityEvent onPlay;
        public UnityEvent onPause;
        public UnityEvent onResume;
        public UnityEvent onKill;

        public float interpolation;
        
        private Action _action;
        private void OnEnable()
        {
            Refresh();
        }

        private void Start()
        {
            interpolation = 0f;
        }

        //TODO : expose via button
        public void Refresh()
        {
            components = new List<Type>();
            foreach(var component in GetComponents<Component>())
            {
                if(component != this) components.Add(component.GetType());
            }
        }

        private void InitializeJob()
        {
            _job.OnUpdate(OnUpdateJob)
                .SetEase(ease)
                .OnComplete(OnJobComplete)
                .OnPlay(OnJobPlayed)
                .OnPause(OnJobPaused)
                .OnResume(OnJobResumed)
                .OnKill(OnJobKilled);
        }
        
        public void SetupTween(string tweenName)
        {
            interpolation = 0f;
            if (tweenName == TweenJobList.Scale.ToString())
            {
                TweenScale();
                UpdateTargetField(FieldsName.Vector3);
            }
            else if (tweenName == TweenJobList.Color.ToString())
            {
                TweenColor();
                UpdateTargetField(FieldsName.Color);
            }
            else if (tweenName == TweenJobList.Opacity.ToString())
            {
                TweenOpacity();
                UpdateTargetField(FieldsName.Float);
            }
            else if (tweenName == TweenJobList.Position.ToString())
            {
                TweenPosition();
                UpdateTargetField(FieldsName.Vector3);
            }
            else if (tweenName == TweenJobList.Rotation.ToString())
            {
                TweenRotation();
                UpdateTargetField(FieldsName.Vector3);
            }
            else UpdateTargetField(FieldsName.None);
        }

        private void UpdateTargetField(params FieldsName[] targetField)
        {
            fieldsStates.ResetStates();
            foreach (var fieldsName in targetField)
            {
                switch (fieldsName)
                {
                    case FieldsName.Color: fieldsStates.targetIsColor = true; break;
                    case FieldsName.Vector3: fieldsStates.targetIsVector3 = true; break;
                    case FieldsName.Float: fieldsStates.targetIsFloat = true; break;
                    case FieldsName.String: fieldsStates.targetIsFloat = true; break;
                }
            }
        }
        
        public bool CanPlay()
        {
            return _action != null;
        }
        
        public void Play()
        {
            _action?.Invoke();
        }

        public bool IsPlaying()
        {
            if (_job == null) return false;
            return _job.isPlaying;
        }

        public void Pause() => _job?.Pause();
        public void Resume() => _job?.Resume();
        public void Kill() => _job?.Kill();

        #region Job Assignment
            private void TweenScale()
            {
                _action = () =>
                {
                    _job = GetComponent<RectTransform>().TweenScale((Vector2)targetVector3, duration);
                    InitializeJob();
                    _job.Play(rewind);
                };
            }
            private void TweenPosition()
            {
                _action = () =>
                {
                    _job = GetComponent<RectTransform>().TweenAnchorPosition(targetVector3, duration);
                    InitializeJob();
                    _job.Play(rewind);
                };
            }
            
            private void TweenRotation()
            {
                _action = () =>
                {
                    _job = GetComponent<RectTransform>().TweenRotation((Vector2)targetVector3, duration);
                    InitializeJob();
                    _job.Play(rewind);
                };
            }
            
            private void TweenColor()
            {
                _action = () =>
                {
                    _job = GetComponent<Image>().TweenColor(targetColor, duration);
                    InitializeJob();
                    _job.Play(rewind);
                };
            }

            private void TweenOpacity()
            {
                _action = () =>
                {
                    _job = GetComponent<Image>().TweenOpacity(targetFloat, duration);
                    InitializeJob();
                    _job.Play(rewind);
                };
            }

            
        #endregion 
        

        #region Event Handlers
            private void OnJobKilled()
            {
                onKill?.Invoke();
            }
    
            private void OnJobResumed()
            {
                onResume?.Invoke();
            }
    
            private void OnJobPaused()
            {
                onPause?.Invoke();
            }
    
            private void OnJobPlayed()
            {
                onPlay?.Invoke();
            }
    
            private void OnJobComplete()
            {
                print("Job Complete !");
                onComplete?.Invoke();
            }
    
            private void OnUpdateJob(JobTrackingData jobData)
            {
                interpolation = jobData.normalizedTime;
                onUpdate?.Invoke(jobData);
            }
        #endregion
    }
}