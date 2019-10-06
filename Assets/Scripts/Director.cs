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
    private Coroutine _levelCoroutine;

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

    void Update()
    {
        if (Input.GetButtonDown("OpenMenu"))
        {
            _menu.SetActive(true);
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

        UpdateLevelData();
    }

    void UpdateLevelData()
    {
        // Gather level-specific data from newly loaded level
        _grid = GameObject.Find("Grid");
        _layers = _grid.GetComponentsInChildren<ColorLayer>(includeInactive: true);

        // Move player to start and set start color
        {
            var playerStartGO = GameObject.FindGameObjectWithTag("PlayerStart");
            if (playerStartGO != null)
            {
                var playerStart = playerStartGO.GetComponent<PlayerStart>();
                Player.transform.position = playerStartGO.transform.position;
                // Force even if player has the same color, because level changed
                Player.SetColor(playerStart.StartColor, force: true);
                Player.SetScale(playerStart.StartScale);
            }
        }

        // Start level
        {
            var level = _grid.GetComponentInChildren<Level>(includeInactive: true);
            if (level != null)
            {
                // Crossfade BGM if needed

                // Find the only paying BGM source, and destroy all others
                AudioSource curBgm = null;
                {
                    var allBgms = GetComponents<AudioSource>();
                    foreach (var bgm in allBgms)
                    {
                        if (bgm.isPlaying)
                        {
                            curBgm = bgm;
                        }
                        else
                        {
                            Destroy(bgm);
                        }
                    }
                }

                // Create the loop BGM audio source, and the intro one if any, then crossfade
                if ((curBgm == null) || ((level.BackgroundMusicIntro != curBgm.clip) && (level.BackgroundMusicLoop != curBgm.clip)))
                {
                    AudioSource newBgmIntro = null;
                    AudioSource newBgmLoop = null;
                    if (level.BackgroundMusicIntro != null)
                    {
                        newBgmIntro = gameObject.AddComponent<AudioSource>();
                        newBgmIntro.volume = 0f;
                        newBgmIntro.loop = false;
                        newBgmIntro.clip = level.BackgroundMusicIntro;
                    }
                    if (level.BackgroundMusicLoop != null)
                    {
                        newBgmLoop = gameObject.AddComponent<AudioSource>();
                        newBgmLoop.volume = 0f;
                        newBgmLoop.loop = true;
                        newBgmLoop.clip = level.BackgroundMusicLoop;
                    }
                    if (newBgmIntro != null)
                    {
                        newBgmLoop.volume = 1f;
                        newBgmIntro.Play();
                        newBgmLoop.PlayDelayed(newBgmIntro.clip.length);
                        StartCoroutine(CrossFade(curBgm, newBgmIntro));
                    }
                    else if (newBgmLoop != null)
                    {
                        newBgmLoop.Play();
                        StartCoroutine(CrossFade(curBgm, newBgmLoop));
                    }
                }

                // Start level-specific logic
                _levelCoroutine = StartCoroutine(level.StartLevel());
            }

        }
    }

    IEnumerator CrossFade(AudioSource oldSource, AudioSource newSource)
    {
        const int numSteps = 180;
        float vol = 0.0f;
        float dvol = 1.0f / numSteps;
        for (int i = 0; i < numSteps; ++i)
        {
            vol += dvol;
            if (oldSource != null)
            {
                oldSource.volume = 1.0f - vol;
            }
            newSource.volume = vol;
            yield return null;
        }

        if (oldSource != null)
        {
            Destroy(oldSource);
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
        yield return _coroutine;
    }

    public void CancelDialog()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
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

    public void ChangeLevelTo(string levelName)
    {
        // Start coroutine from this GameObject, so that scene unloading
        // does not affect the coroutine. Otherwise if started from a GO
        // inside the old scene it'll get aborted when the scene is unloaded.
        StartCoroutine(LoadLevelAsync(levelName));
    }

    IEnumerator LoadLevelAsync(string levelName)
    {
        EndDialog(false);
        PlayerController.enabled = false;

        // Stop any level logic before unloading
        if (_levelCoroutine != null)
        {
            StopCoroutine(_levelCoroutine);
            _levelCoroutine = null;
        }

        // Manage scenes
        if (_currentLevelName != levelName)
        {
            // Swap level scenes
            yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            var oldScene = _levelScene;
            _levelScene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(_levelScene);
            yield return SceneManager.UnloadSceneAsync(oldScene, UnloadSceneOptions.None);
            _currentLevelName = levelName;
        }
        else
        {
            // Reload current level
            SceneManager.SetActiveScene(_baseScene);
            yield return SceneManager.UnloadSceneAsync(_currentLevelName, UnloadSceneOptions.None);
            yield return SceneManager.LoadSceneAsync(_currentLevelName, LoadSceneMode.Additive);
            _levelScene = SceneManager.GetSceneByName(levelName);
            SceneManager.SetActiveScene(_levelScene);
        }

        // Start new level logic
        UpdateLevelData();

        PlayerController.enabled = true;
    }

    IEnumerator WaitForUse()
    {
        // Wait for "Use" button, and also wait for the menu to be closed,
        // so that the menu inputs are not consumed by the dialog.
        while (_menu.activeSelf || !Input.GetButtonDown("Use"))
            yield return null;
    }

    IEnumerator ShowButtonWithDelay(float dt)
    {
        yield return new WaitForSeconds(dt);
        _button.SetActive(true);
    }

    public void ShowMenu()
    {
        CancelDialog();
        PlayerController.enabled = false;
        _menu.SetActive(true);
    }

    public void RestartLevel()
    {
        _menu.SetActive(false);
        StartCoroutine(LoadLevelAsync(_currentLevelName));
    }

    public void ExitGame()
    {
        //< TODO - Exit title?
        Application.Quit();
    }

    public void PlaySfx(AudioClip sfxClip)
    {
        var source = Player.GetComponent<AudioSource>();
        source.Stop();
        source.clip = sfxClip;
        source.volume = 1f;
        source.Play();
    }
}
