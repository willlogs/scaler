using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB.Scale
{
    public class MassTweaker : MonoBehaviour
    {
        public float _mass;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _sinusRadius, _sinScale;

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _time = Random.Range(0f, 0.99f);
            _rb.centerOfMass = _rb.centerOfMass + Vector3.forward * Random.Range(-0.5f, 0.5f);
        }

        private void Update()
        {
            float mass = _rb.mass;

            _rb.mass = _mass + Mathf.Sin(_time * _sinScale) * _sinusRadius;
            if (_rb.mass < 1f)
            {
                _rb.mass = 1f;
            }
            _time += Time.deltaTime;
            _time %= Mathf.PI * 2 / _sinScale;
        }
    }
}