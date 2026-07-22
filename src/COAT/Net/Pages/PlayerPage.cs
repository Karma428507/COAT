namespace COAT.Pages;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class PlayerPage : Page
{
    public PlayerPage() : base(PageManager.PAGE_INDEX_PLAYER)
    {
        Log.Debug("Player page loaded");
    }
}
