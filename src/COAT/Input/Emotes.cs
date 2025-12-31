namespace COAT.Input;

using COAT.Assets;
using COAT.Net;
using COAT.Net.Types;
using COAT.UI;
using COAT.UI.Screen;
using System.Collections;
using UnityEngine;

public class Emotes : MonoSingleton<Emotes>
{
    static NewMovement nm => NewMovement.Instance;
    static CameraController cc => CameraController.Instance;
    static PrefsManager pm => PrefsManager.Instance;
    static PlayerInput pi => InputManager.Instance.InputSource;

    /// <summary> An array containing the length of all emotions in seconds. </summary>
    private readonly float[] emojiLength = { 2.458f, 4.708f, 1.833f, 2.875f, 0f, 9.083f, -1f, 11.022f, -1f, 3.292f, 0f, -1f };

    /// <summary> Current emotion preview, can be null. </summary>
    public GameObject EmojiPreview;
    /// <summary> Start time of the current emotion and hold time of the emotion wheel key. </summary>
    public float EmojiStart, HoldTime;
    /// <summary> Id of the currently playing emotion. </summary>
    public byte Emoji = 0xFF, Rps;

    /// <summary> Environmental mask needed to prevent the skateboard from riding on water and camera from falling trough the ground. </summary>
    private readonly int mask = LayerMaskDefaults.Get(LMD.Environment);
    /// <summary> Whether the death must be fake on this level. </summary>
    private static bool fakeDeath => nm.endlessMode || Tools.Scene == "Level 0-S";

    /// <summary> Speed at which the skateboard moves. </summary>
    public float SkateboardSpeed;
    /// <summary> When the maximum skateboard speed is exceeded, deceleration is activated. </summary>
    public bool SlowsDown;
    /// <summary> Current falling particle object. </summary>
    public GameObject FallParticle;

    /// <summary> Starting and ending position of third person camera. </summary>
    private readonly Vector3 start = new(0f, 6f, 0f), end = new(0f, .1f, 0f);
    /// <summary> Third person camera position. </summary>
    private Vector3 position;
    /// <summary> Third person camera rotation. </summary>
    private Vector2 rotation;
    /// <summary> Third person camera target. If an emote is playing, the camera will aim at the local player, otherwise, at a remote player. </summary>
    private int targetPlayer;

    public static void Load()
    {
        Tools.Create<Movement>("Movement");

        Events.OnLoaded += () =>
        {
            Instance.StartEmoji(0xFF, false);
        };
    }

    private void LateUpdate()
    {
        // skateboard logic
        Skateboard.Instance.gameObject.SetActive(Emoji == 0x0B);
        if (Emoji == 0x0B && !UI.AnyDialog)
        {
            // speed & dash logic
            SkateboardSpeed = Mathf.MoveTowards(SkateboardSpeed, 20f, (SlowsDown ? 28f : 12f) * Time.deltaTime);
            nm.boostCharge = Mathf.MoveTowards(nm.boostCharge, 300f, 70f * Time.deltaTime);

            if (pi.Dodge.WasPerformedThisFrame)
            {
                if (nm.boostCharge >= 100f || (AssistController.Instance.majorEnabled && AssistController.Instance.infiniteStamina))
                {
                    SkateboardSpeed += 20f;
                    nm.boostCharge -= 100f;

                    // major assists make it possible to dash endlessly so we need to clamp boost charge
                    if (nm.boostCharge < 0f) nm.boostCharge = 0f;

                    Instantiate(nm.dodgeParticle, nm.transform.position, nm.transform.rotation);
                    AudioSource.PlayClipAtPoint(nm.dodgeSound, nm.transform.position);
                }
                else Instantiate(nm.staminaFailSound);
            }

            if (SkateboardSpeed >= 70f && !SlowsDown)
            {
                SlowsDown = true;
                FallParticle = Instantiate(nm.fallParticle, nm.transform);
            }
            if (SkateboardSpeed <= 40f && SlowsDown)
            {
                SlowsDown = false;
                Destroy(FallParticle);
            }

            // move the skateboard forward
            var player = nm.transform;
            nm.rb.velocity = (player.forward * SkateboardSpeed) with { y = nm.rb.velocity.y };

            // don’t let the front and rear wheels fall into the ground
            void Check(Vector3 pos)
            {
                if (Physics.Raycast(pos, Vector3.down, out var hit, 1.5f, mask) && hit.distance > .8f) player.position = player.position with { y = hit.point.y + 1.5f };
            }
            Check(player.position + player.forward * 1.2f);
            Check(player.position - player.forward * 1.2f);

            // turn to the sides
            player.Rotate(Vector3.up * pi.Move.ReadValue<Vector2>().x * 120f * Time.deltaTime);
        }

        // third person camera
        if (Emoji != 0xFF || (LobbyController.Online && nm.dead && fakeDeath))
        {
            // rotate the camera according to mouse sensitivity
            if (!UI.AnyDialog)
            {
                rotation += pi.Look.ReadValue<Vector2>() * OptionsManager.Instance.mouseSensitivity / 10f;
                rotation.y = Mathf.Clamp(rotation.y, 5f, 170f);

                // cancel animation if space is pressed
                if (Input.GetKey(KeyCode.Space)) StartEmoji(0xFF);
            }

            // turn on gravity, because if the taunt was launched on the ground, then it is disabled by default
            nm.rb.useGravity = true;

            var cam = cc.cam.transform;
            var player = nm.dead && Networking.Entities.TryGetValue(LobbyController.At(targetPlayer)?.Id.AccountId ?? 0, out var rp) && rp != Networking.LocalPlayer
                ? rp.transform.position + Vector3.up * 2.5f
                : nm.transform.position + Vector3.up;

            // move the camera position towards the start if the animation has just started, or towards the end if the animation ends
            bool ends = !nm.dead && Time.time - EmojiStart > emojiLength[Emoji] && emojiLength[Emoji] != -1f;
            position = Vector3.MoveTowards(position, ends ? end : start, 12f * Time.deltaTime);

            // return the camera to its original position and rotate it around the player
            cam.position = player + position;
            cam.RotateAround(player, Vector3.left, rotation.y);
            cam.RotateAround(player, Vector3.up, rotation.x);
            cam.LookAt(player);

            // do not let the camera fall through the ground
            if (Physics.SphereCast(player, .25f, cam.position - player, out var hit, position.magnitude, mask))
                cam.position = hit.point + .5f * hit.normal;
        }

        // ultrasoap
        if (Tools.Scene != "Main Menu" && !nm.dead)
            nm.rb.constraints = UI.AnyDialog
                ? RigidbodyConstraints.FreezeAll
                : Instance.Emoji == 0xFF || Instance.Emoji == 0x0B // skateboard
                    ? RigidbodyConstraints.FreezeRotation
                    : (RigidbodyConstraints)122;

    }

    /// <summary> Resets the values of the third person camera. </summary>
    public void StartThirdPerson()
    {
        rotation = new(cc.rotationY, cc.rotationX + 90f);
        position = new();
        targetPlayer = LobbyController.IndexOfLocal();
    }

    public void OnDied()
    {
        StartEmoji(0xFF);
        if (LobbyController.Online && fakeDeath) Events.Post(() =>
        {
            StartThirdPerson();
            nm.endlessMode = true; // take the death screen under control

            //nm.blackScreen.gameObject.SetActive(true);
            //nm.blackScreen.transform.Find("LaughingSkull").gameObject.SetActive(false);
            nm.screenHud.SetActive(false);
        });
    }

    /// <summary> Triggers an emoji with the given id. </summary>
    public void StartEmoji(byte id, bool updateState = true)
    {
        EmojiStart = Time.time;
        Emoji = id; // save id to sync it later

        if (updateState) Movement.UpdateState();
        Destroy(EmojiPreview);
        Destroy(FallParticle);

        // if id is -1, then the emotion was not selected
        if (id == 0xFF) return;
        else EmojiPreview = Doll.Spawn(nm.transform, Networking.LocalPlayer.Team, id, Rps).gameObject;

        // stop sliding so that the preview is not underground
        nm.playerCollider.height = 3.5f;
        nm.gc.transform.localPosition = new(0f, -1.256f, 0f);

        // rotate the third person camera in the same direction as the first person camera
        StartThirdPerson();
        SkateboardSpeed = 0f;

        Bundle.Hud("emoji", true); // telling how to interrupt an emotion
        StopCoroutine("ClearEmoji");
        if (emojiLength[id] != -1f) StartCoroutine("ClearEmoji");
    }

    /// <summary> Returns the emoji id to -1 after the end of an animation. </summary>
    public IEnumerator ClearEmoji()
    {
        yield return new WaitForSeconds(emojiLength[Emoji] + .5f);

        if (Emoji == 3) LobbyController.Lobby?.SendChatString("#/r" + Rps);
        StartEmoji(0xFF);
    }
}