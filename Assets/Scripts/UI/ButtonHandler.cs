using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour {

    private Button button;

    void Awake() {
        button = GetComponent<Button>();
    }

    void Update() {
        button.interactable = false;
        button.interactable = true;
    }
}