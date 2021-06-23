using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atlas : MonoBehaviour
{
   // [SerializeField]
    private List<Sprite> spritesData = new List<Sprite>();

    private Dictionary<string, Sprite> atlasDict = new Dictionary<string, Sprite>();
    // Start is called before the first frame update
   
    public void SetUp()
    {
        object[] loadedIcons = Resources.LoadAll("Sprites", typeof(Sprite));
        for (int x = 0; x < loadedIcons.Length; x++)
        {
           spritesData.Add((Sprite)loadedIcons[x]);
        }
        foreach (Sprite _sprite in spritesData)
        {
            if (!atlasDict.ContainsKey(_sprite.name))
                atlasDict.Add(_sprite.name, _sprite);
        }
    }

    public Sprite GetSprite(string name)
    {
        Sprite value = null;
        atlasDict.TryGetValue(name, out value);
        return value;
    }    
}
