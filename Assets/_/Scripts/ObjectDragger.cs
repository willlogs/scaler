using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB.Utils
{
    public class ObjectDragger : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        [SerializeField] private float _rotationSpeed = 2f;

        private bool _mouseDown = false;

        private FloatingObject _dragee;
        private bool _hasDragee = false;
        private Vector3 _planePoint = Vector3.zero;
        private Vector3 _planeNormal = -Vector3.forward;

        private void Update()
        {
            SetMouseState();
            CastRay();            
        }

        private void FixedUpdate()
        {
            if (_hasDragee)
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                float d = Vector3.Dot((_planePoint - transform.position), _planeNormal);
                float lower = Vector3.Dot(r.direction.normalized, _planeNormal);
                d = d / lower;
                _dragee.transform.position = transform.position + d * r.direction.normalized;
                _dragee.transform.rotation = Quaternion.Slerp(_dragee.transform.rotation, _dragee.starterRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }

        private void CastRay()
        {
            if (_mouseDown && !_hasDragee)
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(r, out hit, 30, _layer, QueryTriggerInteraction.Ignore);
                if (hit.collider != null)
                {
                    _dragee = hit.collider.GetComponent<FloatingObject>();

                    if (_dragee != null)
                    {
                        _hasDragee = true;
                        _dragee.StartDrag();
                        _planePoint = _dragee.transform.position;
                        _dragee.rb.isKinematic = true;
                    }
                }
                else
                {
                    _hasDragee = false;
                }
            }
        }
        
        private void OnStopDrag()
        {
            if (_hasDragee)
            {
                _dragee.rb.isKinematic = false;
                _dragee.StopDrag();
                _hasDragee = false;
            }
        }

        private void SetMouseState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseDown = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _mouseDown = false;
                OnStopDrag();
            }
        }
    }
}