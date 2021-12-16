using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace DB.Scale
{
    public class WeightUI : MonoBehaviour
    {
        public UnityEvent OnMustLose, OnTouchWater;

        public void WaterTouchActivate()
        {
            _freeze = true;
            _uiElement.sprite = _waterTouch;
            OnTouchWater?.Invoke();
            StartCoroutine(CountToLose());
        }

        public void WaterTouchDeactivate()
        {
            _freeze = false;
        }

        [SerializeField] private Image _uiElement;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private Sprite _waterTouch;
        [SerializeField] private float[] _ranges;
        [SerializeField] private GameObject _textMessagePrefab;

        private bool _freeze = false;

        private IEnumerator CountToLose()
        {
            bool _showedCount = false;
            int _counted = 3;
            float time = 0;
            while (time < 3 && _freeze)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;

                if(time % 1 < 0.1f)
                {
                    if (!_showedCount)
                    {
                        _showedCount = true;
                        /*GameObject textgo = Instantiate(_textMessagePrefab);
                        TextMeshPro text = textgo.GetComponent<TextMeshPro>();*/
                        /*text.text = _counted-- + "";*/
                    }
                }
                else
                {
                    _showedCount = false;
                }
            }

            if (_freeze)
            {
                OnMustLose?.Invoke();
            }
        }

        private void Update()
        {
            if (!_freeze)
            {
                /*_uiElement.transform.rotation = new Quaternion(
                    0,
                    0,
                    transform.rotation.z,
                    transform.rotation.w
                );*/

                int i = 0;
                bool flag = false;
                float rot = Mathf.Abs(transform.rotation.z);
                for (; i < _ranges.Length; i++)
                {
                    if (_ranges[i] > rot)
                    {
                        flag = true;
                        break;
                    }
                }
                _uiElement.sprite = flag ? _sprites[i] : _sprites[_sprites.Length - 1];
            }
        }
    }
}