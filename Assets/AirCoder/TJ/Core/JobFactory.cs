using AirCoder.TJ.Core.Jobs;
using AirCoder.TJ.Core.Sequences;
using UnityEngine;

namespace AirCoder.TJ.Core
{
    public static class JobFactory
    {
        public static ITweenJob CreateJob<T>()
        {
            Debug.Log($"Create Job of type {typeof(T).Name}");
            ITweenJob job = null;
            
            if (typeof(RectTransform) == typeof(T))                  job = new RectTransformJob();
            else if (typeof(UnityEngine.Graphics) == typeof(T))      job = new GraphicsJob();
            else if (typeof(UnityEngine.UI.Graphic) == typeof(T))    job = new GraphicsJob();
            else if (typeof(Material) == typeof(T))                  job = new MaterialJob();
            
            job?.Initialize(typeof(T));
            return job;
        }

        public static ITweenSequence CreateSequence(TJSystem system, SequenceType seqType)
        {
           return new SequenceJob(system, seqType);
        }
    }
}