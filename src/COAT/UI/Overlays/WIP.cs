namespace COAT.UI.Overlays;

using COAT.Net;
using System;
using System.Collections.Generic;
using System.Text;

public class WIP : CanvasSingleton<WIP>
{
    private void Start()
    {
        Events.OnLoaded += Toggle;

        // make UI element here
    }

    private void Update()
    {

    }
    public void Toggle() => gameObject.SetActive(Shown = LobbyController.Online);
}
