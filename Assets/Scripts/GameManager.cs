using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    private Pose[] _poses;
    private Pose _currentPose;
    private int _poseIndex = 0;


    private Character[] _players;
    private AudioSource _audioSource;

    [SerializeField] private Image _frame;
    [SerializeField] private Image _feedImage;
    [SerializeField] private Sprite[] _instagramFeeds;
    private int _instagramFeedIndex = 0;

    [Header("Audio")] [SerializeField] private AudioClip _correctSound;

    [SerializeField] private GameObject _parentUI;
    [SerializeField] private GameObject _parentGame;
    [SerializeField] private GameObject _parentIntro;
    [SerializeField] private GameObject _parentVictory;

    private bool _gameStarted = false;
    private float _gameTime = 60f;
    private float _originalGameTime = 60f;
    [SerializeField] private Image _fillBar;
    private bool _playingTickSound = false;
    private bool _gameEnded = false;
    [SerializeField] private AudioClip _tickSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _cameraSound;
    [SerializeField] private Animator _flashAnimator;

    [SerializeField] private Text _winText;
    [SerializeField] private Text _statText;

    [SerializeField] private AudioLowPassFilter _lowpassFilter;
    public bool PlayingAnimation { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        _parentVictory.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _correctSound;

        _poses = Resources.LoadAll<Pose>("Positions");
        Debug.Log($"Loaded {_poses.Length} poses.");


        System.Random random = new System.Random();
        random.Shuffle(_poses);
        random.Shuffle(_instagramFeeds);

        _players = FindObjectsOfType<Character>();
        SetNextPose();

        if (!Application.isEditor)
        {
            _parentGame.SetActive(false);
            _parentUI.SetActive(false);
            _parentIntro.SetActive(true);
        }
        else
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.H))
            StartCoroutine(Win());

        if (!_gameStarted)
            return;

        _gameTime -= Time.deltaTime;
        _fillBar.fillAmount = _gameTime / _originalGameTime;


        if (_gameTime < 11 && !_playingTickSound)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = _tickSound;
            source.Play();
            _playingTickSound = true;
        }

        if (_gameTime < 0 && !_gameEnded)
        {
            _gameEnded = true;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = _winSound;
            source.Play();

            StartCoroutine(Win());
        }
    }

    public bool CheckCombination(Character character, int combination)
    {
        if (_currentPose == null || _currentPose.Code != combination)
            return false;

        Score(character);

        return true;
    }

    private void Score(Character character)
    {
        Debug.Log($"{character.name} posed! Score: {character.Score}");

        _audioSource.time = 0;
        _audioSource.Play();

        StartCoroutine(character.PlayCameraFlashes());

        // start NegativeReactions for the other players
        foreach (Character player in _players)
        {
            if (player == character)
                continue;

            StartCoroutine(player.NegativeReactions());
        }
    }

    public void SetNextPose()
    {
        _currentPose = _poses[_poseIndex];
        _poseIndex = (_poseIndex + 1) % _poses.Length;
        _frame.sprite = _currentPose.Reference;

        _feedImage.sprite = _instagramFeeds[_instagramFeedIndex];
        _instagramFeedIndex = (_instagramFeedIndex + 1) % _instagramFeeds.Length;
    }

    public void StartGame()
    {
        _parentGame.SetActive(true);
        _parentUI.SetActive(true);
        _parentIntro.SetActive(false);
        _parentVictory.SetActive(false);

        _gameStarted = true;

        Destroy(_lowpassFilter);
    }

    private IEnumerator Win()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = _cameraSound;
        source.Play();

        _flashAnimator.Play("Flash");
        yield return new WaitForSeconds(0.1f);

        _parentVictory.SetActive(true);
        _parentGame.SetActive(false);

        Character mostScore = _players[0];
        string statText = "FOLLOWER COUNTS:\n";
        for (int i = 0; i < _players.Length; i++)
        {
            Character c = _players[i];

            if (mostScore == null || mostScore.Score < c.Score)
            {
                mostScore = c;
                _winText.text = $"CONGRATS P{i + 1}, YOU ARE TRENDY";
            }

            statText += $"P{i + 1}: {c.Score}\n";
        }

        _statText.text = statText;

        GameObject clone = mostScore.CreateClone();
        clone.transform.localScale = Vector3.one;
        clone.transform.SetParent(_parentVictory.transform);
        clone.transform.SetSiblingIndex(1);

        RectTransform rect = clone.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localPosition = Vector3.zero;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }
}