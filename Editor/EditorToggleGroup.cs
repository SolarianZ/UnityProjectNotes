using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace GBG.ProjectNotes.Editor
{
    public class EditorToggleGroup : GroupBox
    {
        private readonly List<Toggle> _toggles = new List<Toggle>();
        private Toggle _activeToggle;

        public event Action<Toggle> activeToggleChanged;


        public new void Add(VisualElement child)
        {
            if (child is Toggle toggle && !_toggles.Contains(toggle))
            {
                _toggles.Add(toggle);
                toggle.RegisterValueChangedCallback(OnToggleValueChanged);
            }

            base.Add(child);
        }

        public new void Remove(VisualElement child)
        {
            if (child is Toggle toggle)
            {
                _toggles.Remove(toggle);
                toggle.UnregisterValueChangedCallback(OnToggleValueChanged);
            }

            base.Remove(child);
        }


        private void OnToggleValueChanged(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
            {
                if (evt.target == _activeToggle)
                {
                    _activeToggle.SetValueWithoutNotify(true);
                }

                return;
            }

            _activeToggle = evt.target as Toggle;
            foreach (Toggle toggle in _toggles)
            {
                if (toggle != _activeToggle)
                {
                    toggle.SetValueWithoutNotify(false);
                }
            }

            activeToggleChanged?.Invoke(_activeToggle);
        }
    }
}