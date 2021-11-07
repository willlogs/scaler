using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooksManager : MonoBehaviour
{
    [SerializeField] private Text _weightText;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Gradient _color;
    [SerializeField] private float _sinusRadius, _sinScale, _mass, _weightCap;

    private float _time;

    private void Start()
    {
        _time = Random.Range(0f, 0.99f);
        _rb.centerOfMass = _rb.centerOfMass + Vector3.forward * Random.Range(-0.5f, 0.5f);
    }

    private void Update()
    {
        float mass = _rb.mass;
        _weightText.text = _mass + "";
        _renderer.material.SetColor("_Color", _color.Evaluate(mass / _weightCap));

        _rb.mass = _mass + Mathf.Sin(_time * _sinScale) * _sinusRadius;
        if(_rb.mass < 1f)
        {
            _rb.mass = 1f;
        }
        _time += Time.deltaTime;
        _time %= Mathf.PI * 2 / _sinScale;
    }
}
