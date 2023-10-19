using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Reset : MonoBehaviour
{
	[SerializeField]private Button resetButton;
	private const string NAME_SCENE_GAMEPLAY = "New";
	private void Awake()
	{
		if (!resetButton) resetButton = gameObject.GetComponentInChildren<Button>();
	}

	private void OnEnable()
	{
		resetButton.onClick.AddListener(OnClick_Reset);
	}

	private void OnClick_Reset()
	{
		SceneManager.LoadScene(NAME_SCENE_GAMEPLAY);
	}

	private void OnDisable()
	{
		resetButton.onClick.RemoveListener(OnClick_Reset);
	}
}
