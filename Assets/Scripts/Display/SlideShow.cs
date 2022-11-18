using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideShow : MonoBehaviour
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

    public void NextSlide()
    {
        if (curSlide < slides.Count - 1)
        {
            curSlide++;
            image.texture = slides[curSlide];
        }
    }

    public void BackSlide()
    {
        if (curSlide > 0)
        {
            curSlide--;
            image.texture = slides[curSlide];
        }
    }

    public bool IsComplete()
    {
        return curSlide >= slides.Count;
    }
}
