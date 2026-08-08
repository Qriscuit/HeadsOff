using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureMover : MonoBehaviour
{
    public Image Pattern;
    public Renderer rend;
    public float scrollSpeed;
    public int MatNum;

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        if (Pattern == null) rend.materials[MatNum].SetTextureOffset("_BaseMap", new Vector2(offset, 0));
        //else Pattern.sprite.text = new Vector2(offset, 0);
    }
}
