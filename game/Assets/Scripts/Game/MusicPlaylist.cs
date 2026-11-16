using System.Collections.Generic;
using UnityEngine;

namespace Saga.Game
{
    // Battle music: plays its tracks one after another, starting from a random one, and crossfades each into the
    // next over its last few seconds, so a long battle never sits on one looping piece. Unscaled time, so pausing
    // the battle does not stop the music.
    public class MusicPlaylist : MonoBehaviour
    {
        const float Crossfade = 6f;

        AudioSource a, b;
        List<AudioClip> clips;
        int index;
        float volume, fade = -1f;

        public void Begin(AudioSource source, List<AudioClip> tracks)
        {
            a = source;
            volume = source.volume;
            b = gameObject.AddComponent<AudioSource>();
            b.playOnAwake = false; b.spatialBlend = 0f; b.loop = false; b.volume = 0f;
            a.loop = false;
            clips = tracks;
            index = Random.Range(0, clips.Count);
            a.Stop();
            a.clip = clips[index];
            a.volume = volume;
            a.Play();
        }

        const float DuckFade = 1f;   // seconds to fade between full and half volume (story calls)
        float level = 1f;

        void Update()
        {
            if (a == null || a.clip == null) return;
            float target = SoundBoard.Current != null ? SoundBoard.Current.MusicTarget : 1f;
            level = Mathf.MoveTowards(level, target, Time.unscaledDeltaTime * 0.5f / DuckFade);
            if (fade < 0f) a.volume = volume * level;
            // Start the next track as this one nears its end.
            if (fade < 0f && (!a.isPlaying || a.time >= a.clip.length - Crossfade))
            {
                index = (index + 1) % clips.Count;
                b.clip = clips[index];
                b.volume = 0f;
                b.Play();
                fade = 0f;
            }
            if (fade >= 0f)
            {
                fade += Time.unscaledDeltaTime / Crossfade;
                float k = Mathf.Clamp01(fade);
                a.volume = volume * level * (1f - k);
                b.volume = volume * level * k;
                if (k >= 1f)
                {
                    a.Stop();
                    (a, b) = (b, a);
                    fade = -1f;
                }
            }
        }
    }
}
