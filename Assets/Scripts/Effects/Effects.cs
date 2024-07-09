using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void HideEffect() {
        anim.gameObject.SetActive(false);
    }

    public void ShowEffect() {
        anim.gameObject.SetActive(true);
    }
}
