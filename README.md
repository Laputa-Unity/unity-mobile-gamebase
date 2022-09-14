# Gamee base
Description: Gamebase for mobile hyper casual, casual game 
## Table of contents
- [Introduction](#Introduce)
- [Installation](#Install)
- [Documentation](#Documents)
  - [Sound](#Sound-Controller)
  - [Popup](#Popup-Controller)

## Introduce
- This is a basic casual game base for intern and junior, it's integrated with [ads](https://github.com/gamee-studio/ads), [notification](https://github.com/pancake-llc/local-notification), [firebase tracking](https://github.com/pancake-llc/firebase-app), [firebase remote config](https://github.com/pancake-llc/firebase-remote-config)
- The project has simple data stream flow to help you to handle the game easier.

## Install
- Version: **Unity 2021.3.0f1**
- Type select: **Android**

## Documents
### Sound Controller
<details><summary>Adding new sound</summary>
<p>

- Add sound by adding new **SoundType** in file **SoundConfig.cs** then click **Update sound list** in **SoundConfig scriptable object**.
![image](https://user-images.githubusercontent.com/88299194/171227540-bb29f744-2e3c-4d64-8bad-07094f2fc9bb.png)
![image](https://user-images.githubusercontent.com/88299194/171226912-166151c1-c0f8-4730-ac9f-636a8070eae5.png)
  
</p>
</details>

<details><summary>Playing sound</summary>
<p>

```SoundController.Instance.PlayBackground(SoundType.Background)``` or ```SoundController.Instance.PlayFX(SoundType.Win)```
  
</p>
</details>

### Popup Controller
<details><summary>Adding new popup</summary>
<p>

- _Step 1: Create a new prefab attaching a script extend an popup interface (for example: ```public class PopupLose : Popup```)_
![image](https://user-images.githubusercontent.com/88299194/171231178-8c2bbbb7-43ed-48a5-b017-d489daaeea6c.png)
![image](https://user-images.githubusercontent.com/88299194/171231384-a286ccac-ecf0-4926-80ab-c375d9b8ea2c.png)
- _Step 2: Attach the prefab to PopupController list_
![image](https://user-images.githubusercontent.com/88299194/171232063-7661a9c1-b1f9-4bbe-a524-2dfc034c1648.png)

</p>
</details>

<details><summary>Handling popup</summary>
<p>

- Get a popup: ```PopupController.Instance.Get<PopupInGame>()```
- Show a popup: ```PopupController.Instance.Show<PopupInGame>()```
- Hide a popup: ```PopupController.Instance.Hide<PopupInGame>()```

- Here some override functions you can use:
```
protected virtual void AfterInstantiate() { }
protected virtual void BeforeShow() { }
protected virtual void AfterShown() { }
protected virtual void BeforeHide() { }
protected virtual void AfterHidden() { }
```
  
</p>
</details>

