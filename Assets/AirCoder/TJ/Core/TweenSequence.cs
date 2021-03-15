using System;
using AirCoder.TJ.Core.Ease;
using UnityEngine;

namespace AirCoder.TJ.Core
{
    
    [System.Serializable]
    public class ScaleSequenceJob
    {
        public float duration;
        public Vector3 targetScale;
        public Easing.EaseType ease;
        
        public void Execute()
        {
            Type t = Type.GetType("SequenceJob", throwOnError:true);
            var a = Activator.CreateInstance(t);
        }
    }
    
}
