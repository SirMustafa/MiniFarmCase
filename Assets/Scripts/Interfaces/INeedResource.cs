using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedResource
{
    Sprite NeededResourceSprite();
    int InputResource();
    int OutputResource();
}