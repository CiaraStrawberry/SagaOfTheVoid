using System.Linq;
using Saga.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace Saga.Game
{
    // The start screen: skirmish options and the campaign. Choosing something sets
    // SkirmishController.Pending and reloads the scene into that battle.
    public class StartMenu
    {
        public const string ProgressKey = "saga.campaign.completed"; // index of the highest mission won, -1 for none

        readonly SkirmishController game;
        readonly CampaignDef campaign;
        readonly ShipLibrary lib;
        readonly Rulebook rules;
        readonly Text skirmishMap, skirmishDifficulty, briefing;
        readonly GameObject launch;
        readonly Text launchLabel;
        int chosenMission = -1;
        int mapIndex;
        Difficulty difficulty = Difficulty.Normal;
        static readonly string[] MapIds = { "asteroid_field", "space_station", "ship_graveyard" };
        static readonly string[] MapSkies = { "skybox5", "Skybox1", "skybox4" };

        public static int Completed => PlayerPrefs.GetInt(ProgressKey, -1);
        public static bool Unlocked(int mission) => mission <= Completed + 2; // 2017: up to two beyond the last win

        public StartMenu(SkirmishController game, ShipLibrary lib, Camera cam, CampaignDef campaign)
        {
            this.game = game; this.lib = lib; this.campaign = campaign;
            rules = lib.Rules();
            var root = UiKit.Canvas("Start menu", cam, 10).transform;
            UiKit.Label(root, "title", 64, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(80, -70), new Vector2(1200, 90), "<b>SAGA OF THE VOID</b>");
            UiKit.Label(root, "sub", 22, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(84, -150), new Vector2(1200, 40),
                "<color=#8d98ab>Remake preview · Unity 6 · F10 quits</color>");

            // Skirmish column
            var sk = UiKit.Box(root, "skirmish", new Vector2(0, 1), new Vector2(80, -220), new Vector2(520, 520), UiKit.Panel).transform;
            UiKit.Label(sk, "h", 30, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(24, -20), new Vector2(480, 40), "<b>Skirmish</b>");
            UiKit.Label(sk, "p", 20, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(24, -64), new Vector2(470, 60),
                "<color=#8d98ab>You command Blue against the computer. Both sides earn credits and buy ships.</color>");
            skirmishMap = UiKit.Button(sk, "", new Vector2(0, 1), new Vector2(24, -140), new Vector2(472, 56), () => { mapIndex = (mapIndex + 1) % MapIds.Length; Refresh(); }, 22);
            skirmishDifficulty = UiKit.Button(sk, "", new Vector2(0, 1), new Vector2(24, -206), new Vector2(472, 56), () => { difficulty = (Difficulty)(((int)difficulty + 1) % 3); Refresh(); }, 22);
            UiKit.Button(sk, "Full battle  ·  18 v 18", new Vector2(0, 1), new Vector2(24, -300), new Vector2(472, 64), () => StartSkirmish(false), 26);
            UiKit.Button(sk, "Quick battle  ·  8 v 8", new Vector2(0, 1), new Vector2(24, -376), new Vector2(472, 64), () => StartSkirmish(true), 26);

            // Campaign column
            var cp = UiKit.Box(root, "campaign", new Vector2(0, 1), new Vector2(640, -220), new Vector2(1200, 640), UiKit.Panel).transform;
            UiKit.Label(cp, "h", 30, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(24, -20), new Vector2(600, 40), "<b>Campaign</b>");
            for (int i = 0; i < campaign.missions.Count; i++)
            {
                int idx = i;
                var m = campaign.missions[i];
                bool open = Unlocked(i);
                string mark = i <= Completed ? "  <color=#58c98f>✓</color>" : "";
                var label = UiKit.Button(cp, "", new Vector2(0, 1), new Vector2(24, -74 - i * 66), new Vector2(420, 56), () => Choose(idx), 22);
                label.text = open ? $"{i + 1}.  {m.name}{mark}" : $"<color=#5a6170>{i + 1}.  {m.name}   (locked)</color>";
                label.alignment = TextAnchor.MiddleLeft;
                label.horizontalOverflow = HorizontalWrapMode.Overflow;
                var lr = label.rectTransform; // fill the button, with left padding
                lr.anchorMin = Vector2.zero; lr.anchorMax = Vector2.one; lr.pivot = new Vector2(0.5f, 0.5f);
                lr.offsetMin = new Vector2(20, 0); lr.offsetMax = new Vector2(-10, 0);
                label.transform.parent.GetComponent<Button>().interactable = open;
            }
            briefing = UiKit.Label(cp, "briefing", 22, TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(476, -74), new Vector2(690, 560),
                "<color=#8d98ab>Choose a mission.</color>");
            launchLabel = UiKit.Button(cp, "Launch mission", new Vector2(1, 0), new Vector2(-24, 24), new Vector2(300, 64), Launch, 26);
            launch = launchLabel.transform.parent.gameObject;
            launch.SetActive(false);
            Refresh();
        }

        void Refresh()
        {
            skirmishMap.text = "Map:  " + Pretty(MapIds[mapIndex]) + "   <color=#8d98ab>(click to change)</color>";
            skirmishDifficulty.text = "Computer:  " + difficulty + "   <color=#8d98ab>(click to change)</color>";
        }

        void Choose(int i)
        {
            chosenMission = i;
            var m = campaign.missions[i];
            var names = m.enemy_fleet.GroupBy(x => x).Select(g => (g.Count() > 1 ? g.Count() + " × " : "") + ShipName(g.Key));
            string goal = m.objective == "survive" ? $"Survive for {m.time_limit / 60} minutes" : m.objective == "kill_target" ? "Destroy the enemy flagship" : $"Destroy every enemy ship within {m.time_limit / 60} minutes";
            briefing.text =
                $"<size=30><b>{i + 1}. {m.name}</b></size>\n\n" +
                $"<i>{m.story}</i>\n\n" +
                $"<color=#a8d4ff>Objective</color>   {goal}\n" +
                $"<color=#a8d4ff>Enemy</color>        {string.Join(", ", names)}\n" +
                $"<color=#a8d4ff>Map</color>            {Pretty(m.map)}\n" +
                $"<color=#a8d4ff>Credits</color>       {m.start_money:N0}{(m.objective == "survive" ? " + income" : ", no income")}\n" +
                $"<color=#a8d4ff>You can build</color>  {Pretty(m.ships_allowed).Replace(" And ", " and ")}";
            launch.SetActive(true);
        }

        string ShipName(string id) => rules.Ships.TryGetValue(id, out var d) ? d.name : Pretty(id);

        void StartSkirmish(bool quick) => SkirmishController.Begin(new BattleConfig
        {
            Quick = quick, Map = MapIds[mapIndex], Sky = MapSkies[mapIndex], Difficulty = difficulty,
        });

        void Launch()
        {
            if (chosenMission >= 0) SkirmishController.Begin(new BattleConfig { Mission = chosenMission });
        }

        static string Pretty(string id) => string.Join(" ", id.Split('_').Select(w => char.ToUpper(w[0]) + w.Substring(1)));
    }
}
