namespace COAT.UI.Screen;

using System.Collections;
using System.Collections.Generic;

using Steamworks;
using UnityEngine;
using UnityEngine.UI;

using COAT.Assets;
using COAT.Chat;
using COAT.Input;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI.Menus;
using Sam;

using static Elements.Pal;
using static Elements.Rect;

/// <summary> Front end of the chat, back end implemented via Steamworks. </summary>
public class ChatUI : CanvasSingleton<ChatUI>, IOverlayInterface
{
    /// <summary> Maximum length of chat message. </summary>
    public const int MAX_MESSAGE_LENGTH = 128;
    /// <summary> How many messages at a time will be shown. </summary>
    public const int MESSAGES_SHOWN = 14;
    /// <summary> Chat width in pixels. </summary>
    public const float WIDTH = 640f;

    /// <summary> List of the chat messages. </summary>
    private RectTransform list;
    /// <summary> Canvas group used to change the chat transparency. </summary>
    private CanvasGroup listBg;

    /// <summary> List of the players currently typing. </summary>
    private Text typing;
    /// <summary> Background of the typing players list. </summary>
    private RectTransform typingBg;

    /// <summary> Whether auto TTS is enabled. </summary>
    public bool AutoTTS;
    /// <summary> Background of the auto TTS sign. </summary>
    private RectTransform ttsBg;

    /// <summary> Input field in which the message will be entered directly. </summary>
    public InputField Field;
    /// <summary> Arrival time of the last message, used to change the chat transparency. </summary>
    private float lastMessageTime;

    /// <summary> Messages sent by the player. </summary>
    private List<string> messages = new();
    /// <summary> Index of the current message in the list. </summary>
    private int messageIndex;

    private void Start()
    {
        Events.OnLobbyEntered += () => ChatUtils.Hello(); // send some useful information to the chat so that players know about the mod's features
        AutoTTS = Settings.AutoTTS;

        list = UIB.Table("List", transform, Blh(WIDTH)).rectTransform;
        listBg = UIB.Component<CanvasGroup>(list.gameObject, group => group.blocksRaycasts = false); // disable the chat collision so it doesn't interfere with other buttons

        typingBg = UIB.Table("Typing", transform, Blh(0f)).rectTransform;
        typing = UIB.Text("", typingBg, Blh(4200f).Text);

        ttsBg = UIB.Table("TTS", transform, Blh(128f)).rectTransform;
        UIB.Text("#chat.tts", ttsBg, Blh(128f).Text);

        Field = UIB.Field("#chat.info", transform, Msg(1888f) with { y = 32f }, cons: OnFocusLost);
        Field.characterLimit = MAX_MESSAGE_LENGTH;
        Field.gameObject.SetActive(false);

        // start the update cycle of typing players
        InvokeRepeating("UpdateTyping", 0f, .5f);
    }

    private void Update()
    {
        listBg.alpha = Mathf.Lerp(listBg.alpha, Shown || Time.time - lastMessageTime < 5f ? 1f : 0f, Time.deltaTime * 5f);
        ttsBg.gameObject.SetActive(AutoTTS && Shown);
    }

    private void UpdateTyping()
    {
        // get a list of players typing in the chat
        List<string> list = new();

        if (Shown) list.Add(Bundle.Get("chat.you"));
        Networking.EachPlayer(player =>
        {
            if (player.Typing) list.Add(player.Header.Name);
        });

        // hide the typing label if there is no one in the chat
        typingBg.gameObject.SetActive(list.Count > 0);

        if (list.Count != 0)
        {
            if (list.Count == 1 && Shown)
                typing.text = Bundle.Get("chat.only-you");
            else
            {
                typing.text = string.Join(", ", list.ToArray(), 0, Mathf.Min(list.Count, 3));
                if (list.Count > 3) typing.text += Bundle.Get("chat.other");

                typing.text += Bundle.Get(list.Count == 1 ? "chat.single" : "chat.multiple");
            }

            float width = typing.preferredWidth + 16f;

            typingBg.sizeDelta = new(width, 32f);
            typingBg.anchoredPosition = new(16f + width / 2f, 80f);
        }

        ttsBg.anchoredPosition = new(list.Count > 0 ? typingBg.anchoredPosition.x + typingBg.sizeDelta.x / 2f + 80f : 80f, 80f);
    }

    private void OnFocusLost(string msg) // how does this work, cuz when its used it doesnt do OnFocusLost("bjewhr") it just does OnFocusLost and nothing else
    {
        // focus lost because the player entered a message
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) Send(msg);

        // focus lost for some other reason
        else
        {
            Field.gameObject.SetActive(Shown = false);
            Movement.UpdateState();
        }
    }

    /// <summary> Sends a message to all other players. </summary>
    public void Send(string msg)
    {
        msg = msg.Trim(); // remove extra spaces from the message before formatting

        // if the message is not empty, then send it to other players and remember it
        if (ChatParser.Parse(msg).Trim() != "")
        {
            if (!ChatParser.IsCommand(msg)) LobbyController.Lobby?.SendChatString(AutoTTS ? "/tts " + msg : msg);
            messages.Insert(0, msg);
        }

        Field.text = "";
        messageIndex = -1;
        Events.Post(Toggle);
    }

    #region Interface Stuff

    /// <summary> Toggles visibility of the chat. </summary>
    public void Toggle()
    {
        Field.gameObject.SetActive(Shown = !Shown && LobbyController.Online);
        Movement.UpdateState();
        UpdateTyping();

        if (Shown) Field.ActivateInputField();
    }

    public bool Condition()
    {
        return true;
    }

    #endregion
    #region scroll

    /// <summary> Scrolls messages through the list of messages sent by the player. </summary>
    public void ScrollMessages(bool up)
    {
        // to scroll through messages, the chat must be open and the list must have at least one element
        if (messages.Count == 0 || !Shown) return;

        // limiting the message index
        if (up ? messageIndex == messages.Count - 1 : messageIndex == -1) return;

        // update the message id and text in the input field
        messageIndex += up ? 1 : -1;
        Field.text = messageIndex == -1 ? "" : messages[messageIndex];
        Field.caretPosition = Field.text.Length;

        // run message highlight
        StopCoroutine("MessageScrolled");
        StartCoroutine("MessageScrolled");
    }

    /// <summary> Interpolates the color of the input field from green to white. </summary>
    private IEnumerator MessageScrolled()
    {
        float start = Time.time;
        while (Time.time - start < .4f)
        {
            Field.textComponent.color = Color.Lerp(green, white, (Time.time - start) * 2.5f);
            yield return null;
        }
    }

    #endregion
    #region receive

    /// <summary> Writes a message directly to the chat. </summary>
    public void Receive(string msg, bool format = true)
    {
        // add the given message to the list
        if (format) msg = Bundle.ParseColors(msg);
        var text = UIB.Text(msg, list, Msg(WIDTH - 16f), null, 16, TextAnchor.MiddleLeft);
        //Text text = null;
        //text = UIB.ButtonText(msg, list, Msg(WIDTH - 16f), null, 16, TextAnchor.MiddleLeft, () => CopyText(msg.Substring(msg.IndexOf("[][#FF7F50]:[]</b> ")), text), () => DropUpMenu(msg.Substring(msg.IndexOf("[][#FF7F50]:[]</b> ")), msg.Substring(msg.IndexOf(']'), msg.IndexOf("[][#FF7F50]:[]</b> ")), text));


        float height = text.preferredHeight + 4f;
        text.rectTransform.sizeDelta = new(WIDTH - 16f, height);
        text.rectTransform.anchoredPosition = new(0f, 8f - height / 2f);

        foreach (RectTransform child in list) child.anchoredPosition += new Vector2(0f, height);
        if (list.childCount > MESSAGES_SHOWN) DestroyImmediate(list.GetChild(0).gameObject);

        // scale the chat panel
        var top = list.GetChild(0) as RectTransform;
        list.sizeDelta = new(WIDTH, top.anchoredPosition.y + top.sizeDelta.y / 2f + 8f);
        list.anchoredPosition = new(16f + WIDTH / 2f, 112f + list.sizeDelta.y / 2f);

        // save the time the message was received to give the player time to read it
        lastMessageTime = Time.time;
    }

    /*public void CopyText(string TextToCopy, Text text)
    {
        //TextEditor textEditor = new TextEditor();
        //textEditor.text = TextToCopy;
        //textEditor.SelectAll();
        //textEditor.Copy();

        GUIUtility.systemCopyBuffer = TextToCopy; YOU DONT SEE ANYTHING I SWEARRR D:
        MessageColChange(text);
    }*/

    //public void DropUpMenu(string text, string author, Text textobject) => CopyText($"haha", textobject);

    /// <summary> Interpolates the color of the input field from green to white. </summary>
    private IEnumerator MessageColChange(Text text)
    {
        float start = Time.time;
        while (Time.time - start < .4f)
        {
            text.color = Color.Lerp(darkgrey, white, (Time.time - start) * 2.5f);
            yield return null;
        }
    }

    /// <summary> Writes a message to the chat, formatting it beforehand. </summary>
    public void Receive(string color, string author, string msg) => Receive($"<b>[{(color.StartsWith('#') ? color : $"#{color}")}]{author}[][#FF7F50]:[]</b> {Moderation.ParseMessage(Bundle.CutDangerous(msg))}");


    public void NewReceive(string color, Friend author, string msg, bool tts = false)
    {
        string FormattedColor = (color.StartsWith('#') ? color : $"#{color}");
        string FormattedName = author.Name.Replace("[", "\\[");
        string FormattedMsg = Moderation.ParseMessage(Bundle.CutDangerous(msg));

        string FormattedPrefixes = tts ? ChatUtils.TTS_PREFIX : "";
        FormattedPrefixes += author.Id == LobbyController.LastOwner ? ChatUtils.HOST_PREFIX : "";
        FormattedPrefixes += Networking.COATPLAYERS.Contains(author.Id.AccountId) ? ChatUtils.COAT_PREFIX : "";

        Receive($"<b>{FormattedPrefixes}[{FormattedColor}]{FormattedName}[][#F75]:[]</b> {FormattedMsg}");
    }

    /// <summary> Writes a message to the chat, formatting it beforehand. </summary>
    /*public void Receive(string color, Friend author, string msg, bool TTS)
    {
        string username = TTS ? TTS_PREFIX : "";

        if (Networking.Entities.TryGetValue(author.Id.AccountId, out var entity) && entity && entity is RemotePlayer player)
            username += player.GetUsername(author);
        else
            username += author.Name.Replace("[", "\\[");

        Receive($"<b>[{(color.StartsWith('#') ? color : $"#{color}")}]{username}[][#FF7F50]:[]</b> {Bundle.CutDangerous(msg)}");
    }*/

    static ChatUI chat => ChatUI.Instance;

    /// <summary> Writes a message to the chat, formatting it beforehand. While being static, this is used for telling the user something. </summary>
    public static void StaticReceive(string color, string author, string msg) => chat.Receive($"<b>[#{color}]{author}[][#FF7F50]:[]</b> {msg}");

    /// <summary> Writes a message to the chat. While being static, this is used for telling the user something. </summary>
    public static void StaticReceive(string msg) => chat.Receive(msg);

    /// <summary> Speaks the message before writing it. </summary>
    public void ReceiveTTS(string color, Friend author, string msg)
    {
        // play the message in the local player's position if he is its author
        if (author.IsMe)
            SamAPI.TryPlay(msg, Networking.LocalPlayer.Voice);

        // or find the author among the other players and play the sound from them
        else if (Networking.Entities.TryGetValue(author.Id.AccountId, out var entity) && entity is RemotePlayer player)
            SamAPI.TryPlay(msg, player.Voice);

        //AudioSource.PlayClipAtPoint(SamAPI.Clip, NewMovement.Instance.transform.position);

        NewReceive(color, author, msg, true);
    }

    #endregion
}