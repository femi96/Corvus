using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party {

  /* A group of monsters
    members - All monsters that are party members
    board - All monsters that are out on the board
    */
  public Monster[] members;
  public Monster[] board;

  public Party(Monster[] mons) {
    members = new Monster[6];
    board = new Monster[3];

    for (int i = 0; i < mons.Length; i++) {
      members[i] = mons[i];

      if (i < 3)
        board[i] = mons[i];
    }
  }

  public Monster GetFront() {
    return board[0];
  }

  public Monster GetMiddle() {
    return board[1];
  }

  public Monster GetBack() {
    return board[2];
  }
}