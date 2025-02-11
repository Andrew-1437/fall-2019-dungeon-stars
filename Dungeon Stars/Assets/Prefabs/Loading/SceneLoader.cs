﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Animator))]
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    Animator anim;

    bool loading;

    public TextMeshProUGUI tip;

    public string[] loadScreenTips;

    private void Awake()
    {
        if (Instance != null && Instance == this)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;
        anim = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string scene_name)
    {
        if (loading)
            return;
        StartCoroutine(LoadSceneAsync(scene_name));

        tip.text = loadScreenTips[Random.Range(0, loadScreenTips.Length)];
    }

    public IEnumerator LoadSceneAsync(string scene_name)
    {
        anim.SetBool("StartLoad", true);
        loading = true;

        yield return new WaitForSeconds(3f);

        AsyncOperation async = SceneManager.LoadSceneAsync(scene_name);

        while (!async.isDone)
        {
            yield return null;
        }
        anim.SetBool("StartLoad", false);
        loading = false;
    }
}
