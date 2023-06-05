using UnityEngine;

/// <summary>
/// A character is a collection of 4 parts that can be switched out. Each part has X number of sprites that can be switched between.
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Controls")] public KeyCode SwitchA;
    public KeyCode SwitchB;
    public KeyCode SwitchC;
    public KeyCode SwitchD;

    [Header("CharacterParts")] public Sprite[] PartA;
    public Sprite[] PartB;
    public Sprite[] PartC;
    public Sprite[] PartD;
    public SpriteRenderer RendererA;
    public SpriteRenderer RendererB;
    public SpriteRenderer RendererC;
    public SpriteRenderer RendererD;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}