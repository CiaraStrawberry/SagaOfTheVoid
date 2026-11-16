using System;
using System.Collections.Generic;
using Saga.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace Saga.Game
{
    // Campaign transmissions: the battle pauses and dims, a character portrait plate appears bottom left with the
    // speaker's name and channel, and their lines type out beside it. Click, Space or Enter: finish the line, then
    // the next one. Esc skips the rest of the transmission (never a decision). The opening briefing also shows a
    // title card (act, mission, premise, objective). After the last mission: the ending choice, then its epilogue.
    // Portraits (tools/portraits.py) have transparent backgrounds and are shown with nothing behind them.
    public class CommsPanel
    {
        const float TypeSpeed = 70f;            // characters per second
        static readonly Color Blue = new Color(0.45f, 0.72f, 1f), Gold = new Color(1f, 0.78f, 0.35f), Red = new Color(1f, 0.38f, 0.3f);
        static readonly Color Unknown = new Color(0.75f, 0.45f, 0.45f);

        abstract class Step { }
        class LineStep : Step { public StoryLine Line; public StoryMission Title; }
        class ChoiceStep : Step { public List<StoryEnding> Options; public Action<StoryEnding> Picked; }
        class CardStep : Step { public string Title, Text; }

        readonly ShipLibrary lib;
        readonly StoryDef story;
        readonly int missionIndex;
        readonly Queue<Step> queue = new Queue<Step>();
        readonly GameObject root;
        readonly RectTransform plate, titleCard, textBox;
        readonly Image portrait;
        readonly Text name, channel, body, hint, act, missionName, premise, cardTitle, cardText;
        readonly RectTransform choices;
        Step current;
        string currentSpeaker;
        float shown, portraitAge, flicker;
        public bool Open => current != null;

        public CommsPanel(ShipLibrary lib, Camera cam, StoryDef story, int missionIndex)
        {
            this.lib = lib; this.story = story; this.missionIndex = missionIndex;
            var canvas = UiKit.Canvas("Comms", cam, 30);
            root = canvas.gameObject;
            var r = canvas.transform;

            // Dim the battlefield and block clicks to it.
            var dim = UiKit.Box(r, "dim", new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0f, 0.01f, 0.03f, 0.62f));
            Stretch(dim.rectTransform);
            dim.raycastTarget = true;

            // Title card (opening briefing only).
            titleCard = UiKit.Rect(r, "title", new Vector2(0.5f, 1f), new Vector2(0, -90), new Vector2(1300, 330));
            act = UiKit.Label(titleCard, "act", 22, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), Vector2.zero, new Vector2(1300, 34));
            act.color = UiKit.Muted;
            missionName = UiKit.Label(titleCard, "name", 64, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0, -36), new Vector2(1300, 84));
            missionName.fontStyle = FontStyle.Bold;
            premise = UiKit.Label(titleCard, "premise", 26, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0, -132), new Vector2(1100, 190));

            // The portrait itself, with no box behind it: the transparent cut-out stands on the bottom edge of the
            // screen over the dimmed battlefield (owner request: blend in with the display).
            plate = UiKit.Rect(r, "plate", new Vector2(0f, 0f), new Vector2(30, 0), new Vector2(520, 780));
            portrait = new GameObject("portrait", typeof(RectTransform)).AddComponent<Image>();
            portrait.transform.SetParent(plate, false);
            var pr = portrait.rectTransform;
            pr.anchorMin = new Vector2(0.5f, 0f); pr.anchorMax = new Vector2(0.5f, 0f); pr.pivot = new Vector2(0.5f, 0f);
            pr.sizeDelta = new Vector2(520, 780);
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;

            // Text box beside the plate.
            textBox = UiKit.Box(r, "text", new Vector2(0f, 0f), new Vector2(560, 60), new Vector2(1180, 270), UiKit.Panel).rectTransform;
            name = UiKit.Label(textBox, "name", 34, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(32, -22), new Vector2(800, 44));
            name.fontStyle = FontStyle.Bold;
            channel = UiKit.Label(textBox, "channel", 18, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(-32, -32), new Vector2(320, 30));
            channel.color = UiKit.Muted;
            body = UiKit.Label(textBox, "body", 30, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(32, -78), new Vector2(1116, 150));
            hint = UiKit.Label(textBox, "hint", 18, TextAnchor.LowerRight, new Vector2(1f, 0f), new Vector2(-24, 14), new Vector2(700, 26),
                "Click or Space to continue  ·  Escape to skip");
            hint.color = UiKit.Muted;
            choices = UiKit.Rect(r, "choices", new Vector2(0f, 0f), new Vector2(560, 350), new Vector2(1180, 160));

            // Epilogue card (no portrait).
            cardTitle = UiKit.Label(r, "card title", 54, TextAnchor.UpperCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 230), new Vector2(1300, 80));
            cardTitle.fontStyle = FontStyle.Bold;
            cardText = UiKit.Label(r, "card text", 30, TextAnchor.UpperCenter, new Vector2(0.5f, 0.5f), new Vector2(0, 130), new Vector2(1150, 420));

            root.SetActive(false);
        }

        static void Stretch(RectTransform t)
        {
            t.anchorMin = Vector2.zero; t.anchorMax = Vector2.one; t.pivot = new Vector2(0.5f, 0.5f);
            t.offsetMin = Vector2.zero; t.offsetMax = Vector2.zero;
        }

        // Queues a beat's lines; title shows the mission's title card with them (the opening briefing).
        public void Play(StoryBeat beat, StoryMission title = null)
        {
            bool first = true;
            foreach (var l in beat.lines)
            {
                queue.Enqueue(new LineStep { Line = l, Title = first ? title : null });
                first = false;
            }
            if (current == null) Advance();
        }

        public void Choose(List<StoryEnding> options, Action<StoryEnding> picked)
        {
            queue.Enqueue(new ChoiceStep { Options = options, Picked = picked });
            if (current == null) Advance();
        }

        public void Card(string title, string text)
        {
            queue.Enqueue(new CardStep { Title = title, Text = text });
            if (current == null) Advance();
        }

        // Closes the panel and drops everything queued (tests).
        public void Close()
        {
            queue.Clear();
            current = null;
            root.SetActive(false);
        }

        // Shows the current line in full (tests and screenshots).
        public void FinishLine() { if (current is LineStep l) { shown = l.Line.text.Length; body.text = l.Line.text; portraitAge = 1f; AnimatePortrait(0f); } }

        // Called every frame by the controller; handles the keys and typing while open.
        public void Update(float dt)
        {
            if (current == null) return;
            portraitAge += dt;
            if (current is LineStep ls)
            {
                shown += dt * TypeSpeed;
                body.text = ls.Line.text.Substring(0, Mathf.Min(ls.Line.text.Length, (int)shown));
                AnimatePortrait(dt);
            }
            if (current is ChoiceStep) return; // only the buttons move on from a decision
            bool next = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Skip the rest of the talking, but stop at a decision or an epilogue.
                while (queue.Count > 0 && queue.Peek() is LineStep) queue.Dequeue();
                Advance();
                return;
            }
            if (!next) return;
            if (current is LineStep l && shown < l.Line.text.Length) { shown = l.Line.text.Length; return; }
            Advance();
        }

        void Advance()
        {
            current = queue.Count > 0 ? queue.Dequeue() : null;
            root.SetActive(current != null);
            if (current == null) { currentSpeaker = null; return; }
            bool line = current is LineStep, choice = current is ChoiceStep, card = current is CardStep;
            plate.gameObject.SetActive(line || choice);
            textBox.gameObject.SetActive(line || choice);
            choices.gameObject.SetActive(choice);
            cardTitle.gameObject.SetActive(card); cardText.gameObject.SetActive(card);
            titleCard.gameObject.SetActive(line && ((LineStep)current).Title != null);
            hint.gameObject.SetActive(!choice);

            if (current is LineStep ls)
            {
                shown = 0f;
                ShowSpeaker(ls.Line.speaker);
                if (ls.Title != null)
                {
                    act.text = ls.Title.act;
                    missionName.text = (ls.Title.index + 1) + ".  " + ls.Title.name;
                    premise.text = ls.Title.premise + "\n\n<color=#a8d4ff>Objective</color>   " + ls.Title.primary;
                }
                SoundBoard.Current?.Select();
            }
            else if (current is ChoiceStep cs)
            {
                body.text = "<color=#a8b3c4>Your decision.</color>";
                foreach (Transform c in choices) UnityEngine.Object.Destroy(c.gameObject);
                for (int i = 0; i < cs.Options.Count; i++)
                {
                    var opt = cs.Options[i];
                    UiKit.Button(choices, opt.choice, new Vector2(0f, 0f), new Vector2(i * 600, 0), new Vector2(570, 72), () =>
                    {
                        cs.Picked?.Invoke(opt);
                        Advance();
                    }, 24);
                }
            }
            else if (current is CardStep cd)
            {
                cardTitle.text = cd.Title;
                cardText.text = cd.Text;
            }
        }

        void ShowSpeaker(string id)
        {
            var c = story.Character(id);
            bool hidden = StoryDirector.Hidden(id, missionIndex);
            var colour = hidden ? Unknown : c == null ? UiKit.Accent : c.color == "gold" ? Gold : c.color == "red" ? Red : Blue;
            name.text = hidden ? "Unidentified signal" : c != null ? c.name : id;
            name.color = colour;
            channel.text = hidden ? "UNKNOWN RELAY" : c != null ? c.faction : "";
            name.text += hidden || c == null ? "" : "   <size=20><color=#8d98ab>" + c.role + "</color></size>";
            if (id != currentSpeaker) { portraitAge = 0f; currentSpeaker = id; }
            portrait.sprite = lib.Portrait(id);
            portrait.enabled = portrait.sprite != null;
            flicker = hidden ? 1f : 0f;
        }

        // A new speaker slides in and fades up; a hidden one stays a dark, flickering silhouette.
        void AnimatePortrait(float dt)
        {
            float k = Mathf.Clamp01(portraitAge / 0.25f);
            float ease = 1f - (1f - k) * (1f - k);
            portrait.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-36f, 0f, ease), 0f);
            if (flicker > 0f)
            {
                // A black silhouette only: no face until she names herself.
                float f = 0.8f + 0.2f * Mathf.PerlinNoise(Time.unscaledTime * 9f, 0.3f);
                portrait.color = new Color(0.03f, 0f, 0.01f, ease * f);
            }
            else portrait.color = new Color(1f, 1f, 1f, ease);
        }
    }
}
