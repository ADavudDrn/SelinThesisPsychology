using UnityEngine;

namespace ReferenceValue
{
    [CreateAssetMenu(fileName = "Vector3 Value", menuName = "Values/Vector3")]
    public class Vector3Ref : RefValue
    {
        public Vector3 Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueHasChanged();
            }
        }

        private Vector3 _value;
    }
}