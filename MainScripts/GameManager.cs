using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    public Player player;
    public CameraMovement cameraMovement;
    public ItemDirectory itemDirectory;
    public ItemGenerator itemGenerator;
    public AstarPath pathfinder;
    public RoomFirstDungeonGenerator dungeonGenerator;
    public JSONSaving saveManager;
    public GameObject loadingScreen;
    public AudioSource clickSound;
    public Transform mobileControls;

    [Header("Objects")]
    public Transform floatingTextPrefab;

    [Header("Object Lists")]
    public Transform sceneComponentsHolder;
    public List<Transform> basicEnemyPrefabs;
    public List<Transform> advancedEnemyPrefabs;
    public List<Transform> breakablePrefabs;
    public List<Transform> chestPrefabs;
    public Transform projectilesContainer;
    public Transform popupsContainer;

    [Header("Values")]
    public bool inMenu = false;


    public static GameManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

    }

    private void Start()
    {
        LoadGame();
    }

    public void PlayClickSound()
    {
        clickSound.Play();
    }

    private void LoadGame()
    {
        // Put up the loading screen, unload current scene, then load the actual game scene.
        ToggleLoadingScreen(true);
        dungeonGenerator.GenerateDungeonRoomFirst();
        saveManager.LoadData();

        StartCoroutine(FinishLoading());
    }

    private IEnumerator FinishLoading()
    {
        while (!dungeonGenerator.doneLoading)
        {
            yield return null;
        }
        ToggleLoadingScreen(false);
    }

    public void ToggleLoadingScreen(bool toggle)
    {
        loadingScreen.gameObject.SetActive(toggle);
    }

    internal void ToggleMobileControls(bool toggle)
    {
        mobileControls.gameObject.SetActive(toggle);
    }
}
