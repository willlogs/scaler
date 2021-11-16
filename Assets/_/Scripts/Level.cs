using DB.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DB.Scale{
    public class Level : MonoBehaviour
    {
        public UnityEvent OnLevelSuccess, OnError, OnSpawn;
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
        [SerializeField] private float _beforeWinning = 3f, _betweenSpawans = 1f;
        [SerializeField] private List<FloatingObject> _objects;
        [SerializeField] private int _notHangingObjects = 0;
        [SerializeField] private GameObject _textPrefab, _spawnFXPrefab;

        private void Awake(){
            _objects = new List<FloatingObject>();
            foreach(FloatingObject fo in FindObjectsOfType<FloatingObject>()){
                if(fo.gameObject.activeSelf){
                    _objects.Add(fo);
                }
            }
        }

        private void Start()
        {
            _notHangingObjects = _objects.Count;

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
                obj.gameObject.SetActive(false);
            }

            StartCoroutine(SpawnObjects());
        }

        private IEnumerator SpawnObjects()
        {
            foreach (FloatingObject obj in _objects)
            {
                yield return new WaitForSeconds(_betweenSpawans);

                GameObject go = Instantiate(_spawnFXPrefab);
                go.transform.position = obj.transform.position;

                obj.gameObject.SetActive(true);
                OnSpawn?.Invoke();
            }
        }

        private void OnAttachOne()
        {
            _notHangingObjects--;
        }

        private void OnDetachOne()
        {
            _notHangingObjects++;
        }

        private void Update()
        {
            for(int i = _touchingWater.Count - 1; i >= 0; i--){
                if(!_touchingWater[i].enabled){
                    _touchingWater.RemoveAt(i);
                }
            }

            if(_touchingWater.Count <= 0 && _notHangingObjects <= 0)
            {
                StartCoroutine(StartLevelEnd());
            }
        }

        private bool _counting;
        private IEnumerator StartLevelEnd(){
            if (!_counting)
            {
                _counting = true;

                bool _timeTick = false;
                bool fail = true;
                float time = 0;
                int sec = 2;
                while (_touchingWater.Count <= 0 && _notHangingObjects <= 0)
                {
                    yield return new WaitForEndOfFrame();
                    time += Time.deltaTime;

                    float trimTime = time % 1f;
                    if (trimTime < 0.1)
                    {
                        if (!_timeTick && sec >= 0)
                        {
                            _timeTick = true;
                            GameObject go = Instantiate(_textPrefab);
                            go.transform.position = transform.position;
                            go.GetComponent<TMPro.TextMeshPro>().text = (sec + 1) + "";
                            sec--;
                        }
                    }
                    else
                    {
                        _timeTick = false;
                    }

                    if (time > _beforeWinning)
                    {
                        OnLevelSuccess?.Invoke();
                        fail = false;
                        break;
                    }
                }

                if (fail)
                {
                    if(_notHangingObjects <= 0)
                        OnError?.Invoke();
                    _counting = false;
                }
            }
        }
    }
}
