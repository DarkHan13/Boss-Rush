using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRushManager : MonoBehaviour
{

    [SerializeField] private BR_Boss boss;
    [SerializeField] private BR_PlayerController player;

    private void Start()
    {
        boss.onDead += RestartLevel;
        player.onDead += RestartLevel;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void RestartLevel()
    {
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(3f);
        var index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

}
