using System;
using System.Collections.Generic;
using System.Linq;
using AirCoder.TJ.Core.Jobs;
using AirCoder.TJ.Core.Sequences;
using UnityEngine;

namespace AirCoder.TJ.Core
{
    public enum UpdateMethod
    {
        Update, FixedUpdate, LateUpdate
    }
    
    public class TJSystem: BaseMonoBehaviour
    {
        public static TJSystem Instance;
        private static HashSet<ITweenJob> _jobsInProgress;
        private static HashSet<ITweenSequence> _sequencesInProgress;
        
        public UpdateMethod method;

        protected override void ReleaseReferences()
        {
            
        }

        private void Awake()
        {
            //TODO : make it dynamic singleton
            if (Instance != null)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }
            Instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            if(method != UpdateMethod.Update) return;
            Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if(method != UpdateMethod.FixedUpdate) return;
            Tick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            if(method != UpdateMethod.LateUpdate) return;
            Tick(Time.deltaTime);
        }

        private void Tick(float deltaTime)
        {
            if (_jobsInProgress != null && _jobsInProgress?.Count > 0)
            {
                foreach (var job in _jobsInProgress.ToList())
                {
                    if(job.isBelongsToSequence) continue;
                    job.Tick(deltaTime);
                }
            }
            if(_sequencesInProgress == null || _sequencesInProgress.Count == 0) return;
                foreach (var sequence in _sequencesInProgress.ToList())
                    sequence.Tick(deltaTime);
        }

        public static ITweenSequence Sequence(SequenceType sequenceType)
        {
            var sequence = GetSequence(sequenceType);
            return sequence;
        }

        public static ITweenJob Tween<T>(T targetReference, JObType job, params object[] parameters)
        {
            var tweenJob = GetTweenJob<T>();
            tweenJob.TweenTo<T>(targetReference, job, parameters);
            return tweenJob;
        }
        
        private static ITweenJob GetTweenJob<T>()
        {
            if(_jobsInProgress == null) _jobsInProgress = new HashSet<ITweenJob>();
            var job = JobPool.CanSpawnJob<T>() 
                    ? JobPool.SpawnJob<T>() 
                    : JobFactory.CreateJob<T>(); 

            if (job == null) throw new Exception($"unsupported type {typeof(T)} !!");
            
            _jobsInProgress.Add(job);
            job.Reset();
            job.OnComplete(() => { JobComplete(job); });
            job.OnKill(() => { JobComplete(job); });
            return job;
        }
        
        private static void JobComplete(ITweenJob job)
        {
            if(job.isBelongsToSequence) return;
                _jobsInProgress.Remove(job);
                JobPool.DespawnJob(job);
        }
        
        private static ITweenSequence GetSequence(SequenceType sequenceType)
        {
            if(_sequencesInProgress == null) _sequencesInProgress = new HashSet<ITweenSequence>();
            var sequence = JobPool.CanSpawnSequence()
                ? JobPool.SpawnSequence(sequenceType)
                : JobFactory.CreateSequence(Instance, sequenceType);
            
            _sequencesInProgress.Add(sequence);
            sequence.OnComplete(() => { SequenceComplete(sequence); });
            sequence.OnKill(() => { SequenceComplete(sequence); });
            return sequence;
        }

        private static void SequenceComplete(ITweenSequence sequence)
        {
            var jobs = sequence.GetJobs();
            if(jobs != null && jobs.Count > 0)
                foreach (var job in jobs)
                {
                    job.isBelongsToSequence = false;
                    JobComplete(job);
                }
            
            _sequencesInProgress.Remove(sequence);
            JobPool.DespawnSequence(sequence);
        }
    }
}