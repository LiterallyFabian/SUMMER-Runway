using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A character is a collection of 4 parts that can be switched out. Each part has X number of sprites that can be switched between.
/// I've made everything public instead of serializing it because I'm lazy and this is just a demo.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Character : MonoBehaviour
{
    [Header("Controls")] public KeyCode SwitchA;
    public KeyCode SwitchB;
    public KeyCode SwitchC;
    public KeyCode SwitchD;
    public KeyCode SwitchCamera;

    [Header("Character parts")] public Sprite[] PartA;
    public Sprite[] PartB;
    public Sprite[] PartC;
    public Sprite[] PartD;
    public Image ImageA;
    public Image ImageB;
    public Image ImageC;
    public Image ImageD;

    private readonly int[] _indexes = {0, 0, 0, 0};

    [SerializeField] private AudioClip _switchSound;
    [SerializeField] private AudioClip[] _cameraFlashSounds;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _polaroidPrefab;
    [SerializeField] private AudioClip _powerfulCamera;

    public int Combination => _indexes[0] * 1000 + _indexes[1] * 100 + _indexes[2] * 10 + _indexes[3]; // will make a number like 1234

    public int Score { get; set; } = 0;

    private void Start()
    {
        UpdatePositions(); // default will always be 0000

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _switchSound;
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(SwitchA) || Input.GetKeyDown(SwitchB) || Input.GetKeyDown(SwitchC) ||
             Input.GetKeyDown(SwitchD) || Input.GetKeyDown(SwitchCamera)) == false)
            return; // early return if no key was pressed

        if (!_gameManager || _gameManager.PlayingAnimation)
            return;

        if (Input.GetKeyDown(SwitchA))
            _indexes[0] = (_indexes[0] + 1) % PartA.Length;
        if (Input.GetKeyDown(SwitchB))
            _indexes[1] = (_indexes[1] + 1) % PartB.Length;
        if (Input.GetKeyDown(SwitchC))
            _indexes[2] = (_indexes[2] + 1) % PartC.Length;
        if (Input.GetKeyDown(SwitchD))
            _indexes[3] = (_indexes[3] + 1) % PartD.Length;
        if (Input.GetKeyDown(SwitchCamera) && Application.isEditor)
            StartCoroutine(PlayCameraFlashes());

        _audioSource.time = 0;
        _audioSource.Play();

        UpdatePositions();

        if (_gameManager != null)
            _gameManager.CheckCombination(this, Combination);

        _scoreText.text = Score.ToString();

        Debug.Log($"{name}: {Combination}");
    }

    private void UpdatePositions()
    {
        ImageA.sprite = PartA[_indexes[0]];
        ImageB.sprite = PartB[_indexes[1]];
        ImageC.sprite = PartC[_indexes[2]];
        ImageD.sprite = PartD[_indexes[3]];
    }

    public IEnumerator PlayCameraFlashes()
    {
        _gameManager.PlayingAnimation = true;

        CreatePolaroid();

        for (int i = 0; i < 10; i++)
        {
            GameObject flash = PlayCameraFlash();
            Destroy(flash, Random.Range(0.2f, 0.3f));

            // create a new temporary audio source
            AudioSource audioSource = flash.AddComponent<AudioSource>();
            audioSource.clip = _cameraFlashSounds[Random.Range(0, _cameraFlashSounds.Length)];
            audioSource.Play();
            Destroy(audioSource, audioSource.clip.length);
            yield return new WaitForSeconds(Random.Range(0.06f, 0.2f));
        }
        
        _gameManager.SetNextPose();

        _gameManager.PlayingAnimation = false;
    }

    public GameObject PlayCameraFlash()
    {
        GameObject flash = CreateClone();
        flash.name = $"{name} (flash)";

        // move it behind the original
        flash.transform.SetSiblingIndex(0);

        // make all images black
        foreach (Image image in flash.GetComponentsInChildren<Image>())
        {
            image.color = new Color(0, 0, 0, 0.4f);
        }

        // move X & Y up by 5-20 units each
        flash.transform.Translate(Random.Range(-50, 50), Random.Range(2, 20), 0);

        return flash;
    }

    public void CreatePolaroid()
    {
        GameObject polaroid = Instantiate(_polaroidPrefab, GameObject.Find("Canvas/UI").transform, false);
        GameObject clone = CreateClone();
        GameObject polaroidCharacter = polaroid.transform.Find("Character").gameObject;
        clone.transform.SetParent(polaroidCharacter.transform);
        clone.transform.localPosition = polaroidCharacter.transform.localPosition;
        clone.transform.localRotation = polaroidCharacter.transform.localRotation;
        clone.transform.localScale = Vector3.one;
        
        AudioSource audioSource = polaroid.AddComponent<AudioSource>();
        audioSource.clip = _powerfulCamera;
        audioSource.Play();
    }

    private GameObject CreateClone()
    {
        Transform t = transform;
        GameObject clone = Instantiate(gameObject, t.position, t.rotation);
        Destroy(clone.GetComponent<Character>());
        clone.transform.SetParent(t.parent);
        return clone;
    }
}