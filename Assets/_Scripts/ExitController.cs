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
    Level_01_02 = 2,
    Level_01_03 = 3,
    Level_01_04 = 4,
    Level_01_05 = 5,
    Level_01_06 = 6,
    Level_02 = 7,
    Level_03 = 8,
    Level_04 = 9,
    Level_05 = 10,
    Level_06 = 11,
    Level_07 = 12,
    Level_08 = 13,
    Level_09 = 14,
    Level_010 = 15,
    Level_01_wall = 16,
    Level_01_08 = 17,

}
