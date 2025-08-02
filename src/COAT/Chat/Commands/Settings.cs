namespace COAT.Chat.Commands;

using COAT.Chat;
using COAT.UI.Overlays;
using UnityEngine;

/// <summary> Commands to change chat settings </summary>
public class Settings : ICommandModule
{
    static ChatUI chat => ChatUI.Instance;

    // Always true
    public bool Condition() => true;

    public void Load()
    {
        ChatHandler.Register("tts-volume", "\\[0-100]", "Set Sam's volume to keep your ears comfortable", args =>
        {
            if (args.Length == 0)
                chat.Receive($"[#FFA500]TTS volume is {UI.Menus.Settings.TTSVolume}.");
            else if (int.TryParse(args[0], out int value))
            {
                int clamped = Mathf.Clamp(value, 0, 100);
                UI.Menus.Settings.TTSVolume = clamped;

                chat.Receive($"[#32CD32]TTS volume is set to {clamped}.");
            }
            else
                chat.Receive("[#FF341C]Failed to parse value. It must be an integer in the range from 0 to 100.");
        });

        ChatHandler.Register("tts-auto", "\\[on/off]", "Turn auto reading of all messages", args =>
        {
            bool enable = args.Length == 0 ? !chat.AutoTTS : (args[0] == "on" || (args[0] == "off" ? false : !chat.AutoTTS));
            if (enable)
            {
                UI.Menus.Settings.AutoTTS = chat.AutoTTS = true;
                chat.Receive("[#32CD32]Auto TTS enabled.");
            }
            else
            {
                UI.Menus.Settings.AutoTTS = chat.AutoTTS = false;
                chat.Receive("[#FF341C]Auto TTS disabled.");
            }
        });
    }
}
