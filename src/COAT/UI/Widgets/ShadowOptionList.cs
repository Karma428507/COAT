namespace COAT.UI.Widgets;

using COAT.Assets;
using COAT.Content;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Menus;
using COAT.World;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using static Pal;
using static Rect;

public class ShadowOptionList : MonoBehaviour
{
    // Given data
    private string name;
    private Dictionary<string, Action> buttons = new Dictionary<string, Action>();

    // UI stuff
    private RectTransform content;

    public static ShadowOptionList Build(Transform parent, string name, Dictionary<string, Action> buttons) =>
        UIB.Component<ShadowOptionList>(parent.gameObject, entry => {
            entry.buttons = buttons; entry.name = name;
        });

    private void Start()
    {
        UIB.Shadow(transform);

        UIB.Table("General", name, transform, Tlw(550, 1050), table =>
        {
            // add a seperator then the option buttons later
            content = UIB.Scroll("Button List", table, new COAT.UI.Rect(0, -25, 320f, 970)).content;
        });

        Rebuild();
    }

    private void Update()
    {
    }

    public void Rebuild()
    {
        float height = (buttons.Count * 88) + 50;
        float y = 40;

        content.sizeDelta = new(336f, height + 54f);

        foreach (var button in buttons)
        {
            UIB.Table("OptionList", content, new(0, y -= 88, 320f, 80f, new(.5f, 1f)), player =>
            {
                UIB.Button("Player", button.Key, player, new(0, 0, 320f, 80f), Color.white, 24, clicked: button.Value);
            });
        }
    }
}
