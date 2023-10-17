using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ReferenceValue
{
    public class ValueObserver : MonoBehaviour
    {
        [SerializeField] private RefValue Reference;
        [SerializeField] private bool WaitUntilStart = true;

        [SerializeField] private UnityEvent OnReferenceChanged;

        private bool _isActive;

        private void Awake()
        {
            if(WaitUntilStart)
                return;

            _isActive = true;
        }

        private void OnEnable()
        {
            Reference.OnValueChanged += ResponseToValueChanged;
        }

        private void OnDisable()
        {
            Reference.OnValueChanged -= ResponseToValueChanged;
        }

        private void Start()
        {
            if(WaitUntilStart)
                return;

            StartCoroutine(WaitForNextFrameToActivate());
        }

        private IEnumerator WaitForNextFrameToActivate()
        {
            yield return new WaitForEndOfFrame();
            _isActive = true;
        }

        private void ResponseToValueChanged()
        {
            if(!_isActive)
                return;
            
            OnReferenceChanged?.Invoke();
        }
    }
}