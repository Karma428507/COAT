namespace COAT.Chat;

/// <summary> The container to organize different type of commands </summary>
public interface ICommandModule
{
    /// <summary> Determines if the module should be loaded </summary>
    public bool Condition();

    /// <summary> Registers the commands on startup </summary>
    public void Load();
}
