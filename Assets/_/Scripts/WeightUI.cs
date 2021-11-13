using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DB.Scale
{
    public class WeightUI : MonoBehaviour
    {
        [SerializeField] private Image _uiElement;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private float[] _ranges;

        private void Update()
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
            for(; i < _ranges.Length; i++)
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