using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcAnimFoot : ProcAnim {

  [Header("Foot Variables")]
  public ProcAnim anchorPiece;
  private Transform anchorTransform;
  public Vector3 anchorOffset;
  public float pull = 4f;
  public float drag = 4f;
  public float strideDist = 1f;
  public float speedForMaxStride = 1f;
  public bool debugAnchor = false;
  private GameObject debugAnchorObj;
  private GameObject debugFootObj;
  private Vector3 footPoint;
  private Vector3 footDir;

  public override void UpdateForce() {
    Vector3 anchorPoint = anchorTransform.position + anchorTransform.rotation * anchorOffset;
    anchorPoint -= anchorPoint.y * Vector3.up;

    float strideRadius = strideDist * Mathf.Clamp(anchorPiece.vel.magnitude / speedForMaxStride, 0.5f, 1f);
    Vector3 footPointDir = anchorPiece.vel.normalized;

    if (footPointDir.magnitude > 0)
      footPointDir += Random.onUnitSphere * 0.25f;

    footPointDir -= footPointDir.y * Vector3.up;

    Vector3 footPointIdeal = anchorPoint + footPointDir.normalized * strideRadius;

    if ((anchorPoint - footPoint).magnitude > strideRadius) {
      footPoint = footPointIdeal;
      Vector3 footDirNoise = Random.onUnitSphere * 0.2f + anchorOffset.normalized * 0.05f;
      footDirNoise -= footDirNoise.y * Vector3.up;
      footDir = anchorPiece.faceOnPlane + footDirNoise;
    }

    if (debugAnchor) {
      if (debugAnchorObj == null)
        debugAnchorObj = Instantiate(MonsterPrefabs.instance.debugAnchorPrefab, transform.parent);

      if (debugFootObj == null)
        debugFootObj = Instantiate(MonsterPrefabs.instance.debugAnchorPrefab, transform.parent);

      debugAnchorObj.transform.position = footPointIdeal;
      debugFootObj.transform.position = footPoint;
    }

    /*
    force = Vector3.zero;
    force += pull * (footPoint - transform.position);
    force -= drag * vel;
    */
    // Insta step
    force = Vector3.zero;
    vel = Vector3.zero;
    pos = footPoint;
  }

  public override void UpdateFaceTarget() {
    faceOnPlaneTarget = footDir;
  }

  public override bool UseAccTilt() { return false; }

  public override void PieceStart() {
    anchorTransform = anchorPiece.transform;
    anchorOffset = transform.position - anchorTransform.position;
    footPoint = transform.position;
  }
}
