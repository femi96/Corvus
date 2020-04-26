using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcAnimator : MonoBehaviour {

  // Manager of all proc anim
  public Transform lookTarget;
  public Transform moveTarget;

  public ProcAnimMain main;
  public ProcAnimHead head;

  void Awake() {
    main.anchorTransform = moveTarget;
    head.lookTransform = lookTarget;
  }
}
