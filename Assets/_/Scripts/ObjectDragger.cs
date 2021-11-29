using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB.Utils
{
    public class ObjectDragger : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer, _hangerLayer;
        [SerializeField] private float _rotationSpeed = 2f, _lerpSpeed = 5f;

        [SerializeField] private Transform _top;

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

                Vector3 restPos = _top.position;
                Vector3 restPosVP = Camera.main.WorldToViewportPoint(restPos);
                Vector3 restPosDiff = _dragee.restPos - transform.position;

                Vector3 ofst = _dragee.hanger.hangerOffsetT.position - _dragee.transform.position;
                ofst.z = 0;
                Vector3 targetPos = transform.position + d * r.direction.normalized + ofst / 2;
                Vector3 targetPosVP = Camera.main.WorldToViewportPoint(targetPos);

                float yDiffTime = Mathf.Clamp01(Mathf.Clamp(targetPosVP.y - restPosVP.y, 0, 0.2f) / 0.2f);
                d = Mathf.Lerp(restPosDiff.magnitude, d, yDiffTime);

                targetPos = transform.position + d * r.direction.normalized + ofst / 2;

                _dragee.transform.position = Vector3.Lerp(
                    _dragee.transform.position,
                    targetPos,
                    Time.deltaTime * _lerpSpeed
                );

                float targetScale = 1;
                targetScale = Mathf.Lerp(0.1f, 1f, yDiffTime);
                _dragee.transform.localScale = Vector3.Lerp(
                    _dragee.transform.localScale,
                    Vector3.one * targetScale,
                    Time.deltaTime * _lerpSpeed
                );

                _dragee.transform.rotation = Quaternion.Slerp(
                    _dragee.transform.rotation,
                    _dragee.starterRotation,
                    _rotationSpeed * Time.fixedDeltaTime
                );

                Ray hangerRay = new Ray(
                        transform.position,
                        _dragee.hanger.hangerOffsetT.position - transform.position
                );
                RaycastHit hit;
                Physics.Raycast(hangerRay, out hit, 30, _hangerLayer);
                if(hit.collider != null)
                {
                    _dragee.hanger.Hang(hit.collider);
                }
                else{
                    _dragee.hanger.TryStopHang();
                }
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
                    _dragee = hit.collider.transform.GetComponent<FloatingObject>();

                    if(_dragee == null)
                        _dragee = hit.collider.transform.parent.GetComponent<FloatingObject>();

                    if (_dragee != null && _dragee.canBeDragged)
                    {
                        _hasDragee = true;
                        _dragee.StartDrag();
                        _planePoint = _dragee.transform.position;
                        _planePoint.z = 0;
                        _dragee.rb.isKinematic = true;
                        _dragee.hanger.TryStopHang();
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
