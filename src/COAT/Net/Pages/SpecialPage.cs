namespace COAT.Pages;

/// <summary> Handles different "pages" or long term net data like world and player info. </summary>
public class SpecialPage : Page
{
    public virtual bool Condition { get; set; }

    public SpecialPage()
    {
        Initialize();
    }


    public virtual void Load()
    {

    }
}
