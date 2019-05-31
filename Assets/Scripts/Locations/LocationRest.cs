using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationRest : Location {

  private int id;

  public LocationRest() {
    id = Random.Range(0, 1000);
  }

  public override string GetName() {
    return "Rest " + id;
  }

  public override void Visit(Map map) {
    map.StartRest();
  }

  public override void Clear(int result) {}
}