using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName="New Piece", menuName="Piece")]
public class PieceDataBase : ScriptableObject
{
    [SerializeField] string title;
    [TextArea (2,10)]
    [SerializeField] string description;
    [SerializeField] int tier;

    [SerializeField] Sprite icon;

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

    public Sprite Icon
    {
        get {return icon;}
    }
}
