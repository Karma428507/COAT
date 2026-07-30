namespace COAT.Pages;

using COAT.Content;

using Steamworks;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class PlayerPage : Page
{
    public PlayerPage() : base(PageManager.PAGE_INDEX_PLAYER)
    {
        // Make it get the team automatically
        AddProperty("team", Team.Yellow);
        AddProperty("team color", TeamExtensions.Color(Team.Yellow));
        AddProperty("username", SteamClient.Name);

        Log.Debug("Player page loaded");
    }
}
