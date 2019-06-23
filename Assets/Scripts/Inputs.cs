using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inputs {

  public static bool NextMessage() {
    return Input.GetKeyDown(KeyCode.Space) ||
           Input.GetKey(KeyCode.Return);
  }
}
