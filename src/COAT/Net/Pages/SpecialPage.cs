namespace COAT.Net.Pages;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class SpecialPage : Page
{
    public virtual bool Condition { get; set; }

    public SpecialPage() : base(PageManager.PAGE_INDEX_SPECIAL)
    {
    }


    public virtual void Load()
    {

    }
}
