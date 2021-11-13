using DB.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DB.Scale{
    public class Level : MonoBehaviour
    {
        public UnityEvent OnLevelSuccess;
        public UnityAction EndLevelAction, DetachAction;

        public void OnSomethingTouchesWater(Collider other){
            if(!_touchingWater.Contains(other)){
                _touchingWater.Add(other);
            }
        }

        public void OnSomethingGetsOutofWater(Collider other){
            if(_touchingWater.Contains(other)){
                _touchingWater.Remove(other);
            }
        }

        [SerializeField] private List<Collider> _touchingWater;
        [SerializeField] private float _beforeWinning = 3f;
        [SerializeField] private FloatingObject[] _objects;
        [SerializeField] private int _notHangingObjects = 0;

        private void Start()
        {
            _notHangingObjects = _objects.Length;

            EndLevelAction += () =>
            {
                OnAttachOne();
                StartCoroutine(StartLevelEnd());
            };

            DetachAction += OnDetachOne;

            foreach (FloatingObject obj in _objects)
            {
                obj.hanger.OnFullyHanged.AddListener(EndLevelAction);
                obj.hanger.OnFullyDetach.AddListener(DetachAction);
            }

            StartCoroutine(Tick());
        }

        private void OnAttachOne()
        {
            _notHangingObjects--;
        }

        private void OnDetachOne()
        {
            _notHangingObjects++;
        }

        private IEnumerator Tick()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if(_touchingWater.Count <= 0 && _notHangingObjects <= 0)
                {
                    StartCoroutine(StartLevelEnd());
                }
            }
        }

        private IEnumerator StartLevelEnd(){
            float time = 0;
            while(_touchingWater.Count <= 0 && _notHangingObjects <= 0){
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
                if(time > _beforeWinning){
                    OnLevelSuccess?.Invoke();
                    break;
                }
            }
        }
    }
}
