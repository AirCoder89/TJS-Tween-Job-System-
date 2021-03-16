using AirCoder.TJ.Core;
using AirCoder.TJ.Core.Ease;
using AirCoder.TJ.Core.Extensions;
using AirCoder.TJ.Core.Jobs;
using UnityEngine;
using UnityEngine.UI;

namespace AirCoder.Examples.Scripts
{
    public class TestUi : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private CanvasGroup cGroup;
        [SerializeField] private Transform rt;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 targetScale;
        [SerializeField] private EaseType ease;
        
        [ContextMenu("Test")]
        private void Test()
        {
          
        }

        private ITweenJob _job;
        [ContextMenu("Test 2")]
        private void Test2()
        {
            _job = text.TweenColor(Color.red);
            _job.Play(true);
        }

        public void Sample()
        {
            Debug.Log($"Completed !!");
        }
    }
}
