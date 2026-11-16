using System.Collections.Generic;
using UnityEngine;

namespace Saga.Game
{
    // All game audio: music, weapon and battle sounds, interface sounds. Uses the 2017 game's own clips.
    // Battle sounds are quieter the further they are from where the camera is looking, panned by screen position,
    // and each clip has a minimum gap so a big fight doesn't turn into noise. M mutes everything.
    public class SoundBoard
    {
        public static SoundBoard Current { get; private set; }
        const string MuteKey = "saga.muted";
        const int Voices = 24;
        const float NearDistance = 1800f;  // camera this close to a sound: full volume
        const float FarDistance = 16000f;  // this far or more: barely audible (camera zooms out to 24,000)

        readonly ShipLibrary lib;
        readonly Camera cam;
        readonly AudioSource music;
        readonly AudioSource[] voices = new AudioSource[Voices];
        readonly Dictionary<AudioClip, float> lastPlayed = new Dictionary<AudioClip, float>();
        int next;

        public static bool Muted => PlayerPrefs.GetInt(MuteKey, 0) == 1;

        public SoundBoard(ShipLibrary lib, GameObject host, Camera cam)
        {
            this.lib = lib; this.cam = cam;
            music = host.AddComponent<AudioSource>();
            music.loop = true; music.playOnAwake = false; music.volume = 0.35f; music.spatialBlend = 0f;
            for (int i = 0; i < Voices; i++)
            {
                var v = host.AddComponent<AudioSource>();
                v.playOnAwake = false; v.spatialBlend = 0f;
                voices[i] = v;
            }
            AudioListener.volume = Muted ? 0f : 1f;
            Current = this;
        }

        public void ToggleMute()
        {
            PlayerPrefs.SetInt(MuteKey, Muted ? 0 : 1);
            PlayerPrefs.Save();
            AudioListener.volume = Muted ? 0f : 1f;
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || music.clip == clip) return;
            music.clip = clip;
            music.Play();
        }

        // ---------- battle ----------

        public void Shot(string visual, bool fromCraft, Vector3 at)
        {
            switch (visual)
            {
                case "shell": At(lib.sfxCannon, at, 0.55f, 0.06f, 0.08f); break;
                case "missile": At(lib.sfxMissile, at, 0.4f, 0.08f, 0.1f); break;
                case "beam": At(lib.sfxBeam, at, 0.5f, 0.2f, 0.05f); break;
                case "bomb": At(lib.sfxRailgun != null ? lib.sfxRailgun : lib.sfxShotgun, at, 0.4f, 0.1f, 0.08f); break;
                case "seeker": At(lib.sfxMissile, at, 0.18f, 0.08f, 0.15f, 1.5f); break;
                case "repair_beam": At(lib.sfxSelect, at, 0.12f, 0.3f, 0f, 0.7f); break;
                default:
                    if (fromCraft) At(lib.sfxMachinegun, at, 0.22f, 0.07f, 0.12f);
                    else At(lib.sfxGatling, at, 0.3f, 0.09f, 0.1f);
                    break;
            }
        }

        public void Explosion(Vector3 at, bool big)
        {
            At(lib.sfxExplosion, at, big ? 0.9f : 0.45f, 0.05f, 0.15f, big ? 0.7f : 1.2f);
            if (big) At(lib.sfxKick, at, 0.7f, 0.05f, 0.1f, 0.8f);
        }

        public void Warp(Vector3 at) => At(lib.sfxWarp, at, 0.45f, 0.15f, 0.08f);

        // ---------- interface ----------

        public void Click() => Ui(lib.sfxClick, 0.5f);
        public void Select() => Ui(lib.sfxSelect, 0.45f);
        public void OrderMove() => Ui(lib.sfxOrderMove, 0.45f);
        public void OrderAttack() => Ui(lib.sfxOrderAttack, 0.45f);

        void Ui(AudioClip clip, float volume)
        {
            if (clip == null) return;
            var v = NextVoice();
            v.panStereo = 0f; v.pitch = 1f;
            v.PlayOneShot(clip, volume);
        }

        // A battle sound at a world position.
        void At(AudioClip clip, Vector3 at, float volume, float minGap, float pitchJitter, float pitch = 1f)
        {
            if (clip == null || cam == null) return;
            float now = Time.unscaledTime;
            if (lastPlayed.TryGetValue(clip, out var last) && now - last < minGap) return;

            // Volume follows how close the camera is to the sound: a faint murmur at full zoom-out, full volume
            // when zoomed in on it. Sounds off to the side of the view fade out as well.
            float camDist = Vector3.Distance(cam.transform.position, at);
            float near = Mathf.InverseLerp(FarDistance, NearDistance, camDist); // 0 far away, 1 up close
            float gain = Mathf.Lerp(0.03f, 1f, near * near * near);
            var look = LookPoint();
            float reach = Mathf.Max(2500f, Vector3.Distance(cam.transform.position, look) * 1.2f);
            float side = Vector3.Distance(new Vector3(at.x, 0, at.z), look);
            gain *= Mathf.Clamp01(1f - side / reach);
            if (gain <= 0.01f) return;
            lastPlayed[clip] = now;

            var vp = cam.WorldToViewportPoint(at);
            var v = NextVoice();
            v.panStereo = Mathf.Clamp((vp.x - 0.5f) * 1.4f, -0.8f, 0.8f);
            v.pitch = pitch * (1f + Random.Range(-pitchJitter, pitchJitter));
            v.PlayOneShot(clip, volume * gain);
        }

        Vector3 LookPoint()
        {
            var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            return new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float d) ? ray.GetPoint(d) : cam.transform.position;
        }

        AudioSource NextVoice()
        {
            var v = voices[next];
            next = (next + 1) % Voices;
            return v;
        }
    }
}
