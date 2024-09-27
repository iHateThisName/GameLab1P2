using UnityEngine;

public class ExitController : MonoBehaviour {
    [SerializeField] private EnumScene scene = EnumScene.MainMenu;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Goal");
            GameManager.Instance.LoadScene(scene);
        }
    }
}

public enum EnumScene {
    MainMenu = 0,
    Level_01_01 = 1,
    Level_01_02 = 2
}
