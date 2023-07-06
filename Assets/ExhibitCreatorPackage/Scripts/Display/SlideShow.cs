using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideShow : DisplayBehavior
{
    public List<Texture2D> slides;
    public RawImage image;

    int curSlide = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (slides.Count > 0)
        {
            image.texture = slides[0];
        }
    }

    public override void OnMainInput()
    {
        if (curSlide < slides.Count - 1)
        {
            curSlide++;
            image.texture = slides[curSlide];
        }
    }

    public override void OnSecondaryInput()
    {
        if (curSlide > 0)
        {
            curSlide--;
            image.texture = slides[curSlide];
        }
    }
}
