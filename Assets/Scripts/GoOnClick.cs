using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoOnClick : MonoBehaviour {

  public Map map;
  public Location location;

  void OnMouseOver() {
    if (Input.GetMouseButtonDown(0) && location != null)
      map.GoToLocation(location);
  }
}
