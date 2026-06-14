namespace COAT.UI.Overlay;

using COAT.Content;
using COAT.Net;
using COAT.Net.Types;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Utils.Pal;
using static Utils.Rect;

/// <summary> Teammates information displayed in the bottom right corner of the screen. </summary>
public class PlayerInfo : CanvasSingleton<PlayerInfo>
{
    /// <summary> Object containing information about players. </summary>
    private Image root;

    private void Start()
    {
        Events.OnLobbyEntered += () => { if (!Shown) Toggle(); };
        Events.OnTeamChanged += Rebuild;
    }

    private void Update()
    {
        if (root) root.color = new(0f, 0f, 0f, PrefsManager.Instance.GetFloat("hudBackgroundOpacity") / 100f);
    }

    private void UpdateMaterials()
    {
        //foreach (var img in root.GetComponentsInChildren<Image>()) img.material = HUDOptions.Instance.hudMaterial;
        //foreach (var txt in root.GetComponentsInChildren<Text>()) txt.material = HUDOptions.Instance.hudMaterial;
    }

    /// <summary> Toggles visibility of the information table. </summary>
    public void Toggle()
    {
        Shown = !Shown;
        Rebuild();
    }

    /// <summary> Rebuilds the information table to match a new state. </summary>
    public void Rebuild()
    {
        if (root) Destroy(root.gameObject); // for some reason the operator ? doesn't work here
        if (!Shown || !StyleHUD.Instance) return;

        List<RemotePlayer> teammates = new();
        Networking.EachPlayer(player =>
        {
            // the player should only see information about teammates
            if (player.Team.Ally()) teammates.Add(player);
        });

        float height = teammates.Count == 0 ? 40f : teammates.Count * 48f + 8f;

        root = UIB.Table("Player Info", StyleHUD.Instance.transform, Size(540f, height));
        root.transform.localPosition = new(-75f, -556f + height / 2f, 0f);

        if (teammates.Count == 0)
            UIB.Text("#player-info.alone", root.transform, Size(540f, 40f));
        else
        {
            float y = -20f;
            teammates.ForEach(p => PlayerInfoEntry.Build(p, UIB.Rect(p.Header.Name, root.transform, Btn(y += 48f) with { Width = 540f })));
        }

        Events.Post2(UpdateMaterials);
    }
}

/// <summary> Interface element displaying information about the player such as name, health and railgun charge. </summary>
public class PlayerInfoEntry : MonoBehaviour
{
    /// <summary> Player whose information this entry displays. </summary>
    private RemotePlayer player;

    /// <summary> Component containing the name and rail charge. </summary>
    private Text pname, railc;
    /// <summary> Health images that the player directly sees. </summary>
    private RectTransform health, overhealth;

    /// <summary> Creates an entry with the given parent. </summary>
    public static PlayerInfoEntry Build(RemotePlayer player, Transform parent) =>
        UIB.Component<PlayerInfoEntry>(parent.gameObject, entry => entry.player = player);

    private void Start()
    {
        var t = Size(540f - 16f, 40f);
        pname = UIB.Text($"<b>{player.Header.Name}</b>", transform, t, size: 32, align: TextAnchor.UpperLeft);
        railc = UIB.Text("", transform, t, size: 32, align: TextAnchor.UpperRight);

        var h = Size(540f - 16f, 8f) with { y = -16f };
        UIB.Image("Background", transform, h, black);

        health = UIB.Image("Health", transform, h, red).rectTransform;
        overhealth = UIB.Image("Overhealth", transform, h, green).rectTransform;
    }

    private void Update()
    {
        float hp = player.Health;
        int charge = Mathf.Min(10, player.RailCharge);

        pname.color = hp > 0f ? white : red;
        railc.text = $"<b><color=#0080FF>ϟ</color></b>[<color=#0080FF>{new string('I', charge)}</color><color=#003060>{new string('-', 10 - charge)}</color>]";

        health.localScale = new(Mathf.Min(hp / 100f, 1f), 1f, 1f);
        health.localPosition = new(-(1f - health.localScale.x) * 262f, -16f, 0f);

        overhealth.localScale = new(Mathf.Max((hp - 100f) / 100f, 0f), 1f, 1f);
        overhealth.localPosition = new(-(1f - overhealth.localScale.x) * 262f, -16f, 0f);
    }
}
