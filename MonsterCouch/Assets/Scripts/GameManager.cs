using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions.Must;
using Unity.VisualScripting.FullSerializer;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Gameplay
    }

    [System.Serializable]
    private struct EnemyInfo
    {
        public Vector2 position;
        public Vector2 currentSpeed;
        public Vector2 targetSpeed;
        public int health;
    }

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float playerSize;
    [SerializeField]
    private float enemySpeed;
    [SerializeField]
    private float enemySize;
    [SerializeField]
    private float enemyRunAwayThreshold = 0.1f;
    [SerializeField]
    private int enemyCount;

    [SerializeField]
    private EnemyInfo[] enemies;
    private Transform[] spawnedEnemiesTransforms;
    private Vector2 playerPosition;
    private Vector2 currentPlayerSpeed;
    private Vector2 targetPlayerSpeed;
    private Transform spawnedPlayerTransform;
    private float screenRatio;
    private float cameraSize;
    private GameObject gameObjectsRoot;


    private GameState currentGameState;

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }



    public void StartGame()
    {
        currentGameState = GameState.Gameplay;
        StartNewGame();
    }
    public void MainMenu()
    {
        currentGameState = GameState.MainMenu;
        StopGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HandleKeyboardInput()
    {
        switch(currentGameState)
        {
            case GameState.MainMenu:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        UIManager.Instance.Back();
                    }
                }
                break;
            case GameState.Gameplay:
                {

                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        MoveUp();
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        MoveDown();
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        MoveLeft();
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        MoveRight();
                    }
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        MainMenu();
                    }
                }
                break;
        }
    }

    void StartNewGame()
    {
        gameObjectsRoot.SetActive(true);
        currentGameState = GameState.Gameplay;
        UIManager.Instance.Gameplay();
        screenRatio = Screen.width / Screen.height;

        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].position = new Vector2(Random.Range(-screenRatio * cameraSize + 0.5f*enemySize, screenRatio * cameraSize - 0.5f * enemySize), Random.Range(-cameraSize + 0.5f * enemySize, cameraSize + 0.5f * enemySize));
            enemies[i].currentSpeed = Vector2.zero;
            enemies[i].targetSpeed = Vector2.zero;
            enemies[i].health = 1;

        }

        playerPosition = new Vector2(0.0f, 0.0f);


        currentPlayerSpeed = Vector2.zero;
        targetPlayerSpeed = Vector2.zero;
    }

    void StopGame()
    {
        currentGameState = GameState.MainMenu;

        gameObjectsRoot.SetActive(false);
        UIManager.Instance.MainMenu();
    }

    void MoveUp()
    {
        targetPlayerSpeed.y += 1.0f;
    }
    void MoveDown()
    {

        targetPlayerSpeed.y -= 1.0f;
    }
    void MoveLeft()
    {

        targetPlayerSpeed.x -= 1.0f;
    }
    void MoveRight()
    {

        targetPlayerSpeed.x += 1.0f;
    }

    void FixedUpdate()
    {
        targetPlayerSpeed = Vector2.zero;
        HandleKeyboardInput();
        if (currentGameState == GameState.Gameplay)
        {

            screenRatio = Screen.width / Screen.height;

            currentPlayerSpeed = Vector2.Lerp(currentPlayerSpeed, targetPlayerSpeed, Time.deltaTime);

            playerPosition += playerSpeed * currentPlayerSpeed * Time.deltaTime;

            playerPosition.x = Mathf.Clamp(playerPosition.x, -screenRatio * cameraSize + 0.5f * playerSize, screenRatio * cameraSize - 0.5f * playerSize);
            playerPosition.y = Mathf.Clamp(playerPosition.y, -cameraSize + 0.5f * playerSize, cameraSize - 0.5f * playerSize);

            spawnedPlayerTransform.position = playerPosition;

            for (int i = 0; i < enemyCount; ++i)
            {
                if (enemies[i].health > 0)
                {
                    Vector2 diff = (enemies[i].position - playerPosition);
                    float diffLength = diff.magnitude;
                    if (diffLength < enemyRunAwayThreshold)
                    {
                        enemies[i].targetSpeed = diff / diffLength;
                    }
                    if (diffLength < 0.5f * (enemySize + playerSize))
                    {
                        enemies[i].health = 0;
                    }
                }
            }

            for (int i = 0; i < enemyCount; ++i)
            {
                if (enemies[i].health > 0)
                {
                    enemies[i].currentSpeed = Vector2.Lerp(enemies[i].currentSpeed, enemies[i].targetSpeed, Time.deltaTime);
                    enemies[i].position += enemySpeed * enemies[i].currentSpeed * Time.deltaTime;
                    enemies[i].position.x = Mathf.Clamp(enemies[i].position.x, -screenRatio * cameraSize + 0.5f * enemySize, screenRatio * cameraSize - 0.5f * enemySize);
                    enemies[i].position.y = Mathf.Clamp(enemies[i].position.y, -cameraSize + 0.5f * enemySize, cameraSize - 0.5f * enemySize);
                }
            }

            for (int i = 0; i < enemyCount; ++i)
            {
                EnemyInfo currentEnemy = enemies[i];
                spawnedEnemiesTransforms[i].position = currentEnemy.position;
                
            }
        }

    }

    void Awake()
    {
        gameObjectsRoot = new GameObject();
        gameObjectsRoot.transform.parent = transform;

        spawnedPlayerTransform = Instantiate(playerPrefab).transform;
        spawnedPlayerTransform.parent = gameObjectsRoot.transform;
        spawnedPlayerTransform.localScale = playerSize * Vector3.one;

        spawnedEnemiesTransforms = new Transform[enemyCount];
        for(int i = 0; i < enemyCount; ++i)
        {
            spawnedEnemiesTransforms[i] = Instantiate(enemyPrefab).transform;

            spawnedEnemiesTransforms[i].parent = gameObjectsRoot.transform;
            spawnedEnemiesTransforms[i].localScale = enemySize * Vector3.one;
        }

        gameObjectsRoot.SetActive(false);
        enemies = new EnemyInfo[enemyCount];

        _instance = this;
        cameraSize = Camera.main.orthographicSize;
    }

    private void Start()
    {
        MainMenu();
    }
}
