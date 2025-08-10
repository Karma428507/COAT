namespace COAT.UI.Widgets;

using COAT.Net;
using COAT.Net.Types;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;

using static Pal;
using static Rect;

public class ShadowOptionList : MonoBehaviour
{
    public int player = 0;

    public static ShadowOptionList Build(Transform parent) =>
        UIB.Component<ShadowOptionList>(parent.gameObject, entry => entry.player = 0);

    private void Start()
    {
        
    }

    private void Update()
    {
    }
}
