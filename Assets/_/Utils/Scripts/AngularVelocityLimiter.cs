using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB.Utils
{
    public class AngularVelocityLimiter : MonoBehaviour
    {
        Rigidbody rb;

        [SerializeField] private float _cap = 10f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            rb.angularVelocity = rb.angularVelocity.normalized *
                Mathf.Clamp(rb.angularVelocity.magnitude, 0, _cap);
        }
    }
}