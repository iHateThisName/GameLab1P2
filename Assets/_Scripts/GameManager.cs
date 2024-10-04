using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject _playerPrefab;

    private Vector2 _levelStartPosition;
    public Vector2? LastCheckPointPosition;
    public bool IsPlayerDead { get; private set; } = false;

    private Camera _camera;
    private bool _cameraFollow = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadScene(EnumScene scene) {
        SceneManager.LoadScene((int)scene);
    }

    public void SetLevelStartPosition(Vector2 position, bool useCameraFollow) {
        _cameraFollow = useCameraFollow;
        _levelStartPosition = position;
        LastCheckPointPosition = null;
        Respawn();
    }

    public void Respawn() {
        GameObject go;

        if (!LastCheckPointPosition.HasValue) {
            go = Instantiate(_playerPrefab, _levelStartPosition, Quaternion.identity);
        } else {
            go = Instantiate(_playerPrefab, LastCheckPointPosition.Value, Quaternion.identity);
        }
        if (_cameraFollow) {
            if (_camera == null) GetCameraInScene();
            _camera.transform.SetParent(go.transform);
            _camera.transform.localPosition = new Vector3(0, 2, -10);  // Adjust this as needed to position the camera correctly
            _camera.orthographicSize = 5;
        }

        IsPlayerDead = false;
    }

    public void Death(GameObject player) {
        if (!IsPlayerDead) {
            IsPlayerDead = true;
            _camera.gameObject.transform.SetParent(null);

            Destroy(player);
            Destroy(GetTimerCanvasInScene());
            Respawn();
        }
    }

    private void GetCameraInScene() => _camera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
    private GameObject GetTimerCanvasInScene() => GameObject.FindGameObjectWithTag("TimerText")?.transform.parent.gameObject;
}
