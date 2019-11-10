using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcAnimSecondary : ProcAnim {

  [Header("Foot Variables")]
  public ProcAnim anchorPiece;
  private Transform anchorTransform;
  public Vector3 anchorOffset;
  public float pull = 4f;
  public float drag = 4f;
  public bool debugAnchor = false;
  private GameObject debugAnchorObj;

  public override void UpdateForce() {
    Vector3 anchorPoint = anchorTransform.position + anchorTransform.rotation * anchorOffset;

    if (debugAnchor) {
      if (debugAnchorObj == null)
        debugAnchorObj = Instantiate(MonsterPrefabs.instance.debugAnchorPrefab, transform.parent);

      debugAnchorObj.transform.position = anchorPoint;
    }

    force = Vector3.zero;
    force += pull * (anchorPoint - transform.position);
    force -= drag * vel;
  }

  public override void UpdateFaceTarget() {
    faceOnPlaneTarget = anchorPiece.faceOnPlane;
  }

  public override bool UseAccTilt() { return false; }

  public override void PieceStart() {
    anchorTransform = anchorPiece.transform;
    anchorOffset = transform.position - anchorTransform.position;
  }
}
