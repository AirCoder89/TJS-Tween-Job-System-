using System;
using System.Collections.Generic;
using System.Linq;
using AirCoder.TJ.Core;
using AirCoder.TJ.Core.Components;
using AirCoder.TJ.Core.Ease;
using UnityEditor;
using UnityEngine;

namespace AirCoder.TJ.Editor
{
    [CustomEditor(typeof(TweenJob))]
    
    public class TweenJobComponentEditor : UnityEditor.Editor
    {
        private Dictionary<string , int> _types;
        private string _targetTween;
        private FieldsName _targetField = FieldsName.None;
        SerializedObject _targetObject;
        
        
        private int _tabIndex = 0;
        private List<string> _tabs;
        private SerializedProperty _interpolation;
        
        private Dictionary<string , int> availableTypes
        {
            get
            {
                if (_types == null)
                {
                    _types = Enum.GetValues(typeof(AvailableTypes))
                        .Cast<AvailableTypes>()
                        .ToDictionary(t => t.ToString(), t => (int) t);
                }
                return _types;
            }
        }
        
        void OnEnable()
        {
            _targetObject= new SerializedObject((TweenJob)target);
            _interpolation = serializedObject.FindProperty("interpolation");
        }

        public override void OnInspectorGUI()
        {
            var script = (TweenJob) target;
           
            if(script == null || script.components == null || script.components.Count == 0) return;

            DrawJobController(script);
            
            DrawAvailableTween(script);

            DrawJobParameters(script);

            DrawTweenEvents(script);
        }

        
        private void DrawAvailableTween(TweenJob script)
        {
            if(_tabs == null) _tabs = new List<string>();
            _tabs.Clear();

            GUILayout.Space(10);
            DrawTitle("Available Jobs :");
            StartBox(true);
            GUI.backgroundColor  = Color.cyan;
            foreach (var component in script.components)
            {
                var methodName = GetName(component.ToString());
                if(methodName == null || methodName.Count == 0) continue;
                foreach (var tweenName in methodName)
                {
                    _tabs.Add(tweenName);
                }
            }
            
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabs.ToArray(),GUILayout.Height(25));
            _targetTween = _tabs[_tabIndex];
            script.SetupTween(_targetTween);
            GUI.backgroundColor  = Color.white;
            EndBox(true);
        }

        private void DrawJobController(TweenJob script)
        {
            Handles.BeginGUI();
            GUILayout.Space(10);
            DrawTitle("Job Controller :");
            StartBox(true);
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor  = Color.blue;
            if (GUILayout.Button("Play", GetControlButtonStyle()))
            {
                script.Play();
            }
            GUI.backgroundColor  = Color.magenta;

            if (script.IsPlaying())
            {
                if (GUILayout.Button("Pause", GetControlButtonStyle()))
                {
                    script.Pause();
                }
            }
            else
            {
                if (GUILayout.Button("Resume", GetControlButtonStyle()))
                {
                    script.Resume();
                }
            }
            GUI.backgroundColor  = Color.red;
            if (GUILayout.Button("Kill", GetControlButtonStyle()))
            {
                script.Kill();
            }
            
            GUI.backgroundColor  = Color.yellow;
            if (GUILayout.Button("Refresh", GetControlButtonStyle()))
            {
                script.Refresh();
            }
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.EndHorizontal();
            
            AddSeparator(1, false);

            
            ProgressBar(_interpolation.floatValue, "Interpolation:");
            Handles.EndGUI();
            if (GUI.changed) EditorUtility.SetDirty((TweenJob)target);
            EndBox(true);
        }

        private void DrawJobParameters(TweenJob script)
        {
            GUILayout.Space(10);
            DrawTitle("Job Parameters :");
            StartBox(true);
            if(script.fieldsStates.targetIsVector2)
                script.targetVector2 =
                    EditorGUILayout.Vector2Field($"Target {_targetTween}", script.targetVector2);
            
            if(script.fieldsStates.targetIsVector3)
                script.targetVector3 =
                    EditorGUILayout.Vector3Field($"Target {_targetTween}", script.targetVector3);
            
            if(script.fieldsStates.targetIsColor)
                script.targetColor =
                    EditorGUILayout.ColorField($"Target Color", script.targetColor);

            if (script.fieldsStates.targetIsFloat)
            {
                script.targetFloat =
                    EditorGUILayout.Slider($"Target {_targetTween}", script.targetFloat, 0, 1);
            }
            if (script.fieldsStates.targetString)
            {
                script.targetString =
                    EditorGUILayout.TextField($"Target {script.fieldsStates.targetString}", script.targetString);
                // EditorGUILayout.Slider($"Target {_targetTween}", script.targetFloat, 0, 1);
            }
               
            script.duration = EditorGUILayout.FloatField("Duration : ", script.duration);
            script.ease = (EaseType) EditorGUILayout.EnumPopup("Ease : ", script.ease);
            script.rewind = EditorGUILayout.Toggle("Rewind : ", script.rewind);
            EndBox(true);
        }

        private void DrawTweenEvents(TweenJob script)
        {
            GUILayout.Space(10);
            DrawTitle("Tween Events : ");

            StartBox(true);
            if (script.eventsStates.IsEnabled() || script.eventsStates.enable)
            {
                UpdateOnCompleteEvent(script);
                UpdateOnUpdateEvent(script);
                UpdateOnPlayEvent(script);
                UpdateOnPauseEvent(script);
                UpdateOnResumeEvent(script);
                UpdateOnKillEvent(script);
            }
            else
            {
                var style = GetEventButtonStyle();
                GUI.backgroundColor  = Color.green;
                
                if (GUILayout.Button("Add Events", style))
                {
                    script.eventsStates.enable = true;
                }
                GUI.backgroundColor  = Color.white;
            }
            EndBox(true);
        }

        private void ProgressBar (float value, string label)
        {
            EditorGUILayout.LabelField(label);
            Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField",GUILayout.MaxHeight(12f), GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar (rect, value, value.ToString("P"));
            EditorGUILayout.Space ();
        }
        
        private GUIStyle GetEventButtonStyle()
        {
            return new GUIStyle(GUI.skin.button) {fixedHeight = 30, fontSize = 13,alignment = TextAnchor.MiddleCenter};
        }
        
        private GUIStyle GetControlButtonStyle()
        {//
            return new GUIStyle(GUI.skin.button) {fixedHeight = 20, fontSize = 11,alignment = TextAnchor.MiddleCenter};
        }
        
        private GUIStyle GetBoxStyle()
        {
            return  GUI.skin.GetStyle("HelpBox");
        }

        private void StartBox(bool space = false)
        {
            EditorGUILayout.BeginVertical(GetBoxStyle());
            if(space) EditorGUILayout.Space(5);
        }

        private void EndBox(bool space = false)
        {
            if(space) EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }
        
        private void UpdateOnCompleteEvent(TweenJob script)
        {
            var style = GetEventButtonStyle();

            EditorGUILayout.Space(5);
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.BeginHorizontal();

            if (script.eventsStates.onComplete)
            {
                script.onComplete = null;
                   
                var prop = _targetObject.FindProperty("onComplete");
                EditorGUILayout.PropertyField(prop, true);
                _targetObject.ApplyModifiedProperties();
                    
                GUI.backgroundColor  = Color.red;
                if (GUILayout.Button("Remove", style))
                {
                    script.eventsStates.onComplete = false;
                    script.eventsStates.enable = script.eventsStates.IsEnabled();
                }
            }
            else
            {
                GUI.backgroundColor  = Color.green;
                if (GUILayout.Button("OnComplete Event", style))
                {
                    script.eventsStates.onComplete = true;
                    script.eventsStates.enable = script.eventsStates.IsEnabled();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateOnUpdateEvent(TweenJob script)
        {
            string buttonName;
            var style = GetEventButtonStyle();
            
            EditorGUILayout.Space(5);
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (script.eventsStates.onUpdate)
            {
                buttonName = "Remove";
                script.onUpdate = null;
                   
                var prop = _targetObject.FindProperty("onUpdate");
                EditorGUILayout.PropertyField(prop, true);
                _targetObject.ApplyModifiedProperties();
                    
                GUI.backgroundColor  = Color.red;
            }
            else
            {
                buttonName = "OnUpdate Event";
                GUI.backgroundColor  = Color.green;
            }
            
            if (GUILayout.Button(buttonName,style))
            {
                script.eventsStates.onUpdate = !script.eventsStates.onUpdate;
                script.eventsStates.enable = script.eventsStates.IsEnabled();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateOnPlayEvent(TweenJob script)
        {
            string buttonName;
            var style = GetEventButtonStyle();
            
            EditorGUILayout.Space(5);
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (script.eventsStates.onPlay)
            {
                buttonName = "Remove";
                script.onUpdate = null;
                   
                var prop = _targetObject.FindProperty("onPlay");
                EditorGUILayout.PropertyField(prop, true);
                _targetObject.ApplyModifiedProperties();
                    
                GUI.backgroundColor  = Color.red;
            }
            else
            {
                buttonName = "OnPlay Event";
                GUI.backgroundColor  = Color.green;
            }
            
            if (GUILayout.Button(buttonName,style))
            {
                script.eventsStates.onPlay = !script.eventsStates.onPlay;
                script.eventsStates.enable = script.eventsStates.IsEnabled();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateOnPauseEvent(TweenJob script)
        {
            string buttonName;
            var style = GetEventButtonStyle();
            
            EditorGUILayout.Space(5);
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (script.eventsStates.onPause)
            {
                buttonName = "Remove";
                script.onUpdate = null;
                   
                var prop = _targetObject.FindProperty("onPause");
                EditorGUILayout.PropertyField(prop, true);
                _targetObject.ApplyModifiedProperties();
                    
                GUI.backgroundColor  = Color.red;
            }
            else
            {
                buttonName = "OnPause Event";
                GUI.backgroundColor  = Color.green;
            }
            
            if (GUILayout.Button(buttonName,style))
            {
                script.eventsStates.onPause = !script.eventsStates.onPause;
                script.eventsStates.enable = script.eventsStates.IsEnabled();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateOnResumeEvent(TweenJob script)
        {
            string buttonName;
            var style = GetEventButtonStyle();
            
            EditorGUILayout.Space(5);
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (script.eventsStates.onResume)
            {
                buttonName = "Remove";
                script.onUpdate = null;
                   
                var prop = _targetObject.FindProperty("onResume");
                EditorGUILayout.PropertyField(prop, true);
                _targetObject.ApplyModifiedProperties();
                    
                GUI.backgroundColor  = Color.red;
            }
            else
            {
                buttonName = "OnResume Event";
                GUI.backgroundColor  = Color.green;
            }
            
            if (GUILayout.Button(buttonName,style))
            {
                script.eventsStates.onResume = !script.eventsStates.onResume;
                script.eventsStates.enable = script.eventsStates.IsEnabled();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateOnKillEvent(TweenJob script)
        {
            string buttonName;
            var style = GetEventButtonStyle();
            
            EditorGUILayout.Space(5);
            GUI.backgroundColor  = Color.white;
            EditorGUILayout.BeginHorizontal();
            if (script.eventsStates.onKill)
            {
                buttonName = "Remove";
                script.onUpdate = null;
                   
                var prop = _targetObject.FindProperty("onKill");
                EditorGUILayout.PropertyField(prop, true);
                _targetObject.ApplyModifiedProperties();
                    
                GUI.backgroundColor  = Color.red;
            }
            else
            {
                buttonName = "OnKill Event";
                GUI.backgroundColor  = Color.green;
            }
            
            if (GUILayout.Button(buttonName,style))
            {
                script.eventsStates.onKill = !script.eventsStates.onKill;
                script.eventsStates.enable = script.eventsStates.IsEnabled();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTitle(string title)
        {
            var titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 14;
            EditorGUILayout.LabelField(title, titleStyle);
        }
        
        private void AddSeparator( int i_height = 1, bool space = false )
        {
            if(space) EditorGUILayout.Space(5);
            Rect rect = EditorGUILayout.GetControlRect(false, i_height );
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            if(space) EditorGUILayout.Space(5);
        }
        
        //- return list of available tween of the given type
        private List<string> GetName(string fullNameSpace)
        {
            var splitName = fullNameSpace.Split(char.Parse(".")).ToList();
            var typeName = splitName[splitName.Count - 1];
            //var superTypeName = splitName[splitName.Count - 2];

            if (availableTypes.ContainsKey(typeName))
            {
                return GetTweenList((AvailableTypes) availableTypes[typeName]);
            }
            return null;
        }

        private List<string> GetTweenList(AvailableTypes type)
        {
            switch (type)
            {
                case AvailableTypes.Image: return GetImageTween();
                case AvailableTypes.RectTransform: return GetRectTransformTween();
            }
            return null;
        }

        private List<string> GetImageTween()
        {
            return new List<string>()
            {
                TweenJobList.Opacity.ToString(),
                TweenJobList.Color.ToString(),
            };
        }
        private List<string> GetRectTransformTween()
        {
            return new List<string>()
            {
                TweenJobList.Scale.ToString(),
                TweenJobList.Rotation.ToString(),
                TweenJobList.Position.ToString()
            };
        }
    }
}
