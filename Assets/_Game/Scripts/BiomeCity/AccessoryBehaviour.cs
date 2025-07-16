using UnityEngine;

namespace BiomeCity
{
    /// <summary>
    /// Default MonoBehaviour implementation of IAccessory.
    /// </summary>
    public class AccessoryBehaviour : MonoBehaviour, IAccessory
    {
        [SerializeField] private bool _followBuildingTransform = true;
        [HideInInspector] [SerializeField] private Color _gizmoColor = default;

        [HideInInspector] [SerializeField] private Vector3 _initialLocalPosition;
        [HideInInspector] [SerializeField] private Quaternion _initialLocalRotation;
        [HideInInspector] [SerializeField] private Vector3 _initialBuildingPosition;
        [HideInInspector] [SerializeField] private Quaternion _initialBuildingRotation;
        [HideInInspector] [SerializeField] private Vector3 _lastKnownLocalPosition;
        [HideInInspector] [SerializeField] private Quaternion _lastKnownLocalRotation;

        public bool FollowBuildingTransform
        {
            get => _followBuildingTransform;
            set => _followBuildingTransform = value;
        }

        public Color GizmoColor
        {
            get => _gizmoColor;
            set => _gizmoColor = value;
        }

        public Vector3 InitialLocalPosition
        {
            get => _initialLocalPosition;
            set => _initialLocalPosition = value;
        }

        public Quaternion InitialLocalRotation
        {
            get => _initialLocalRotation;
            set => _initialLocalRotation = value;
        }

        public Vector3 InitialBuildingPosition
        {
            get => _initialBuildingPosition;
            set => _initialBuildingPosition = value;
        }

        public Quaternion InitialBuildingRotation
        {
            get => _initialBuildingRotation;
            set => _initialBuildingRotation = value;
        }

        public Vector3 LastKnownLocalPosition
        {
            get => _lastKnownLocalPosition;
            set => _lastKnownLocalPosition = value;
        }

        public Quaternion LastKnownLocalRotation
        {
            get => _lastKnownLocalRotation;
            set => _lastKnownLocalRotation = value;
        }
    }
}