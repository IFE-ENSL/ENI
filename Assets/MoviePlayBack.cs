using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoviePlayBack : MonoBehaviour {

    MovieTexture movie;
    AsyncOperation async;
    public Text LoadingText;
    public Text SkipText;
    float time;
    int vsyncPrevious;

    // Use this for initialization
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
            QualitySettings.vSyncCount = vsyncPrevious;
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
            if(!SkipText.enabled)
                SkipText.enabled = true;
            else
            {
                QualitySettings.vSyncCount = vsyncPrevious;
                async.allowSceneActivation = true;
            }
        }

       
	}
}
