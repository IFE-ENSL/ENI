using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Simple class that plays the introduction video and enables the user to skip it
public class MoviePlayBack : MonoBehaviour {

    MovieTexture movie;
    AsyncOperation async;
    public Text LoadingText;
    public Text SkipText;
    float time;
    int vsyncPrevious;

    void Start ()
    {
        vsyncPrevious = QualitySettings.vSyncCount;
        QualitySettings.vSyncCount = 0;
        LoadingText.enabled = false;
        SkipText.enabled = false;
        RawImage r = GetComponent<RawImage>();
        movie = (MovieTexture)r.mainTexture;
        movie.loop = false;
        movie.Play();
        async = SceneManager.LoadSceneAsync("Login");
        async.allowSceneActivation = false;
    }

	// Update is called once per frame
	void Update ()
    {
	    if (!movie.isPlaying)
        {
            LoadingText.enabled = true;
            QualitySettings.vSyncCount = vsyncPrevious; //According to some forums it helps improves video Playback performances, but I have doubts about it...
            async.allowSceneActivation = true;
        }

        if (SkipText.enabled)
        {
            time += Time.deltaTime;

            if (time > 3f)
            {
                time = 0;
                SkipText.enabled = false;
            }
        }

        if (movie.isPlaying && Input.anyKeyDown)
        {
            if(!SkipText.enabled) //If a key is pressed, first display the text "Press again to skip..." for 3 seconds (See script up)
                SkipText.enabled = true;
            else
            {
                QualitySettings.vSyncCount = vsyncPrevious; //if texte already displayed, skip the intro
                async.allowSceneActivation = true;
            }
        }

       
	}
}
