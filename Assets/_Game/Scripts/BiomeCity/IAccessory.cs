using UnityEngine;

namespace BiomeCity
{
    /// <summary>
    /// Interface for building accessories.
    /// </summary>
    public interface IAccessory
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        bool FollowBuildingTransform { get; set; }
        Color GizmoColor { get; set; }

        Vector3 InitialLocalPosition { get; set; }
        Quaternion InitialLocalRotation { get; set; }
        Vector3 InitialBuildingPosition { get; set; }
        Quaternion InitialBuildingRotation { get; set; }
        Vector3 LastKnownLocalPosition { get; set; }
        Quaternion LastKnownLocalRotation { get; set; }
    }
}