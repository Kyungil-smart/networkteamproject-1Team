using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    public Image image;
    public PressAction _pressAction;
    public Sprite _sprite;
    public Sprite _changeSprite;
    

    private void Start()
    {
        image.sprite = _sprite;
        
        _pressAction.OnPressCompleted += OnPressCompleted;
    }

    private void OnPressCompleted()
    {
        image.sprite = _changeSprite;
    }
}
