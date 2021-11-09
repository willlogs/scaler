using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB.Utils
{
	[RequireComponent(typeof(Rigidbody))]
	public class FloatingObject : MonoBehaviour
	{
		public event Action OnStopDrag;

		public Rigidbody rb;
		public bool isBeingDragged = false;
		public Quaternion starterRotation;

		public void StartDrag()
        {
			isBeingDragged = true;
        }

		public void StopDrag()
        {
			isBeingDragged = false;
			OnStopDrag?.Invoke();
		}

		[SerializeField] private float _waterHeight = 0f;
		[SerializeField] private float _gravityIntensity = 5f;
		[SerializeField] private float _surfaceThreshold = 0.5f;

        private void Awake()
        {
			rb = GetComponent<Rigidbody>();
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
					rb.velocity += Time.fixedDeltaTime * _gravityIntensity / 2 * -Mathf.Sign(diff) * Vector3.down;
				}
			}
		}
	}
}