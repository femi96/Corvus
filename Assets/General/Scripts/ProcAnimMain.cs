using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcAnimMain : ProcAnim {

  [Header("Main Variables")]
  public Transform anchorTransform;
  public Vector3 anchorOffset;
  public float pull = 4f;
  public float drag = 4f;

  public override void UpdateForce() {
    Vector3 anchorPoint = anchorTransform.position + anchorOffset;
    // Force is temporary spring damp system. Will develop better later
    force = Vector3.zero;
    force += pull * (anchorPoint - transform.position);
    force -= drag * vel;
  }

  public override void UpdateFaceTarget() {
    faceOnPlaneTarget = vel - vel.y * Vector3.up;
  }

  public override void PieceStart() {
    anchorOffset = transform.position - anchorTransform.position;
  }
}
