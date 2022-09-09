using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName="New Piece", menuName="Piece")]
public class PieceBase : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] string title;
    [TextArea (2,10)]
    [SerializeField] string description;
    [SerializeField] int tier;

    [SerializeField] Sprite characterSprite;
    [SerializeField] Sprite icon;
    [SerializeField] Sprite movementExample;

    [SerializeField] Color primaryColor;
    [SerializeField] Color secondaryColor;

    public string Name
    {
        get {return characterName; }
    }
    public string Title 
    {
        get {return title;}
    }

    public string Description{
        get { return description;}
    }

    public int Tier
    {
        get {return tier;}
    }

    public Sprite Sprite
    {
        get {return characterSprite; }
    }
    public Sprite Icon
    {
        get {return icon;}
    }

    public Sprite MovementExample
    {
        get {return movementExample;}
    }

    public Color PrimaryColor
    {
        get {return primaryColor;}
    }

    public Color SecondaryColor
    {
        get {return secondaryColor;}
    }
}
