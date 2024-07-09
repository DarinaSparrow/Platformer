using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("����� ���������")]
    [SerializeField] private GameObject HUD; // ���
    [SerializeField] private int numOfLevels; // ���

    [Header("����� ��������� ����")]
    [SerializeField] private GameObject gameOverScreen; // ����� ��������� ����
    [SerializeField] private AudioClip gameOverSound; // ���� ��������� ����

    [Header("�����")]
    [SerializeField] private GameObject pauseScreen; // ����� �����

    [Header("�������� ���������� ������")]
    [SerializeField] private GameObject nextLevelScreen; // ����� �����

    [Header("��� ������ ��������")]
    [SerializeField] private GameObject endLevelsScreen; // ����� �����

    [Header("��������� ����")]
    [SerializeField] private Button playButton;

    [Header("����� ��������")]
    [SerializeField] private GameObject loadingScreen;

    [Header("������ �������")]
    [SerializeField] private Button[] levelButtons;

    public static UIManager instance { get; private set; }

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != null && instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        if (gameOverScreen) gameOverScreen.SetActive(false);
        if (pauseScreen) pauseScreen.SetActive(false);

        int level = PlayerPrefs.GetInt("lastLevel", -1);
        if (level == -1 && playButton) playButton.interactable = false;
    }

    public void Play() {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("isNewGame", 0); // ���������� ���� ����������� ����
        int loadLevel = PlayerPrefs.GetInt("lastLevel", 1);
        StartLoading(loadLevel);
    }

    public void NextLevel() {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("isNewGame", 1); // ���������� ���� ����� ����
        PlayerPrefs.SetInt("lastLevel", SceneManager.GetActiveScene().buildIndex + 1);
        int loadLevel = SceneManager.GetActiveScene().buildIndex + 1;
        StartLoading(loadLevel);
    }

    public void LoadLevel(int loadLevel) {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("isNewGame", 1); // ���������� ���� ����� ����
        PlayerPrefs.SetInt("lastLevel", loadLevel);
        StartLoading(loadLevel);
    }

    public void LoadScene(int loadScene) {
        Time.timeScale = 1;
        SceneManager.LoadScene(loadScene);
    }

    public void Quit() {
        Application.Quit(); // ����� �� ���� (�������� ������ � ������)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ����� �� ������ ���� (�������� ������ � ���������)
#endif
    }

    private void StartLoading(int loadLevel) {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsync(loadLevel));
    }

    private IEnumerator LoadAsync(int loadLevel) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadLevel);
        while (!asyncLoad.isDone) {
            yield return null;
        }
        loadingScreen.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseScreen) {
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    public void GameOver() {
        PlayerPrefs.DeleteKey("lastLevel");
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    public void Restart() {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("isNewGame", 1); // ���������� ���� ����� ����
        PlayerPrefs.SetInt("lastLevel", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }

    public void PauseGame(bool status) {
        pauseScreen.SetActive(status);
        HUD.SetActive(!status);

        Time.timeScale = status ? 0 : 1;
    }

    public void LevelComplete(bool status) {
        if (SceneManager.GetActiveScene().buildIndex < numOfLevels)
            nextLevelScreen.SetActive(status);
        else 
            endLevelsScreen.SetActive(status);

        HUD.SetActive(!status);
    }

    public void SoundVolume() {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume() {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
}