using DB.Scale;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DB.Utils
{
	[RequireComponent(typeof(Rigidbody))]
	public class FloatingObject : MonoBehaviour
	{
		public UnityEvent OnStartDragUE, OnStopDragUE;
		public event Action OnStopDrag;

		public Rigidbody rb;
		public bool isBeingDragged = false;
		public Quaternion starterRotation;
		public Hanger hanger;

		public void StartDrag()
        {
			isBeingDragged = true;

            if(!hanger._isHanger)
                _meshCollider.enabled = false;

			OnStartDragUE?.Invoke();
        }

		public void StopDrag()
        {
			isBeingDragged = false;
			OnStopDrag?.Invoke();

            if(!hanger._isHanger)
                _meshCollider.enabled = true;

			OnStopDragUE?.Invoke();
		}

		[SerializeField] private float _waterHeight = 0f;
		[SerializeField] private float _gravityIntensity = 5f;
		[SerializeField] private float _surfaceThreshold = 0.5f;
        [SerializeField] private Collider _meshCollider;

        private void Awake()
        {
            _meshCollider = GetComponentInChildren<MeshCollider>();
			rb = GetComponent<Rigidbody>();
            hanger = GetComponentInChildren<Hanger>();
            hanger.RegisterFO(this);
        }

        private void Start()
		{
			rb.useGravity = false;
			starterRotation = transform.rotation;
		}

		private void FixedUpdate()
		{
			if (!isBeingDragged)
			{
				float diff = transform.position.y - _waterHeight;
				if (Mathf.Abs(diff) > _surfaceThreshold)
					rb.velocity += Time.fixedDeltaTime * _gravityIntensity * (
						transform.position.y < 0
						?
						Vector3.up
						:
						transform.position.y > 0
						?
						Vector3.down
						:
						Vector3.zero
					);
				else
				{
					//rb.velocity += Time.fixedDeltaTime * _gravityIntensity / 2 * -Mathf.Sign(diff) * Vector3.down;
				}
			}
		}
	}
}
