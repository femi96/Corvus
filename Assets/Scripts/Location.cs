using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Location {

  public abstract string GetName();

  public abstract void Visit(Player player);

  public abstract void Clear(int result);
}
