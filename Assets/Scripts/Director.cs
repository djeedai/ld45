using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TextSnippet
{
    public string text;
    public bool append = false;
}

public class TextDialog
{
    public List<TextSnippet> Snippets = new List<TextSnippet>();
}

public class Director : MonoBehaviour
{
    public static Director Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindGameObjectWithTag("Director").GetComponent<Director>();
            }
            return _instance;
        }
    }

    private static Director _instance;

    public string StartLevelName;
    public Player Player { get; private set; }
    public PlayerController PlayerController { get; private set; }

    private TMPro.TextMeshPro _text;
    private GameObject _frame;
    private GameObject _button;
    private GameObject _grid;
    private GameObject _menu;
    private ColorLayer[] _layers;
    private IEnumerator _dialog;
    private Coroutine _coroutine;
    private string _currentLevelName;
    private Scene _baseScene;
    private Scene _levelScene;
    private bool _controllerWasActive;

    void Start()
    {
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        Player = playerGameObject.GetComponent<Player>();
        PlayerController = playerGameObject.GetComponent<PlayerController>();
        _text = GetComponentInChildren<TMPro.TextMeshPro>(includeInactive: true);
        _frame = gameObject.transform.Find("Frame").gameObject;
        _frame.SetActive(true); // For Find() below
        _button = GameObject.Find("SkipButton");
        _frame.SetActive(false);
        _menu = gameObject.transform.Find("Menu").gameObject;

        _text.enabled = false;
        //_frame.SetActive(false);
        _button.SetActive(false);
        _menu.SetActive(false);

        // Get base scene (always loaded)
        _baseScene = SceneManager.GetSceneByName("BaseScene");

        // Ensure start level scene is loaded and active
        {
            var co = LoadStartLevel();
            StartCoroutine(co);
        }
    }

    IEnumerator LoadStartLevel()
    {
        // Load the start scene
        _currentLevelName = StartLevelName;
        _levelScene = SceneManager.GetSceneByName(_currentLevelName);
        if ((_levelScene == null) || !_levelScene.isLoaded)
        {
            var loadParams = new LoadSceneParameters
            {
                loadSceneMode = LoadSceneMode.Additive,
                localPhysicsMode = LocalPhysicsMode.None // use global physics scene, not a separate local one
            };
            var op = SceneManager.LoadSceneAsync(_currentLevelName, loadParams);
            while (!op.isDone)
            {
                yield return null;
            }

            // This should succeed now
            _levelScene = SceneManager.GetSceneByName(_currentLevelName);
        }
        SceneManager.SetActiveScene(_levelScene);

        // Unload all other scenes
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            var scene = SceneManager.GetSceneAt(i);
            if ((scene != _baseScene) && (scene != _levelScene) && scene.isLoaded)
            {
                var op = SceneManager.UnloadSceneAsync(scene);
                while (!op.isDone)
                {
                    yield return null;
                }
            }
        }

        // Prepare level
        _grid = GameObject.Find("Grid");
        _layers = _grid.GetComponentsInChildren<ColorLayer>(includeInactive: true);

        // Start level
        {
            var level = _grid.GetComponentInChildren<Level>(includeInactive: true);
            var co = level.StartLevel();
            StartCoroutine(co);
        }
    }

    public void DisableLayer(LogicColor color)
    {
        foreach (var layer in _layers)
        {
            layer.gameObject.GetComponent<TilemapCollider2D>().enabled = (layer.Color != color);
        }
    }

    public void StartDialog(TextDialog dialog, bool block = true)
    {
        _dialog = StartDialogImpl(dialog, block);
        _coroutine = StartCoroutine(_dialog);
    }

    public IEnumerator WaitForDialog(TextDialog dialog, bool block = true)
    {
        _dialog = StartDialogImpl(dialog, block);
        _coroutine = StartCoroutine(_dialog);
        return _dialog;
    }

    public void CancelDialog()
    {
        StopCoroutine(_coroutine);
        _coroutine = null;
        _dialog = null;
        EndDialog(false);
    }

    IEnumerator StartDialogImpl(TextDialog dialog, bool block)
    {
        _controllerWasActive = PlayerController.enabled;
        if (block)
        {
            PlayerController.enabled = false;
        }
        _text.enabled = true;
        _frame.SetActive(true);

        foreach (var snippet in dialog.Snippets)
        {
            if (snippet.append)
            {
                _text.text += "\n" + snippet.text;
            }
            else
            {
                _text.text = snippet.text;
            }
            var co_bt = ShowButtonWithDelay(2.0f);
            StartCoroutine(co_bt);
            yield return null; // wait for next loop so the GetButtonDown() reset
            yield return StartCoroutine(WaitForUse());
            StopCoroutine(co_bt);
            _button.SetActive(false);
        }

        EndDialog(block);
    }

    void EndDialog(bool block)
    {

        _text.enabled = false;
        _frame.SetActive(false);
        if (block)
        {
            PlayerController.enabled = _controllerWasActive;
        }
    }

    public IEnumerator LoadLevelAsync(string levelName)
    {
        PlayerController.enabled = false;

        {
            AsyncOperation op = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            while (!op.isDone)
            {
                yield return null;
            }
        }

        SceneManager.UnloadSceneAsync(_levelScene, UnloadSceneOptions.None);
        _levelScene = SceneManager.GetSceneByName(levelName);
        SceneManager.SetActiveScene(_levelScene);

        _currentLevelName = levelName;
        PlayerController.enabled = true;
    }

    IEnumerator WaitForUse()
    {
        while (!Input.GetButtonDown("Use"))
            yield return null;
    }

    IEnumerator ShowButtonWithDelay(float dt)
    {
        yield return new WaitForSeconds(dt);
        _button.SetActive(true);
    }

    public IEnumerator ShowMenu()
    {
        CancelDialog();
        PlayerController.enabled = false;
        _menu.SetActive(true);
        yield return null;
    }
}
