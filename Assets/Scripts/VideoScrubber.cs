using UnityEngine;
using UnityEngine.UI;  // Make sure you have this
using UnityEngine.Video;
using UnityEngine.Events;  // And this

public class VideoScrubber : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider slider;

    private bool isUserScrubbing = false;

    void Start()
    {
        // This is the correct way to add a listener to the slider
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        if (videoPlayer.clip != null)
        {
            slider.maxValue = (float)videoPlayer.clip.length;
        }
    }

    void Update()
    {
        if (!isUserScrubbing && videoPlayer.isPlaying)
        {
            slider.value = (float)videoPlayer.time;
        }
    }

    void OnSliderValueChanged(float value)
    {
        isUserScrubbing = true;
        videoPlayer.time = value;
        isUserScrubbing = false;
    }

    public void UpdateSliderForNewVideo()
    {
        if (videoPlayer.clip != null)
        {
            slider.maxValue = (float)videoPlayer.clip.length;
            slider.value = 0;
        }
    }
}