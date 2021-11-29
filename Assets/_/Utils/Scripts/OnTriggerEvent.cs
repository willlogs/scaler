using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DB.Utils
{
    public class OnTriggerEvent : MonoBehaviour
    {
        public LayerMask layerMask;
        public UnityEvent<Collider> OnEnter, OnExit, OnStay;

        [SerializeField] private bool _checkForDisabled = false;
        private List<Collider> colliders = new List<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            int layerTest = layerMask.value & (1 << other.gameObject.layer);
            if (layerTest > 0)
            {
                OnEnter?.Invoke(other);
                colliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            int layerTest = layerMask.value & (1 << other.gameObject.layer);
            if (layerTest > 0)
            {
                OnExit?.Invoke(other);
                colliders.Remove(other);
            }
        }

        private void Update()
        {
            if (_checkForDisabled)
            {
                for (int i = colliders.Count - 1; i >= 0; i--)
                {
                    Collider collider = colliders[i];
                    if (!collider.enabled)
                    {
                        OnExit?.Invoke(collider);
                        colliders.Remove(collider);
                    }
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            int layerTest = layerMask.value & (1 << other.gameObject.layer);
            if (layerTest > 0)
            {
                OnStay?.Invoke(other);
            }
        }
    }
}