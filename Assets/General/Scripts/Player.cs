using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

  public int curHealth;
  public int maxHealth = 40;

  private GameObject uiHover;
  private GameObject uiHealth;
  private Vector3 uiHoverOffset = new Vector3(0, 1.25f, 0);

  void Start() {
    curHealth = maxHealth;

    uiHover = Instantiate(UIPrefabs.instance.playerHoverPrefab, UIPrefabs.instance.canvasTransform);
    uiHealth = uiHover.transform.Find("Health").gameObject;
  }

  void Update() {

  }

  void LateUpdate() {
    UpdateUI();
  }

  private void UpdateUI() {
    float healthPercent = 100f * curHealth / maxHealth;
    Vector3 pos = transform.position + uiHoverOffset;
    uiHealth.GetComponent<RectTransform>().sizeDelta = new Vector2(healthPercent, 20);
    uiHover.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
  }

  public void DealDamage(int damage) {
    curHealth = Mathf.Max(curHealth - damage, 0);
  }
}
