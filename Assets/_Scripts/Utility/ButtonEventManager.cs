using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEventManager : MonoBehaviour {
    public void OnExit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnStart() => OnLevel01_01();
    public void OnMainMenu() => GameManager.Instance.LoadScene(EnumScene.MainMenu);

    public void OnLevel01_01() => GameManager.Instance.LoadScene(EnumScene.Level_01_01);
    public void OnLevel02() => GameManager.Instance.LoadScene(EnumScene.Level_02);
    public void OnLevel03() => GameManager.Instance.LoadScene(EnumScene.Level_03);
    public void OnLevel04() => GameManager.Instance.LoadScene(EnumScene.Level_04);
    public void OnLevel05() => GameManager.Instance.LoadScene(EnumScene.Level_05);
    public void OnLevel06() => GameManager.Instance.LoadScene(EnumScene.Level_01_06);
    public void OnLevel07() => GameManager.Instance.LoadScene(EnumScene.Level_07);
    public void OnLevel08() => GameManager.Instance.LoadScene(EnumScene.Level_08);
}
