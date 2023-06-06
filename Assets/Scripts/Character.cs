using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A character is a collection of 4 parts that can be switched out. Each part has X number of sprites that can be switched between.
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Controls")] public KeyCode SwitchA;
    public KeyCode SwitchB;
    public KeyCode SwitchC;
    public KeyCode SwitchD;

    [Header("Character parts")] public Sprite[] PartA;
    public Sprite[] PartB;
    public Sprite[] PartC;
    public Sprite[] PartD;
    public Image ImageA;
    public Image ImageB;
    public Image ImageC;
    public Image ImageD;

    private readonly int[] _indexes = {0, 0, 0, 0};

    private void Start()
    {
        UpdatePositions(); // default will always be 0000
    }

    private void Update()
    {
        if ((Input.GetKeyDown(SwitchA) || Input.GetKeyDown(SwitchB) || Input.GetKeyDown(SwitchC) ||
             Input.GetKeyDown(SwitchD)) == false)
            return; // early return if no key was pressed

        if (Input.GetKeyDown(SwitchA))
            _indexes[0] = (_indexes[0] + 1) % PartA.Length;
        if (Input.GetKeyDown(SwitchB))
            _indexes[1] = (_indexes[1] + 1) % PartB.Length;
        if (Input.GetKeyDown(SwitchC))
            _indexes[2] = (_indexes[2] + 1) % PartC.Length;
        if (Input.GetKeyDown(SwitchD))
            _indexes[3] = (_indexes[3] + 1) % PartD.Length;

        UpdatePositions();

        Debug.Log($"{name}: {GetCombination()}");
    }

    private void UpdatePositions()
    {
        ImageA.sprite = PartA[_indexes[0]];
        ImageB.sprite = PartB[_indexes[1]];
        ImageC.sprite = PartC[_indexes[2]];
        ImageD.sprite = PartD[_indexes[3]];
    }

    public int GetCombination()
    {
        return _indexes[0] * 1000 + _indexes[1] * 100 + _indexes[2] * 10 + _indexes[3]; // will make a number like 1234
    }
}