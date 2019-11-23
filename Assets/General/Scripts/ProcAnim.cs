using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Procedural Animation System

Physics based animations using unique pieces
To maintain smoothness of movements, try to maintain 2nd order continuity, i.e. keep
  velocity and position continuous. Do not directly modify them, only modify them via
  acceleration/forces. Visual representation not directly based on the factors should
  similarly use 2nd order correct.

ToDos:
 - Add bounce arc where height is inverse of speed, period is distance traveled
 - Head target looking
 - Make faceOnPlane use acceleration with (just under?) critically dampened spring system
 - Add wind field that applies to all forces

Pieces:
 - Main - Force towards anchor point, used to move entire system
 - Secondary - Force towards anchor point relative to an anchor piece for secondary motion
 - Foot - Jump to position on ground. Doesn't slide on ground
 - Head - Looks at target, with secondary force

Math notes:
  critical drag = 2 * sqrt(pull * mass)
*/

public abstract class ProcAnim : MonoBehaviour {

  [Header("Base Variables - Physics")]
  public float mass = 1;
  public Vector3 force = Vector3.zero;
  public Vector3 acc = Vector3.zero;
  public Vector3 vel = Vector3.zero;
  public Vector3 pos = Vector3.zero;

  [Header("Base Variables - Face Direction On Plane")]
  public Vector3 faceOnPlane = Vector3.forward;
  public Vector3 faceOnPlaneTarget = Vector3.forward;
  public float turnSpeed = 180f; // Degrees per second

  [Header("Base Variables - Acceleration Tilt")]
  public float accTilt = 4f;
  public float maxTilt = 45f; // Degrees

  void Start() {
    pos = transform.position;

    if (mass == 0)
      mass = 1;

    PieceStart();
  }

  void Update() {
    // Apply force to physics
    UpdateForce();
    acc = force / mass;
    vel += acc * Time.deltaTime;
    pos += vel * Time.deltaTime;

    // Update position
    transform.position = pos;

    // Update rotations
    transform.rotation = Quaternion.identity;

    // Update face direction on plane
    if (UseFaceTarget()) {
      UpdateFaceTarget();

      if (faceOnPlaneTarget.magnitude > 0) {
        faceOnPlaneTarget = faceOnPlaneTarget.normalized;
        Vector3 rotP = Quaternion.Euler(0, turnSpeed * Time.deltaTime, 0) * faceOnPlane;
        Vector3 rotN = Quaternion.Euler(0, -turnSpeed * Time.deltaTime, 0) * faceOnPlane;
        float disP = (rotP - faceOnPlaneTarget).magnitude;
        float disN = (rotN - faceOnPlaneTarget).magnitude;
        float disT = (faceOnPlane - faceOnPlaneTarget).magnitude;

        if (disT < disP && disT < disN)
          faceOnPlane = faceOnPlaneTarget;

        if (disP < disT && disP < disN)
          faceOnPlane = rotP;

        if (disN < disP && disN < disT)
          faceOnPlane = rotN;
      }

      if (faceOnPlane.magnitude > 0)
        transform.rotation *= Quaternion.LookRotation(faceOnPlane);
    }

    // Update acc tilt
    if (UseAccTilt()) {
      Vector3 accOnPlane = acc - acc.y * Vector3.up;

      // Break into vectors relative to piece face direction
      Vector3 accOnPlaneF = Vector3.Project(accOnPlane, faceOnPlane);
      Vector3 accOnPlaneR = accOnPlane - accOnPlaneF;

      if (accOnPlaneF.magnitude > 0) {
        Vector3 right = Vector3.right * Mathf.Sign(Vector3.Dot(accOnPlaneF, faceOnPlane));
        float tiltAngleF = Mathf.Min(Mathf.Max(accOnPlaneF.magnitude * accTilt, -maxTilt), maxTilt);
        transform.rotation *= Quaternion.AngleAxis(tiltAngleF, right);
      }

      if (accOnPlaneR.magnitude > 0) {
        Vector3 welOnPlane = Vector3.Cross(Vector3.up, faceOnPlane);
        Vector3 back = Vector3.back * Mathf.Sign(Vector3.Dot(accOnPlaneR, welOnPlane));
        float tiltAngleR = Mathf.Min(Mathf.Max(accOnPlaneR.magnitude * accTilt, -maxTilt), maxTilt);
        transform.rotation *= Quaternion.AngleAxis(tiltAngleR, back);
      }
    }
  }

  // Apply force to piece
  public abstract void UpdateForce();

  // If use a face rotation target to rotate piece on plane
  public virtual bool UseFaceTarget() { return false; }

  // Update faceOnPlaneTarget
  public virtual void UpdateFaceTarget() {}

  // If use acceleration tilt
  public virtual bool UseAccTilt() { return false; }

  // Startup behavior
  public virtual void PieceStart() {}
}
