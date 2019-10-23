using UnityEngine;
using UnityEngine.Video;

namespace VIC.Creator.UI
{
    public class VideoEndCheck : MonoBehaviour
    {
        [Header("RESOURCES")]
        public Animator backgroundAnimator;
        public VideoPlayer loopVideo;
        private VideoPlayer vidPlayer;

        void Start()
        {
            vidPlayer = this.GetComponent<VideoPlayer>();
            vidPlayer.Prepare();
            vidPlayer.loopPointReached += CheckOver;
            loopVideo.Prepare();
        }

        void CheckOver(UnityEngine.Video.VideoPlayer vp)
        {
            loopVideo.Play();
            backgroundAnimator.Play("Plexus Loop");
        }
    }
}