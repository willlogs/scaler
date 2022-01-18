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
		public bool canBeDragged = true;

		public Vector3 restPos;

		public Transform menuTarget;
		public bool hasMenuTarget = false;

		public void SetUIImage(Transform target)
        {
			menuTarget = target;
			hasMenuTarget = true;
        }

		public void Activate()
        {
			rb.isKinematic = false;
        }

		public void Deactivate()
        {
			rb.isKinematic = true;
			transform.position = restPos;
			transform.localScale = Vector3.one * 0.1f;
			hanger.TryStopHang();
        }

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

			if (!hanger._isHanger)
			{
				_meshCollider.enabled = true;
			}

			OnStopDragUE?.Invoke();
		}

		[SerializeField] private float _waterHeight = 0f;
		[SerializeField] private float _gravityIntensity = 5f;
		[SerializeField] private float _surfaceThreshold = 0.5f;
        [SerializeField] private Collider _meshCollider;
		[SerializeField] private Transform _shade;
		[SerializeField] private SpriteRenderer _shadeSR;
		[SerializeField] private Vector3 _shadeOffset;

        private void Awake()
        {
            _meshCollider = GetComponentInChildren<MeshCollider>();
			rb = GetComponent<Rigidbody>();
            hanger = GetComponentInChildren<Hanger>();
            hanger.RegisterFO(this);
        }

        private void Start()
		{
			starterRotation = transform.rotation;
		}

		private void FixedUpdate()
		{
			if (!isBeingDragged)
			{

			}
		}

        private void Update()
        {
			if (canBeDragged)
			{
				_shade.position = new Vector3(transform.position.x, 0, transform.position.z) + _shadeOffset;
				_shade.up = Vector3.forward;

				if (transform.position.y > 0)
				{
					_shadeSR.color = new Color(1f, 1f, 1f, ((5 - transform.position.y) / 5) - 0.2f);
                }
                else
                {
					_shadeSR.color = new Color(1f, 1f, 1f, (1 + transform.position.y) / 1);
				}
			}
        }
    }
}
