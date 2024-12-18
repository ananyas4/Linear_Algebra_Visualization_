using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneChanger : MonoBehaviour
{

    public TMP_Text title;
    public TMP_Text titleDescription;

    public VideoPlayer videoPlayer;


    // Start is called before the first frame update
    void Start()
    {
        GameObject titleObject = GameObject.FindWithTag("Title");
        title = titleObject.GetComponent<TMP_Text>();

        GameObject descriptionObject = GameObject.FindWithTag("TitleDescription");
        titleDescription = descriptionObject.GetComponent<TMP_Text>();

        GameObject videoObject = GameObject.FindWithTag("Video");
        videoPlayer = videoObject.GetComponent<VideoPlayer>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LinearTransformationButtonClicked()
    {
        videoPlayer.Stop();

        title.text = "Chapter One: Linear Transformations";
        titleDescription.text = "A linear transformation is basically changing the cartesian coordinate system to whatever you'd like (as long as the origin stays fixed and grid lines remain evenly spaced).";

        videoPlayer.clip = Resources.Load<VideoClip>("Videos/LinearTransformations/LinTransf") as VideoClip;
        videoPlayer.Play();

    }

    public void MatrixButtonClicked()
    {
        Debug.Log("Matrix button clicked");
        videoPlayer.Stop();

        title.text = "Chapter Two: Matrix Multiplication";
        titleDescription.text = "A matrix represents the new location of the basis vectors (the unit vectors in our standard Cartesian coordinate system) after a linear transformation. When you multiply matrices, you essentially apply one transformation, then the next.";

        VideoClip newClip = Resources.Load<VideoClip>("Videos/MatrixMultiplication/MatrixMultiplication") as VideoClip;
        Debug.Log($"Loaded clip: {(newClip != null ? newClip.name : "null")}");

        videoPlayer.clip = newClip;
        Debug.Log($"Current video clip: {(videoPlayer.clip != null ? videoPlayer.clip.name : "null")}");

        videoPlayer.Play();

    }
}
