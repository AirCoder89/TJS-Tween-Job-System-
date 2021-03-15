using System;
using AirCoder.TJ.Core.Ease;

namespace AirCoder.TJ.Core.Jobs
{
    public enum JObType
    {
        Config, Scale, Position, Rotation, Opacity, Color, FillAmount, Offset
    }
    public delegate void onUpdateEvent(float normalizedTime);
    public interface ITweenJob
    {
        float normalizedTime { get; set; }
        float jobDuration { get; set; }
        bool isPlaying { get; set; }
        bool isBelongsToSequence { get; set; }
        Type currentType { get; set; }
        
        void Initialize(Type type);
        ITweenJob TweenTo<T>(T targetInstance,JObType job, params object[] parameters);
        void Tick(float deltaTime);
        void Play();
        void Kill();
        void Reset();
        void SetDuration(float duration);
        
        //chained events subscribe
        ITweenJob SetEase(Easing.EaseType easeType);
        ITweenJob OnKill(Action callback);
        ITweenJob OnPlay(Action callback);
        ITweenJob OnUpdate(onUpdateEvent callback);
        ITweenJob OnComplete(Action callback);
        ITweenJob OnComplete(ITweenJob nextJob);
    }

}