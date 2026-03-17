# Mobile Game Base — Core Singleton Architecture (Android & iOS)

<div align="center">
<img width="270" height="480" src="https://github.com/user-attachments/assets/bb86d0e5-8869-4815-8158-bce630052c27">
<img width="270" height="480" src="https://github.com/user-attachments/assets/4aff266a-7cb9-4eb1-bf6a-035b8874fafb">
</div>

👋 **Hi there, I'm HoangVanThu**. This repository helps you build mobile games quickly.

- This is a **sample gamebase for mobile games** using the **singleton design pattern**. It lets you get up to speed quickly and ship a complete game fast. Custom packages provide powerful inspector, tween, and debug tools that accelerate development.
- **Note:** The singleton design pattern is generally considered an anti-pattern for large projects. Use this template for small-to-mid-sized mobile games.

---

## Table of Contents

1. [Features](#features)
2. [Installation](#installation)
3. [Folder Structure](#folder-structure)
4. [Architecture Overview](#architecture-overview)
5. [Data System](#data-system)
   - [ScriptableObject Configs](#scriptableobject-configs)
   - [Player Data](#player-data)
   - [How to Add a New Data Field](#how-to-add-a-new-data-field)
6. [Popup / UI System](#popup--ui-system)
   - [How to Create a New Popup](#how-to-create-a-new-popup)
   - [Show / Hide a Popup from Code](#show--hide-a-popup-from-code)
   - [Popup Lifecycle Hooks](#popup-lifecycle-hooks)
7. [Sound System](#sound-system)
   - [How to Add a New Sound](#how-to-add-a-new-sound)
   - [Playing Sounds from Code](#playing-sounds-from-code)
8. [Level System](#level-system)
   - [How to Add a New Level](#how-to-add-a-new-level)
   - [Level Loop Configuration](#level-loop-configuration)
9. [Observer / Event System](#observer--event-system)
   - [How to Add a New Event](#how-to-add-a-new-event)
10. [Shop & Item System](#shop--item-system)
    - [How to Add a New Skin / Item](#how-to-add-a-new-skin--item)
11. [Daily Reward System](#daily-reward-system)
12. [Extension Packages](#extension-packages)
    - [CustomTween](#customtween)
    - [CustomInspector](#custominspector)
    - [CustomHierarchy](#customhierarchy)
    - [CustomPlayerPref](#customplayerpref)
    - [CustomFindReference](#customfindreference)
    - [CustomBuildReport](#custombuildreport)
13. [Debug Tools](#debug-tools)
14. [Third Party](#third-party)
15. [Support](#support)

---

## Features

- [x] `Resources System` — flexible UI with smooth animations
- [x] `Gameplay` — drag-and-drop puzzle with win/lose detection
- [x] `Setting` — music, sound FX, and vibration toggles
- [x] `Shop` — buy/unlock skins with in-game currency
- [x] `Daily Reward` — calendar-based daily reward system
- [x] `Lucky Spin` — daily spin feature
- [x] `Debug` — quick debug popup for level/skin shortcuts
- [x] `Data Storage` — encrypted JSON save system
- [x] `Custom Inspector` — configure which variables show in the Inspector
- [x] `Custom Hierarchy` — richer hierarchy panel
- [x] `Find Reference` — find all references to any asset
- [x] `Build Report` — build statistics report
- [x] `Custom Tween` — lightweight tweening library (alternative to DOTween)
- [x] `PlayerPref Editor` — view/edit PlayerPrefs in the Editor
- [x] `Debug Console` — advanced in-game debugging console
- [x] `Level Editor` — design levels with a custom editor tool
- [ ] `Laputa Thirdparty` — Firebase, Ads integration *(planned)*
- [ ] `Localization` — multi-language support *(planned)*
- [ ] `Rank` — online leaderboard *(planned)*
- [ ] `Push Notification` — mobile notifications *(planned)*

---

## Installation

| Requirement | Version |
|-------------|---------|
| Unity | **6000.2.10f1** |
| Platform | **Android** or **iOS** |

1. Clone or download this repository.
2. Open the project in **Unity 6000.2.10f1** (or a compatible version).
3. Switch the build platform to **Android** or **iOS** via *File → Build Settings*.
4. Open `Assets/_Project/Scenes/LoadingScene.unity` and press **Play**.

---

## Folder Structure

```
Assets/
├── _Project/
│   ├── Animations/              # Animation clips
│   ├── Audio/                   # Music and SFX files
│   ├── Config/                  # All ScriptableObject assets (.asset)
│   │   ├── GameConfig.asset
│   │   ├── SoundConfig.asset
│   │   ├── PopupConfig.asset
│   │   ├── LevelConfig.asset
│   │   ├── DailyRewardConfig.asset
│   │   ├── VibrationConfig.asset
│   │   ├── ItemConfig.asset
│   │   ├── InternetConfig.asset
│   │   └── VisualEffectConfig.asset
│   ├── Fonts/
│   ├── Materials/
│   ├── Models/
│   ├── Prefabs/
│   │   └── Controller/          # Singleton manager prefabs
│   ├── Resources/
│   │   └── Levels/              # Level prefabs (Level 1.prefab … Level N.prefab)
│   ├── Scenes/
│   │   ├── LoadingScene.unity   # Entry point
│   │   └── GameplayScene.unity  # Main game loop
│   ├── Scripts/
│   │   ├── Common/              # Shared animation helpers (GoMove, GoBounce, etc.)
│   │   ├── Gameplay/
│   │   │   └── Level/           # Level.cs, Pill.cs, Hole.cs
│   │   └── System/
│   │       ├── GameManager.cs
│   │       ├── Config/          # ScriptableObject class definitions
│   │       ├── Controller/      # All singleton controllers (12 total)
│   │       ├── Data/            # Player data + encryption
│   │       ├── Observer/        # Event system (partial classes)
│   │       ├── Pattern/         # Singleton / SingletonDontDestroy base classes
│   │       ├── UI/              # All popup scripts + PopupCreator tool
│   │       ├── Components/      # Popup base class, UIEffect
│   │       ├── GUI/             # CustomButton, CustomSwitchButton, BackgroundScroller
│   │       ├── Resources/       # Resource loading helpers
│   │       ├── Vibration/       # Haptic feedback
│   │       └── Common/          # Utility, SafeArea, CanvasScaleHandler
│   ├── Textures/ & Sprites/
│   ├── ~ExtensionPackages/      # Reusable custom editor packages
│   │   ├── CustomInspector/
│   │   ├── CustomTween/
│   │   ├── CustomHierarchy/
│   │   ├── CustomFindReference/
│   │   ├── CustomBuildReport/
│   │   └── CustomPlayerPref/
│   └── ~LevelEditor/            # Level design editor (project-specific)
├── Plugins/                     # Android / iOS native plugins
├── Spine/                       # Spine animation runtime
└── ThirdParty/                  # TextMesh Pro, Lean packages
```

---

## Architecture Overview

```
┌─────────────────────────────────────────────┐
│            UI Layer (PopupController)        │
│  PopupHome, PopupShop, PopupSetting …        │
│  Registry pattern — Show<T>() / Hide<T>()    │
└──────────────────┬──────────────────────────┘
                   │
       ┌───────────┴───────────┐
       ▼                       ▼
┌──────────────┐      ┌────────────────────┐
│  GameManager │      │   Observer (Events) │
│  (GameState) │◄────►│   static Actions    │
└──────┬───────┘      └──────────┬─────────┘
       │                         │
  ┌────┴─────────────────────────┤
  ▼                              ▼
Controllers (12)            PlayerData
LevelController             (partial classes)
SoundController             Data.SaveData()
ItemController              Data.LoadData()
PlayerDataController        EncryptionHelper
…
```

**Scenes:**

| Scene | Role |
|-------|------|
| `LoadingScene` | Bootstraps all singleton controllers, loads player data, then loads `GameplayScene` |
| `GameplayScene` | Main game loop; levels are spawned dynamically at runtime |

All controllers inherit from `SingletonDontDestroy<T>` and survive scene transitions.

---

## Data System

### ScriptableObject Configs

All game configuration lives in `Assets/_Project/Config/` as ScriptableObject assets. They are never modified at runtime — only read.

| Asset | Class | Purpose |
|-------|-------|---------|
| `GameConfig.asset` | `GameConfig` | `isTesting` flag to enable debug tools |
| `SoundConfig.asset` | `SoundConfig` | Maps `SoundName` enum values to `AudioClip` lists |
| `PopupConfig.asset` | `PopupConfig` | Holds prefab references for every popup |
| `LevelConfig.asset` | `LevelConfig` | `maxLevel`, `levelLoopType`, `startLoopLevel` |
| `DailyRewardConfig.asset` | `DailyRewardConfig` | Per-day reward definitions |
| `VibrationConfig.asset` | `VibrationConfig` | Delay time between haptic pulses |
| `ItemConfig.asset` | `ItemConfig` | All shop items / skins |

**Creating a new ScriptableObject config:**

1. Create a C# class that inherits from `ScriptableObject` with a `[CreateAssetMenu]` attribute:

```csharp
[CreateAssetMenu(fileName = "MyConfig", menuName = "ScriptableObject/MyConfig")]
public class MyConfig : ScriptableObject
{
    public int someValue;
    public string someText;
}
```

2. In the Unity Editor, right-click inside `Assets/_Project/Config/` → **Create → ScriptableObject → MyConfig**.
3. Assign the asset as a serialized field on the controller or MonoBehaviour that needs it.

---

### Player Data

Player data is stored as an **encrypted JSON** file at:

```
Application.persistentDataPath/player_data.json
```

The data class is split into partial files for clarity:

| File | Contents |
|------|---------|
| `PlayerData.cs` | `IsFirstPlaying`, `CurrentLevelIndex`, `CurrentEnergy`, `CurrentGold`, `CurrentDiamond`, `SavingReward` |
| `PlayerData.Setting.cs` | `MusicVolume`, `SoundVolume`, `VibrationState` |
| `PlayerData.Shop.cs` | `CurrentSkin`, `OwnedSkins` |
| `PlayerData.DailyReward.cs` | `CurrentDailyReward`, `LastDailyRewardClaimed` |

**Accessing player data from anywhere:**

```csharp
// Read
int gold = Data.PlayerData.CurrentGold;

// Write (automatically fires Observer events)
Data.PlayerData.CurrentGold += 100;

// Save manually (also auto-saves on app pause/quit)
Data.SaveData();

// Load (called automatically at startup)
Data.LoadData();

// Delete save file
Data.ClearData();
```

Property setters on `PlayerData` automatically invoke the relevant `Observer` events, so listeners update immediately (e.g., HUD counters refresh when gold changes).

---

### How to Add a New Data Field

1. Open (or create) the appropriate partial file in `Assets/_Project/Scripts/System/Data/`.
2. Add a `[SerializeField]` backing field and a public property. Fire an `Observer` event in the setter when other systems need to react:

```csharp
// PlayerData.cs  (or a new partial file)
public partial class PlayerData
{
    [SerializeField] private int currentStars;

    public int CurrentStars
    {
        get => currentStars;
        set
        {
            Observer.StarsChanged?.Invoke(value - currentStars);
            currentStars = value;
        }
    }
}
```

3. Declare the new event in `Observer.cs` (see [Observer / Event System](#observer--event-system)).
4. The field is saved/loaded automatically because `Data.SaveData()` serializes the whole `PlayerData` object.

---

## Popup / UI System

All popups inherit from the `Popup` base class and are managed by `PopupController` (a singleton).  
`PopupController` holds a `Dictionary<Type, Popup>` so every popup can be retrieved, shown, or hidden by its C# type.

### How to Create a New Popup

#### Option A — Using the built-in PopupCreator tool (recommended)

1. In the **Hierarchy**, select the `PopupCreator` GameObject (found in `GameplayScene`).
2. In the **Inspector**, fill in:
   - **Popup Prefab** — the template prefab to copy from (e.g., an existing popup).
   - **Popup Saving Directory** — where the new prefab will be saved (e.g., `Assets/_Project/Prefabs/UI/`).
   - **Script Saving Directory** — where the new C# script will be saved (e.g., `Assets/_Project/Scripts/System/UI/`).
3. Click the **Create New Popup** button. A dialog will appear asking for the popup name (e.g., `PopupAchievement`).
4. Click **OK**. The tool will:
   - Generate a new C# script (`PopupAchievement.cs`) that inherits from `Popup`.
   - Duplicate the template prefab and attach the new script.
   - Save both files at the configured paths.
5. **Register the new popup** — open `Assets/_Project/Config/PopupConfig.asset` in the Inspector and add the new prefab to the `Popups` list.

#### Option B — Manually

1. Create a new C# script that inherits from `Popup`:

```csharp
public class PopupAchievement : Popup
{
    // Override lifecycle hooks as needed
    protected override void BeforeShow()
    {
        // Called before the popup becomes visible
    }

    protected override void AfterShown()
    {
        // Called after the show animation completes
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        // Called before the hide animation starts
    }

    protected override void AfterHidden()
    {
        base.AfterHidden();
        // Called after the popup is fully hidden
    }
}
```

2. Create a prefab in `Assets/_Project/Prefabs/UI/` and attach the script to the root GameObject.  
   The root must also have a **Canvas** component and a **CanvasGroup** component.
3. Add the prefab to **PopupConfig** → `Popups` list.

---

### Show / Hide a Popup from Code

```csharp
// Show with no animation
PopupController.Instance.Show<PopupAchievement>();

// Show with scale + fade animation
PopupController.Instance.Show<PopupAchievement>(PopupAnimation.ScaleFade);

// Hide
PopupController.Instance.Hide<PopupAchievement>();
PopupController.Instance.Hide<PopupAchievement>(PopupAnimation.FadeOnly);

// Hide all open popups at once
PopupController.Instance.HideAll();

// Get a reference to a popup instance
if (PopupController.Instance.Get<PopupAchievement>() is PopupAchievement popup)
{
    popup.SetData(someData);
}
```

**Available animations (`PopupAnimation` enum):**

| Value | Effect |
|-------|--------|
| `None` | Instant show/hide |
| `ScaleFade` | Scale from small → normal + fade in |
| `ScaleFade2` | Scale from large → normal + fade in |
| `FadeOnly` | Fade in/out only |

Animation parameters (duration, easing, scale range) are configurable per popup in the **Inspector** on the `Popup` component.

---

### Popup Lifecycle Hooks

Override these virtual methods in your popup subclass to react at each stage:

```
OnInstantiate()  →  called once when the popup is first created
BeforeShow()     →  called every time before the popup appears
AfterShown()     →  called after the show animation finishes
BeforeHide()     →  called before the hide animation starts
AfterHidden()    →  called after the popup is fully hidden / deactivated
```

You can also assign one-time callbacks without subclassing:

```csharp
var popup = PopupController.Instance.Get<PopupWin>() as PopupWin;
popup.AfterHiddenAction = () => Debug.Log("Win popup closed!");
popup.Show(PopupAnimation.ScaleFade);
```

---

## Sound System

### How to Add a New Sound

1. Add a new entry to the `SoundName` enum in `SoundConfig.cs`:

```csharp
public enum SoundName
{
    HomeBackgroundMusic,
    InGameBackgroundMusic,
    ClickButton,
    PurchaseCompleted,
    LevelComplete,   // ← new entry
}
```

2. Open `Assets/_Project/Config/SoundConfig.asset` in the Inspector.
3. Click the **Update Sound Data** button — this auto-adds a new row for `LevelComplete`.
4. Expand the new row and drag your `AudioClip(s)` into the **Clips** list.  
   Multiple clips are picked randomly each time the sound plays.
5. Set **Delay Time** if you want to debounce rapid repeated plays (e.g., `0.1` seconds).

### Playing Sounds from Code

```csharp
// Play a one-shot sound effect
SoundController.Instance.PlayFX(SoundName.LevelComplete);

// Play/switch background music
SoundController.Instance.PlayBackground(SoundName.HomeBackgroundMusic);
```

Volumes are driven by `Data.PlayerData.MusicVolume` and `Data.PlayerData.SoundVolume` (both 0–1).  
Setting those properties automatically fires `Observer.MusicChanged` / `Observer.SoundChanged`, which `SoundController` listens to and applies immediately.

---

## Level System

Levels are stored as prefabs at `Assets/_Project/Resources/Levels/` and loaded at runtime via `Resources.Load`.

### How to Add a New Level

1. Create your level GameObject in the scene, add a `Level` script to the root.
2. **Name the prefab exactly** `Level {N}` where N is the level number (e.g., `Level 11.prefab`).
3. Save it to `Assets/_Project/Resources/Levels/`.
4. Open `Assets/_Project/Config/LevelConfig.asset` and update **Max Level** to include the new level.

### Level Loop Configuration

Once the player finishes all designed levels, the system loops based on `LevelConfig`:

| `LevelLoopType` | Behavior |
|-----------------|---------|
| `Recycle` | Replays levels from `startLoopLevel` → `maxLevel` in order |
| `Random` | Picks a random level between 1 and `maxLevel` |

---

## Observer / Event System

`Observer` is a **static partial class** containing C# `Action` delegates. It acts as a lightweight pub/sub event bus — any script can publish or subscribe without holding a direct reference.

**Subscribing to an event:**

```csharp
void OnEnable()
{
    Observer.GoldChanged += OnGoldChanged;
    Observer.WinLevel    += OnWinLevel;
}

void OnDisable()
{
    Observer.GoldChanged -= OnGoldChanged;
    Observer.WinLevel    -= OnWinLevel;
}

void OnGoldChanged(int delta) { /* update HUD */ }
void OnWinLevel(Level level)  { /* show confetti */ }
```

**Publishing an event:**

```csharp
Observer.GoldChanged?.Invoke(50);   // notify all listeners
```

**Available events (selected):**

| Event | Signature | When fired |
|-------|-----------|-----------|
| `StartLevel` | `Action<Level>` | Level starts |
| `WinLevel` | `Action<Level>` | Player wins |
| `LoseLevel` | `Action<Level>` | Player loses |
| `ReplayLevel` | `Action<Level>` | Level replayed |
| `SkipLevel` | `Action<Level>` | Level skipped |
| `GoldChanged` | `Action<int>` | Gold amount changes |
| `DiamondChanged` | `Action<int>` | Diamond amount changes |
| `EnergyChanged` | `Action<int>` | Energy amount changes |
| `MusicChanged` | `Action` | Music volume changes |
| `SoundChanged` | `Action` | SFX volume changes |
| `VibrationChanged` | `Action` | Vibration toggle changes |
| `EquipPlayerSkin` | `Action<string>` | Skin equipped |
| `Notify` | `Action<string, Vector3>` | Floating text notification |

### How to Add a New Event

1. Open (or create) an appropriate partial file inside `Assets/_Project/Scripts/System/Observer/` (e.g., `Observer.Gameplay.cs`).
2. Declare the event:

```csharp
public static partial class Observer
{
    public static Action<int> StarsChanged;
}
```

3. Fire it from the property setter or game logic:

```csharp
Observer.StarsChanged?.Invoke(delta);
```

4. Subscribe/unsubscribe in any MonoBehaviour that needs to react.

---

## Shop & Item System

Items (skins, weapon skins, etc.) are defined in `Assets/_Project/Config/ItemConfig.asset`.

Each `ItemData` entry has:

| Field | Type | Description |
|-------|------|-------------|
| `identity` | `string` | Unique ID (e.g., `"Skin_01"`) |
| `itemType` | `ItemType` | `PlayerSkin` or `WeaponSkin` |
| `buyType` | `BuyType` | How the item is obtained |
| `skinPrefab` | `GameObject` | The 3-D / 2-D skin prefab |
| `shopIcon` | `Sprite` | Thumbnail shown in the shop |
| `price` | `int` | Cost in gold (shown only when `buyType == Money`) |

**`BuyType` values:**

| Value | Meaning |
|-------|---------|
| `Default` | Free — unlocked automatically at startup |
| `Money` | Purchase with gold |
| `DailyReward` | Obtained via the daily reward calendar |
| `WatchAds` | Obtained by watching a rewarded ad |
| `Event` | Obtained during limited-time events |

### How to Add a New Skin / Item

1. Open `Assets/_Project/Config/ItemConfig.asset`.
2. Click **+** on the `Item Data` list and fill in the fields described above.
3. Set a unique `identity` string — this is the key stored in `Data.PlayerData.OwnedSkins`.
4. If `buyType` is `Default`, the skin will be unlocked automatically on first launch via `ItemConfig.UnlockDefaultSkins()`.

**Checking / granting ownership from code:**

```csharp
// Check if the player owns a skin
bool owns = Data.PlayerData.IsOwnedSkin("Skin_01");

// Grant a skin
Data.PlayerData.OwnedSkins.Add("Skin_01");

// Equip a skin
Data.PlayerData.CurrentSkin = "Skin_01";
Observer.EquipPlayerSkin?.Invoke("Skin_01");
```

---

## Daily Reward System

Configured in `Assets/_Project/Config/DailyRewardConfig.asset`.

- **`dailyRewardData`** — the fixed calendar (e.g., days 1–7).
- **`loopDailyRewardData`** — rewards that cycle after the fixed list ends.

Each `DailyRewardData` entry:

| Field | Description |
|-------|-------------|
| `dailyRewardType` | `Money` or `Skin` |
| `icon` | Display sprite |
| `value` | Gold amount (Money type only) |
| `skinID` | Skin identity string (Skin type only) |

The `DailyRewardController` retrieves the correct reward using:

```csharp
DailyRewardData reward = DailyRewardController.Instance.GetDailyRewardData(dayIndex);
```

Claim logic lives in `PopupDailyReward.cs` and updates `Data.PlayerData.CurrentDailyReward` and `Data.PlayerData.LastDailyRewardClaimed`.

---

## Extension Packages

All packages live in `Assets/_Project/~ExtensionPackages/` and are designed to work in any Unity project.

### CustomTween

A lightweight tweening library — use instead of DOTween for smaller build sizes.

```csharp
// Scale an object
Tween.Scale(transform, Vector3.zero, Vector3.one, duration: 0.5f, Ease.OutBack);

// Fade a CanvasGroup
Tween.Alpha(canvasGroup, from: 0f, to: 1f, duration: 0.3f);

// Chain animations in a Sequence
Sequence.Create()
    .ChainDelay(1f)
    .Chain(Tween.Scale(transform, Vector3.zero, Vector3.one, 0.5f, Ease.OutBack))
    .ChainCallback(() => Debug.Log("Done!"))
    .OnComplete(() => gameObject.SetActive(false));
```

All tweens support `useUnscaledTime: true` so they work correctly when the game is paused (`Time.timeScale = 0`).

### CustomInspector

Provides attribute-based Inspector customization:

```csharp
[ReadOnly]  public int score;          // Shows field but prevents editing
[ShowIf("isEnabled", true)] public float speed;  // Conditional visibility

[Button]
public void ResetScore() { score = 0; } // Adds a clickable button in the Inspector

[TableList] public List<ItemData> items; // Renders a list as a compact table
```

### CustomHierarchy

Enhances the Hierarchy panel with color labels, icons, and separators. Configure via the GameBase menu in the Unity Editor.

### CustomPlayerPref

Opens an editor window (*Window → Custom Player Pref*) that lets you read, edit, and delete all `PlayerPrefs` keys without writing code.

### CustomFindReference

Right-click any asset in the Project window → **Find References** to see every scene, prefab, and asset that references it.

### CustomBuildReport

After a build, open *Window → Build Report* to see a breakdown of asset sizes, textures, audio, and scripts.

---

## Debug Tools

### PopupDebug (in-game)

Accessible from the Home screen when `GameConfig.isTesting = true`. Lets you:
- Skip to any level.
- Unlock all skins instantly.
- Clear all save data.

### PopupDebugConsole

A floating, draggable console overlay (enabled when `isTesting = true`) with:
- Real-time FPS counter.
- Unity log output (info / warning / error).
- Custom command panel — register commands via `Observer.DebugConsole` events.
- Profiler panel for memory stats.

### CustomPlayerPref Editor

Access via *Window → Custom Player Pref* to view all saved PlayerPref keys directly in the Editor.

---

## Third Party

| Library | Purpose |
|---------|---------|
| **LeanTouch** | Multi-touch input and drag-and-drop |
| **LeanPool** | Object pooling for UI notifications and VFX |
| **TextMesh Pro** | High-quality text rendering |
| **Spine** | Skeletal animation runtime |

---

## Support

- If you like this project, please give it a ⭐ on GitHub!
- I would greatly appreciate it if you could support me with a cup of coffee:

<a href="https://www.buymeacoffee.com/HoangVanThu">
<img src="https://www.the3rdsequence.com/texturedb/images/donate/buymeacoffee.svg" width="200" height="47"/>
