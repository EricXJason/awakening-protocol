# 《覺醒協議》 (Awakening Protocol)

<div align="center">

[![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-black?style=for-the-badge&logo=unity)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Meta%20Quest-blue?style=for-the-badge&logo=oculus)](https://www.meta.com/quest/)
[![SDK](https://img.shields.io/badge/SDK-OpenXR%20%7C%20XRI%203.0-purple?style=for-the-badge)](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html)
[![Genre](https://img.shields.io/badge/Genre-VR%20Puzzle%20Adventure-orange?style=for-the-badge)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23-blue?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)

</div>

## 專案簡介

《覺醒協議 (Awakening Protocol)》是一部基於瑞士心理學家皮亞傑 (Jean Piaget) 兒童認知發展理論開發的 VR 冒險解謎遊戲。玩家在虛擬認知重構設施中甦醒，依序通過象徵不同認知階段的環境試煉，重構自我意識與邏輯思維，並根據遊玩過程中的累積失敗次數導向雙結局分支。

本專案重點在於**模組解耦**、**關卡流程抽象化**與 **XR Interaction Toolkit (XRI 3.0)** 的工程實踐，消除過場時序與各關卡之間的重複代碼，確保流暢穩定的 VR 互動體驗。

---

## 實機展示影片

* 🎥 **實機展示影片**：[https://youtu.be/QG96sBGAUuI](https://youtu.be/QG96sBGAUuI)

---

## 核心玩法與關卡試煉

遊戲以皮亞傑認知發展四階段為設計骨幹，玩家必須在 VR 空間中以雙手實體互動通過一連串環境謎題：

* **Stage 0: 邊界序章 (Limbo Stage)**：
  - 玩家在極簡黑白抽象空間甦醒，跟隨前方幽浮巡航的導引光球前進，熟悉 VR 視角轉向、平移與瞬移操作，抵達首道閘門。
* **Stage 1: 感覺動作期 (Sensorimotor Stage)**：
  - 環形房間周遭懸浮著高低不同的音階光球。光球會依特定旋律發光並發出音階，玩家必須伸出雙手實體觸碰光球，按照正確順序彈奏複誦旋律，達成感覺與動作的協調反饋以開啟閘門。
* **Stage 2: 前運思期 (Preoperational Stage)**：
  - 空間幾何形狀與符號匹配。玩家需在場景中抓取不同形狀的 3D 積木（立方體、圓柱等），精確嵌入對應形狀的插槽檢驗基座。若不慎掉落，物件會自動平滑回彈至原位，全數配對完成後解開防護柵欄。
* **Stage 3: 具體運思期 (Concrete Operational Stage)**：
  - 能量算式守恆解謎。中央為一座斷開的高架通路，兩側基座擺放著標有數字與運算符號（+、-、×）的能量電池。玩家需抓取電池插入左右電路槽，使兩側電壓算式數值相等達成平衡，成功通電後中央升降大橋升起。
* **Stage 4: 形式運思期 (Formal Operational Stage)**：
  - 高階抽象假設與迷宮抉擇。迷宮內部會依據玩家當前累計的失敗次數動態切換格局（共 8 種迷宮走向），玩家必須在多重岔路與帶有符號的「生門 / 死門」之間進行邏輯推理。一旦選錯死門將被電網阻隔並重置回迷宮起點。
* **Stage 5: 雙結局分支 (Dual Branching Endings)**：
  - 由 `GameManager` 全程記錄總失敗次數。若失敗次數低於門檻，通過終點線後迎來「意識成功覺醒」的明亮結局；若失敗過多則觸發「重構失敗」的重置輪迴。

---

## 系統架構圖

```mermaid
flowchart TD
    subgraph CoreLayer ["Core / 核心管理中樞"]
        IGameManager["IGameManager (契約)"]
        GameManager["GameManager : IGameManager\n(語言、失敗計數、操作權限)"]
        SceneLoader["SceneLoader\n(非同步場景切換器)"]
    end

    subgraph InfrastructureLayer ["Infrastructure / 基礎設施"]
        EventBus["EventBus\n(泛型事件分發中樞)"]
        VREvents["VREvents\n(事件結構體)"]
    end

    subgraph ManagersLayer ["Managers / 關卡與過場管理"]
        ILevelManager["ILevelManager (契約)"]
        IStageAnimationService["IStageAnimationService (契約)"]
        BaseLevelManager["BaseLevelManager : ILevelManager\n(關卡生命週期與通關模板)"]
        StageSeq["StageSequenceController : IStageAnimationService\n(過場時序、視野暗角、字幕與語音)"]
        
        LimboMgr["LevelManager_LimboStage"]
        SensoriMgr["LevelManager_SensorimotorStage"]
        PreopMgr["LevelManager_PreoperationalStage"]
        ConcreteMgr["LevelManager_ConcreteOperationalStage"]
        FormalMgr["LevelManager_FormalOperationalStage"]
        EndingMgr["LevelManager_Ending"]
        
        LimboMgr -->|繼承| BaseLevelManager
        SensoriMgr -->|繼承| BaseLevelManager
        PreopMgr -->|繼承| BaseLevelManager
        ConcreteMgr -->|繼承| BaseLevelManager
        FormalMgr -->|繼承| BaseLevelManager
        EndingMgr -->|繼承| BaseLevelManager
        BaseLevelManager -->|時序控制| StageSeq
    end

    subgraph SubManagersLayer ["Managers/SubManagers / 關卡局部解謎管理"]
        PowerQuiz["PowerQuizManager\n(具體運思期算式仲裁)"]
        MazeMgr["MazeManager\n(形式運思期迷宮切換)"]
    end

    subgraph ControllersLayer ["Controllers / 場景物件與機關控制器"]
        IBarrierController["IBarrierController (契約)"]
        GateCtrl["GateController : IBarrierController"]
        FenceCtrl["FenceController : IBarrierController"]
        ElectricGrid["ElectricGridController"]
        FinishLine["FinishLineController"]
        LightBall["LightBallController"]
        BlockChecker["BlockCheckerController"]
        MazeDoor["MazeDoorController"]
    end

    subgraph AudioAndUILayer ["Audio & UI / 表現層"]
        IAudioService["IAudioService (契約)"]
        AudioManager["AudioManager\n(音效與語音播放)"]
        HUD["HUDController\n(抬頭顯示淡入淡出)"]
        Subtitle["SubtitleController\n(3D 空間字幕同步)"]
        Dimmer["ScreenDimmerController\n(全景暗角過場)"]
    end

    StageSeq -->|廣播 StagePhaseChangedEvent| EventBus
    GameManager -->|廣播 PlayerStatusChangedEvent| EventBus
    GameManager -->|廣播 HUDStatusChangedEvent| EventBus
    GameManager -->|廣播 SubtitleTriggeredEvent| EventBus
    GameManager -->|廣播 ScreenDimmerTriggeredEvent| EventBus

    EventBus -->|接收狀態開關| HUD
    EventBus -->|接收文字內容| Subtitle
    EventBus -->|接收透明度數值| Dimmer

    FormalMgr -->|累積失敗計數| GameManager
    FormalMgr -->|觸發分支跳轉| SceneLoader
```

---

## 軟體架構與技術實踐深度說明

本專案在技術實作上專注於「高內聚、低耦合」的架構分層，解決 VR 互動開銷、過場狀態衝突與關卡流程重複性問題：

### 1. XR Interaction Toolkit (XRI 3.0) 整合與互動狀態遮罩
* **用了什麼技術**：Unity 官方 XR Interaction Toolkit (XRI 3.0)，配置 `XR Origin`、雙手近遠端射線 (`Near-Far Interactor`)、直接抓取 (`XRDirectInteractor`) 與各類互動組件 (`XRGrabInteractable`, `XRSimpleInteractable`)。
* **為何而用**：提供標準化、低延遲的立體空間互動與位移控制，相容 OpenXR 跨平台標準，避免手寫射線與碰撞判定導致抖動。
* **怎麼設計**：在 `PlayerController` 統一管理移動（Continuous Move）、轉向（Snap Turn）、瞬移（Teleportation）與雙手射線互動器模組。
* **用什麼方法 (設計模式/演算法)**：
  - **事件驅動輸入遮罩 (Event-Driven Locomotion Masking)**：訂閱 `EventBus` 的 `PlayerStatusChangedEvent`。當過場動畫播放或黑幕淡出時，集中禁用 Locomotion 與射線組件，防止玩家在劇情時序中走動出界或誤觸機關。
  - **裝飾器防呆回彈 (Decorator Pattern)**：在 `XRGrabInteractable` 物件（電池、積木）附加通用工具 `ReturnToAnchor`。當玩家放手（`selectExited`）且物件未成功吸附至檢驗槽時，呼叫 DOTween 平滑回彈至原始錨點，解決 VR 抓取掉落穿模卡死的問題。
  - **互動訊號轉接解耦**：音階燈球掛載 `XRSimpleInteractable`，僅監聽接觸事件並轉發為自定義 C# `OnBallTouched` 事件，隔離關卡管理器對 XRI 底層 API 的依賴。

### 2. 關卡生命週期與模板方法模式 (Template Method Pattern)
* **用了什麼技術**：抽象基類 `BaseLevelManager` 與 6 個實體關卡控制器 (`LevelManager_*.cs`)，統一實作 `ILevelManager` 介面契約。
* **為何而用**：6 大關卡具備高度一致的流程（開場語音時序 ➔ 玩家解謎判定 ➔ 通關語音 ➔ 開啟終點閘門 ➔ 切換場景）。若各關卡單獨撰寫，會產生大量冗餘代碼與維護盲點。
* **怎麼設計**：將開場流程、語音觸發、過場中介協調與通關切換固化在 `BaseLevelManager` 抽象基類中，開放 `InitializeLevel()` 與各關特化檢查方法供子類覆寫。
* **用什麼方法**：
  - **模板方法模式 (Template Method Pattern)**：基類定義標準時序骨幹，子類專注自身特化邏輯（如：感覺動作期檢驗音階順序、前運思期檢驗積木插槽、形式運思期比對迷宮生門）。
  - **依賴反轉原則 (DIP)**：關卡管理器僅依賴 `IStageAnimationService` 與 `IBarrierController` 等抽象介面，不直接持有具體場景控制器實體。

### 3. 過場時序調度與中介者模式 (Mediator Pattern)
* **用了什麼技術**：通用過場控制器 `StageSequenceController`，實作 `IStageAnimationService` 介面契約。
* **為何而用**：關卡過場需要同步協調全景暗角淡出 (`ScreenDimmerController`)、語音播放 (`AudioManager`) 與 3D 空間字幕 (`SubtitleController`)，若由關卡管理器直接操縱各個視圖，會導致高度耦合。
* **怎麼設計**：`StageSequenceController` 擔任中介者角色，封裝「開場時序 (`TriggerOpeningProcess`)」與「結尾時序 (`TriggerEndingProcess`)」的協程流水線。
* **用什麼方法**：中介者模式 (Mediator Pattern)、Unity 協程 (Coroutines) 時序同步。

### 4. 模組解耦與泛型事件匯流排 (EventBus)
* **用了什麼技術**：純 C# 靜態泛型 `EventBus`，配合 `VREvents` 定義之領域事件結構體。
* **為何而用**：消滅場景內昂貴的 `FindObjectOfType` 與物件間的互相參照，提高模組獨立性。
* **怎麼設計**：內部以 `Dictionary<Type, Delegate>` 維護事件委派，監聽者在 `OnEnable` 與 `OnDisable` 進行生命週期訂閱與註銷。
* **用什麼方法**：發布-訂閱模式 (Publish-Subscribe Pattern)。

### 5. ScriptableObject 音訊與語音配置資產化 (SFXList)
* **用了什麼技術**：`Audio/SFXList.cs` 繼承 Unity `ScriptableObject`。
* **為何而用**：遊戲 6 大關卡具備大量語音引導、環境音與機關音效（音階聲、電網滋滋聲、閘門滑動聲）。若硬編碼在各場景或腳本中，會造成音訊資源分散難以維護。
* **怎麼設計**：定義專屬資料容器資產，集中管理鍵值名稱與 `AudioClip` 的對應陣列；`AudioManager` 在載入時注入該資產，提供 `GetClip(string key)` 供全局查詢。
* **用什麼方法**：資料驅動設計 (Data-Driven Design)、外部化資源字典快取。

### 6. 程式化補間動畫與平滑回彈 (DOTween & TransformAnimator)
* **用了什麼技術**：DOTween 補間動畫引擎與 `TransformAnimator.cs`。
* **為何而用**：場景中眾多懸浮光球、裝飾物件若全數掛載 Animator Controller，會產生大量不必要的狀態機開銷；而在 VR 中物件放手回彈需要平滑自然的物理曲線。
* **怎麼設計**：`TransformAnimator` 以數學正弦與旋轉矩陣在 `Update` 內驅動自體微幅旋轉、上下浮動與呼吸縮放；`ReturnToAnchor` 在放手時調用 `transform.DOMove` 與 `DORotate` 搭配 `Ease.OutBack` 彈性曲線回彈。
* **用什麼方法**：程式碼驅動補間 (Code-Driven Tweening)、緩動曲線演算法 (Easing Functions)。

---

## 核心服務介面契約規範 (Interfaces)

| 模組 | 介面名稱 | 檔案路徑 | 實作類別 | 定義之方法與屬性 | 職責說明 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Managers** | `ILevelManager` | `Managers/Interfaces/ILevelManager.cs` | `BaseLevelManager` (派生 6 關管理器) | `StageIndex`, `StageName`, `IsCompleted`, `InitializeLevel()`, `CompleteLevel()` | 規範關卡通關判定與生命週期。 |
| **Managers** | `IStageAnimationService` | `Managers/Interfaces/IStageAnimationService.cs` | `StageSequenceController` | `TriggerOpeningProcess()`, `TriggerEndingProcess()`, 事件委派 | 規範關卡開場/結尾時序、視野暗角補間與字幕廣播。 |
| **Controllers** | `IBarrierController` | `Controllers/Interfaces/IBarrierController.cs` | `GateController`, `FenceController` | `IsOpen`, `Open()`, `Close()` | 規範通行阻擋物（閘門、柵欄）之開關狀態。 |
| **Core** | `IGameManager` | `Core/Interfaces/IGameManager.cs` | `GameManager` | `FailCount`, `CurrentLanguage`, `IsPlayable`, `SetPlayerStatus()` | 定義遊戲全域核心狀態與雙語管理。 |
| **Audio** | `IAudioService` | `Audio/Interfaces/IAudioService.cs` | `AudioManager` | `PlaySFX()`, `GetClip()`, `GetVolume()` | 定義音效播放與語音查詢。 |

---

## 場景節點拓撲與組件配置 (Scene Hierarchy & Component Setup)

```text
Scene Hierarchy (Stage Scene)
├── [Core]
│   ├── [GameManager]               <-- 掛載 Core/GameManager.cs (實作 IGameManager, DontDestroyOnLoad)
│   ├── [AudioManager]              <-- 掛載 Audio/AudioManager.cs (實作 IAudioService, 掛載 AudioSource)
│   └── [SceneLoader]               <-- 掛載 Core/SceneLoader.cs (非同步場景換關)
├── XR Origin (XR Rig)              <-- 綁定 Meta Quest XR Origin 組件
│   ├── Camera Offset
│   │   ├── Main Camera             <-- 附帶 ScreenDimmer 與 Subtitle 視圖
│   │   │   ├── [ScreenDimmer]      <-- 掛載 UI/ScreenDimmerController.cs
│   │   │   └── [SubtitleText]      <-- 掛載 UI/SubtitleController.cs (TextMeshPro)
│   │   ├── Left Controller         <-- 掛載 Player/PlayerController.cs (Near-Far Interactor)
│   │   └── Right Controller        <-- 掛載 Player/PlayerController.cs (Near-Far Interactor)
│   └── Locomotion Medium           <-- Snap Turn, Continuous Move, Teleportation Provider
├── [StageSequence]                 <-- 掛載 Managers/StageSequenceController.cs (配置開場/結尾語音與字幕)
├── [LevelManager]                  <-- 掛載該關卡專屬管理器 (如 LevelManager_SensorimotorStage, LevelManager_FormalOperationalStage)
├── Objects & Environment
│   ├── Gate                        <-- 掛載 Controllers/GateController.cs (實作 IBarrierController, 終點通關閘門)
│   ├── Fence                       <-- 掛載 Controllers/FenceController.cs (實作 IBarrierController, 阻斷防護柵欄)
│   ├── ElectricGrid                <-- 掛載 Controllers/ElectricGridController.cs (高壓電網障礙)
│   ├── FinishLine                  <-- 掛載 Controllers/FinishLineController.cs (終點線判定)
│   ├── GrabObjects                 <-- 掛載 XRGrabInteractable + Infrastructure/Interaction/ReturnToAnchor.cs (放手回彈錨點)
│   ├── FloatingDecor               <-- 掛載 Infrastructure/Animation/TransformAnimator.cs (浮動補間動畫)
│   ├── Batteries                   <-- 掛載 Controllers/BatteryController.cs (算式能量電池)
│   ├── BlockChecker                <-- 掛載 Controllers/BlockCheckerController.cs (前運思期積木配對)
│   ├── LightBalls                  <-- 掛載 Controllers/LightBallController.cs + XRSimpleInteractable
│   ├── MazeStructure               <-- 掛載 Managers/SubManagers/MazeManager.cs (依失敗次數動態切換 8 種迷宮)
│   └── PowerQuiz                   <-- 掛載 Managers/SubManagers/PowerQuizManager.cs (等式算式仲裁)
└── Canvas_HUD                      <-- 掛載 UI/HUDController.cs (CanvasGroup 淡入淡出)
```

---

## 專案檔案目錄結構

```text
├── README.md
├── Audio/                            # 音訊與語音播放
│   ├── Interfaces/
│   │   └── IAudioService.cs          # 音訊服務介面契約
│   ├── AudioManager.cs               # 音訊播放與語音快取 (實作 IAudioService)
│   └── SFXList.cs                    # 關卡語音與音效 ScriptableObject
├── Controllers/                      # 場景物件、機關與障礙控制器
│   ├── Interfaces/
│   │   └── IBarrierController.cs     # 通行阻擋物契約 (閘門/柵欄開關狀態)
│   ├── BatteryController.cs          # 算式能量電池 (數值與運算子)
│   ├── BlockCheckerController.cs     # 前運思期積木配對檢驗器
│   ├── ElectricGridController.cs     # 高壓電網障礙觸發器
│   ├── FenceController.cs            # 升降防護柵欄控制器 (實作 IBarrierController)
│   ├── FinishLineController.cs       # 關卡終點線觸發器
│   ├── GateController.cs             # 終點滑動閘門與非同步換關觸發 (實作 IBarrierController)
│   ├── GuideLightBallController.cs   # Limbo 階段光球航點巡航
│   ├── LightBallController.cs        # 感覺動作期音階燈球 (XRSimpleInteractable)
│   └── MazeDoorController.cs         # 形式運思期生路/死路判定門
├── Core/                             # 核心系統與跨場景常駐管理
│   ├── Interfaces/
│   │   └── IGameManager.cs           # 核心遊戲狀態契約
│   ├── GameManager.cs                # 核心狀態與事件發布 (實作 IGameManager)
│   └── SceneLoader.cs                # 非同步場景換關器 (防重複加載)
├── Editor/                           # 編輯器輔助工具
│   └── GenerateHardEdgeOutline.cs    # 拓撲硬邊輪廓網格生成器
├── Infrastructure/                   # 全域基礎設施與互動工具
│   ├── Animation/
│   │   └── TransformAnimator.cs      # 物件上下浮動、自體旋轉與縮放補間
│   ├── EventBus/
│   │   ├── EventBus.cs               # 泛型事件匯流排
│   │   └── Events/
│   │       └── VREvents.cs           # 事件結構體定義
│   └── Interaction/
│       └── ReturnToAnchor.cs         # 放手後平滑回彈錨點 (DOTween)
├── Managers/                         # 關卡流程管理器
│   ├── Interfaces/
│   │   ├── ILevelManager.cs          # 關卡主契約 (DIP)
│   │   └── IStageAnimationService.cs # 過場時序與動畫契約 (DIP)
│   ├── BaseLevelManager.cs           # 萃取共同流程之抽象基類 (Template Method)
│   ├── StageSequenceController.cs    # 通用過場時序控制器 (實作 IStageAnimationService)
│   ├── LevelManager_LimboStage.cs                 # Stage 0: 邊界序章
│   ├── LevelManager_SensorimotorStage.cs          # Stage 1: 感覺動作期 (音階檢驗)
│   ├── LevelManager_PreoperationalStage.cs        # Stage 2: 前運思期 (積木檢驗)
│   ├── LevelManager_ConcreteOperationalStage.cs   # Stage 3: 具體運思期 (算式仲裁)
│   ├── LevelManager_FormalOperationalStage.cs     # Stage 4: 形式運思期 (迷宮與雙結局分支)
│   ├── LevelManager_Ending.cs                     # Stage 5: 結算展示
│   └── SubManagers/                               # 關卡局部解謎管理
│       ├── MazeManager.cs                         # 8 種迷宮格局切換
│       └── PowerQuizManager.cs                    # 能量等式算式平衡仲裁 (升降橋)
├── Player/                           # 玩家與 XR Rig 控制層
│   └── PlayerController.cs           # 移動、轉向、瞬移與射線互動模組開關
├── Shaders/                          # 自定義著色器
│   ├── PureEdgeLine.shader           # 拓撲純淨邊緣輪廓 Shader
│   └── SilhouetteEffect.shader       # 物件剪影與外框 Shader
└── UI/                               # VR 世界空間介面視圖
    ├── HintTextController.cs         # 雙語提示文字視圖
    ├── HUDController.cs              # 抬頭顯示器淡入淡出 (CanvasGroup)
    ├── MainMenuManager.cs            # 首頁語系切換與遊戲開始
    ├── PauseMenuController.cs        # 暫停選單 (Input System 動作綁定)
    ├── ScreenDimmerController.cs     # 全景視野暗角過場控制器
    └── SubtitleController.cs         # 3D 空間雙語字幕控制器
```
