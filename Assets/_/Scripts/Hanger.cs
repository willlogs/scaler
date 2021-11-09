using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DB.Utils;

namespace DB.Scale
{
    public class Hanger : MonoBehaviour
    {
        public Rigidbody rb;

        public void Hang(Collider other)
        {
            if (_floatingObject.isBeingDragged)
            {
                HangerPos hp = other.GetComponent<HangerPos>();
                if (hp != null && !hp.occupied)
                {
                    hp.occupied = true;
                    _hangerPos = hp;
                    _readyToHang = true;
                }
            }
        }

        public void StopHang(Collider other)
        {
            if (_floatingObject.isBeingDragged)
            {
                if (other == _hangingCollider)
                {
                    _hangerPos.occupied = false;
                    _isHanging = false;
                    // fall off
                }
            }
        }

        [SerializeField] private FloatingObject _floatingObject;
        private Collider _hangingCollider;
        private HangerPos _hangerPos;
        private bool _readyToHang = false, _isHanging = false;

        private void Start()
        {
            _floatingObject.OnStopDrag += OnStopDrag;
        }

        private void OnStopDrag()
        {
            if (_readyToHang)
            {
                Vector3 offset = rb.transform.position - transform.position;
                rb.transform.position = _hangerPos.transform.position + offset;
                ConfigurableJoint cj = rb.gameObject.AddComponent<ConfigurableJoint>();

                cj.xMotion = ConfigurableJointMotion.Locked;
                cj.yMotion = ConfigurableJointMotion.Locked;
                cj.zMotion = ConfigurableJointMotion.Locked;

                cj.anchor = transform.localPosition;
                cj.connectedBody = _hangerPos.rb;
            }
        }
    }
}