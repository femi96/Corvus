using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcAnimHead : ProcAnim {

  [Header("Head Variables")]
  public Transform lookTransform;
  public ProcAnim anchorPiece;
  private Transform anchorTransform;
  public Vector3 anchorOffset;
  public float pull = 4f;
  public float drag = 4f;
  private Vector3 lastAnchorPoint;

  public override void UpdateForce() {
    Vector3 anchorPoint = anchorTransform.position + anchorTransform.rotation * anchorOffset;
    pos += anchorPoint - lastAnchorPoint;
    lastAnchorPoint = anchorPoint;
    force = Vector3.zero;
    force += pull * (anchorPoint - pos);
    force -= drag * vel;
  }

  public override void UpdateFaceTarget() {
    faceOnPlaneTarget = anchorPiece.faceOnPlane;

    if (lookTransform) {
      Vector3 lookDirection = lookTransform.position - transform.position;
      lookDirection = lookDirection - lookDirection.y * Vector3.up;

      if ((lookDirection.normalized - faceOnPlaneTarget.normalized).magnitude < 1.4f)
        faceOnPlaneTarget = lookDirection;
    }
  }

  public override bool UseFaceTarget() { return true; }

  public override void PieceStart() {
    anchorTransform = anchorPiece.transform;
    anchorOffset = transform.position - anchorTransform.position;
    lastAnchorPoint = anchorTransform.position + anchorTransform.rotation * anchorOffset;
  }
}
