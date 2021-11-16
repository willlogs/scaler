using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DB.Utils;
using UnityEngine.Events;

namespace DB.Scale
{
    public class Hanger : MonoBehaviour
    {
        public Rigidbody rb;
        public Transform hangerOffsetT;
        public bool _isHanger = false;
	
        public UnityEvent OnAttach, OnDetach, OnFullyHanged, OnFullyDetach;

        public void RegisterFO(FloatingObject fo){
            rb = fo.rb;
            _floatingObject = fo;
        }

        public void Hang(Collider other)
        {
            if (_floatingObject.isBeingDragged)
            {
                HangerPos hp = other.GetComponent<HangerPos>();
                if (hp != null && !hp.occupied)
                {
                    _hangingCollider = other;
                    hp.occupied = true;
                    _hangerPos = hp;
                    _readyToHang = true;
		            OnAttach?.Invoke();
                }
            }
        }

        public void TryStopHang(){
            if(_readyToHang){
                _readyToHang = false;
                _hangerPos.occupied = false;
                Destroy(cj);
                OnDetach?.Invoke();
                if (_isHanging)
                {
                    _isHanging = false;
                    OnFullyDetach?.Invoke();
                }
            }            
        }

        public void StopHang(Collider other)
        {
            if (_floatingObject.isBeingDragged)
            {
                if (other == _hangingCollider)
                {
                    if (_isHanging)
                    {
                        _isHanging = false;
                        OnFullyDetach?.Invoke();
                    }
                    _hangerPos.occupied = false;
                    Destroy(cj);
		            OnDetach?.Invoke();
                    StartCoroutine(RestAfterDettach());
                }
            }
        }

        [SerializeField] private FloatingObject _floatingObject;
        [SerializeField] private float _betweenFallAndAttach = 1f;
        private Collider _hangingCollider;
        private HangerPos _hangerPos;
        private bool _readyToHang = false, _isHanging = false, _canHang = true;
        private ConfigurableJoint cj, _reference;

        private void Start()
        {
            _floatingObject.OnStopDrag += OnStopDrag;
            _reference = FindObjectOfType<JointReference>().GetComponent<ConfigurableJoint>();
        }

        private IEnumerator RestAfterDettach()
        {
            _canHang = false;
            yield return new WaitForSeconds(_betweenFallAndAttach);
            _canHang = true;
        }

        private void OnStopDrag()
        {
            if (_readyToHang && !_isHanging && _canHang)
            {
                _isHanging = true;
                Vector3 offset = rb.transform.position - hangerOffsetT.position;
                rb.transform.position = _hangerPos.transform.position + offset;
                cj = rb.gameObject.AddComponent<ConfigurableJoint>();

                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;

                cj.anchor = rb.transform.InverseTransformPoint(hangerOffsetT.position);
                cj.connectedBody = _hangerPos.rb;

                // TODO: set angular limits from the main scale
                if(_isHanger){
                    cj.angularXMotion = ConfigurableJointMotion.Limited;
                    cj.angularYMotion = ConfigurableJointMotion.Limited;
                    cj.angularZMotion = ConfigurableJointMotion.Limited;
                }

                cj.angularYLimit = _reference.angularYLimit;
                cj.angularZLimit = _reference.angularZLimit;
                cj.highAngularXLimit = _reference.highAngularXLimit;
                cj.lowAngularXLimit = _reference.lowAngularXLimit;

                cj.linearLimitSpring = _reference.linearLimitSpring;
                cj.angularXLimitSpring = _reference.angularXLimitSpring;
                cj.angularYZLimitSpring = _reference.angularYZLimitSpring;

                if(!_isHanger)
                    cj.enableCollision = _reference.enableCollision;

                OnFullyHanged?.Invoke();
            }
        }
    }
}
