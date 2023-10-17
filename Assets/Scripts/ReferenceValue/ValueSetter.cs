using System;
using UnityEngine;

namespace ReferenceValue
{
    public class ValueSetter : MonoBehaviour
    {
        private enum SetTime
        {
            Awake,
            OnEnable,
            Start,
        }

        [SerializeField] private RefValue Reference;
        [SerializeField] private SetTime SetOn;

        [SerializeField] private Vector3 Vector3Value;


        private void Awake()
        {
            if(SetOn != SetTime.Awake)
                return;
            SetValue();
        }

        private void OnEnable()
        {
            if(SetOn != SetTime.OnEnable)
                return;
            SetValue();
        }

        private void Start()
        {
            if(SetOn != SetTime.Start)
                return;
            SetValue();
        }


        private void SetValue()
        {
            switch (Reference)
            {
                case Vector3Ref vector3Ref:
                    vector3Ref.Value = Vector3Value;
                    break;
            }
        }
    }
}