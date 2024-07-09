using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour {
    [SerializeField] private Vector3 moveSpeed = new Vector3(0, 75, 0);
    [SerializeField] private float timeToFade = 1f;

    private RectTransform textTransform;
    private TextMeshProUGUI textMeshPro;

    private float timeEllapsed = 0f;
    private Color startColor;

    private void Awake() {
        textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    private void Update() {
        textTransform.position += moveSpeed * Time.deltaTime;

        timeEllapsed += Time.deltaTime;
        if (timeEllapsed < timeToFade) {
            float fadeAlfa = startColor.a * (1 - (timeEllapsed / timeToFade));
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlfa);
        } else {
            Destroy(gameObject);
        }
    }
}
