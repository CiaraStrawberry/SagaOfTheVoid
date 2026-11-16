using System;
using System.Collections.Generic;
using UnityEngine;

namespace Saga.Game
{
    // Everything the battle view needs at runtime, in one asset (Assets/Resources/ShipLibrary.asset).
    // Built by ShipModelBuilder; referencing assets here also makes sure they ship in a build.
    [CreateAssetMenu(menuName = "Saga/Ship Library")]
    public class ShipLibrary : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string id;
            public GameObject prefab;
            public Sprite icon;
        }

        [Serializable]
        public class MapEntry
        {
            public string id;
            public GameObject prefab;
        }

        [Serializable]
        public class SkyEntry
        {
            public string id;          // as in campaign.json: Skybox1, skybox2 ... skybox5
            public Texture2D texture;
        }

        public List<Entry> ships = new List<Entry>();
        public List<MapEntry> maps = new List<MapEntry>();
        public List<SkyEntry> skies = new List<SkyEntry>();
        public TextAsset shipsJson;
        public TextAsset weaponsJson;
        public TextAsset campaignJson;
        public TextAsset narrativeJson;          // campaign story beats (tools/make_narrative.py)
        public List<PortraitEntry> portraits = new List<PortraitEntry>(); // comms portraits by character id
        public Shader fxShader;
        public Material skybox;
        public Material skyBackdrop; // flat sky behind the battle (SkyBackdrop), URP Unlit
        [Header("Warp-in (2017 Forge3D WarpJump textures)")]
        public Texture2D warpTunnel;
        public Texture2D warpRing;
        public Texture2D warpAura;
        public Texture2D warpSpark;
        [Header("Audio (2017 clips)")]
        public AudioClip musicMenu, musicBattle;
        public List<AudioClip> battlePlaylist = new List<AudioClip>(); // remake tracks (tools/music.py), crossfaded in turn
        public AudioClip sfxCannon, sfxMachinegun, sfxGatling, sfxMissile, sfxBeam, sfxShotgun, sfxRailgun;
        public AudioClip sfxWarp, sfxExplosion, sfxKick;
        public AudioClip sfxClick, sfxSelect, sfxOrderMove, sfxOrderAttack;

        [Serializable] public class PortraitEntry { public string id; public Sprite sprite; }
        // Unit voice replies, by set: <crew>_<kind>, e.g. pilot_move (tools/split_voice.py).
        [Serializable] public class VoiceEntry { public string set; public AudioClip clip; }
        public List<VoiceEntry> unitVoices = new List<VoiceEntry>();
        public List<AudioClip> VoiceLines(string set) => unitVoices.FindAll(v => v.set == set && v.clip != null).ConvertAll(v => v.clip);
        public Sprite Portrait(string id) => portraits.Find(p => p.id == id)?.sprite;

        public GameObject Map(string id) => maps.Find(m => m.id == id)?.prefab;
        public Texture2D Sky(string id) => skies.Find(s => string.Equals(s.id, id, StringComparison.OrdinalIgnoreCase))?.texture;

        public GameObject Prefab(string id)
        {
            foreach (var e in ships) if (e.id == id) return e.prefab;
            return null;
        }

        public Sprite Icon(string id)
        {
            foreach (var e in ships) if (e.id == id) return e.icon;
            return null;
        }

        public static ShipLibrary Load() => Resources.Load<ShipLibrary>("ShipLibrary");

        public Saga.Sim.Rulebook Rules() => new Saga.Sim.Rulebook(
            JsonUtility.FromJson<Saga.Sim.ShipDefList>(shipsJson.text),
            JsonUtility.FromJson<Saga.Sim.WeaponDefList>(weaponsJson.text));
    }
}
