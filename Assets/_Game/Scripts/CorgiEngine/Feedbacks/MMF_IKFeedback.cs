
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.U2D.IK;


namespace Ameba.MMFeedback {

  /// <summary>
  /// This feedback allows you to animate a transform's position and rotation over time.
  /// </summary>
  [AddComponentMenu("Ameba/Corgi Extensions/Feedbacks/2D IK Feedback")]
  [FeedbackPath("Ameba/Animation/2D IK Feedback")]
  [FeedbackHelp("This feedback allows you to animate an IK transform's position and rotation over time.")]
  public class MMF_IKFeedback : MMF_Feedback {
    [MMFInspectorGroup("IK Feedback", true, 17)]
    [Tooltip("The IK Solver2D for the feedback")]
    public Solver2D Solver;

    [Tooltip("The position to animate to")]
    public Vector3 Position;
    [Tooltip("The rotation to animate to")]
    public Quaternion Rotation;
    [Tooltip("The speed of the animation")]
    public float Speed = 1f;
    [Tooltip("Should the feedback restore the initial position and rotation when stopped")]
    public bool RestoreInitialPosition = true;

    protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f) {
      if (Active && Solver != null) {
        // Set the target position and rotation
        if (Position != null) {
          Solver.GetChain(0).target.transform.position =
              Vector3.Lerp(Solver.GetChain(0).target.transform.position,
                  Position, Speed * Time.deltaTime);
        }
        if (Rotation != null) {
          Solver.GetChain(0).target.transform.rotation =
          Quaternion.Slerp(Solver.GetChain(0).target.transform.rotation,
              Rotation, Speed * Time.deltaTime);
        }
      }
    }

    protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1) {
      base.CustomStopFeedback(position, feedbacksIntensity);
      if (RestoreInitialPosition && Active && Solver != null) {
        // Restore the initial position and rotation
        if (Position != null) {
          Vector3 inversePosition = -(Position);
          Solver.GetChain(0).target.transform.position =
              Vector3.Lerp(Solver.GetChain(0).target.transform.position,
                  inversePosition, Speed * Time.deltaTime);
        }
        if (Rotation != null) {
          Quaternion inverseRotation = Quaternion.Inverse(Rotation);
          Solver.GetChain(0).target.transform.rotation =
          Quaternion.Slerp(Solver.GetChain(0).target.transform.rotation,
              inverseRotation, Speed * Time.deltaTime);
        }
      }
    }


  }
}