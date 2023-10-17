using UnityEngine;

namespace ReferenceValue
{
    public class RefValue : ScriptableObject
    {
        public delegate void ValueChanged();

        public event ValueChanged OnValueChanged;

        public void ValueHasChanged()
        {
            OnValueChanged?.Invoke();
        }
    }
}