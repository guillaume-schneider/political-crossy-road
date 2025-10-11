# 🧭 Developing Guide (Unity + Git)

A practical, opinionated guide for day‑to‑day development on this project.  
If you follow this, you’ll ship faster, avoid merge hell, and keep the repo healthy.

> Pair this with **README.md** (project setup) and **CONTRIBUTING.md** (Git standards).

---

## 0) TL;DR Daily Flow

1. **Sync**  
   ```bash
   git checkout develop   # or main if no develop branch
   git pull --rebase
   git checkout -b feature/<short-name>
   ```
2. **Develop** in Unity (see conventions below).  
3. **Self‑test** (PlayMode + EditMode tests).  
4. **Commit** small, meaningful chunks using conventional commits.  
5. **Push & PR** to `develop` (or `main` if single branch).  
6. **Fix review notes**, re‑test, merge via PR (squash or merge).  
7. **Delete the feature branch** after merge.

---

## 1) Prerequisites

- **Unity**: Use the project’s recommended LTS (set in `ProjectVersion.txt`).  
- **IDE**: Rider / Visual Studio / VS Code with C# support.  
- **Git LFS** installed: `git lfs install`  
- **Git** configured for Unity Smart Merge (UnityYAMLMerge).

---

## 2) Project Structure (conventions)

```
Assets/
  _Project/
    Scripts/
      Runtime/
      Editor/
      Tests/
    Art/
    Audio/
    Materials/
    Prefabs/
    Scenes/
      Bootstrap/
      Gameplay/
      UI/
    Settings/
  Plugins/
  ThirdParty/
Packages/
ProjectSettings/
```

**Rules**
- Keep **project code** in `Assets/_Project/…` to separate from third‑party.  
- One **scene per responsibility**; break big scenes into **additive** sub‑scenes.  
- Prefer **Prefabs** + **ScriptableObjects** for data/config over scene‑only changes.  
- All assets keep their **.meta** files; never rename without Unity.

---

## 3) Coding Standards (C#)

- **Namespaces**: `Company.Project.Feature` (or `Project.Feature`).  
- **Files**: one class per file; file name == class name.  
- **Naming**: `PascalCase` for types/methods, `camelCase` for locals/fields, `_camelCase` for private serialized fields.  
- **SerializeField** private by default:  
  ```csharp
  [SerializeField] private float _speed = 5f;
  public float Speed => _speed;
  ```
- **MonoBehaviours** do orchestration; put domain logic in **plain C# classes**.  
- Prefer **events**/**UnityEvents** over `FindObjectOfType`. Use **DI/service locators** sparingly.  
- Avoid **Update()** unless needed—prefer events, coroutines, or fixed‑rate tickers.  
- Use **ScriptableObject** for tunable data and runtime config.  
- **Null‑safety**: guard serialized refs in `Awake()`/`OnEnable()`.  
- **Logs**: use `UnityEngine.Logger` or a wrapper; no noisy logs in production builds.  
- **Tests**: add **EditMode** tests for pure logic and **PlayMode** for integration.

---

## 4) Scenes, Prefabs & Assets

- **Scenes**  
  - `Assets/_Project/Scenes/Bootstrap/Bootstrap.unity` is the entry scene.  
  - Gameplay split into **additive** scenes: `Environment`, `Gameplay`, `UI`.  
- **Prefabs**  
  - Prefer **Prefab Variants** for customization.  
  - Keep prefabs small and composable; avoid deep hierarchies.  
- **Addressables (if used)**  
  - Group by feature; clear labels; avoid giant bundles.  
- **Art/Audio**  
  - Track large binaries with **Git LFS** (`psd`, `png`, `fbx`, `wav`, `mp4`, etc.).  
  - Keep source files (e.g., `.psd`) when feasible.

---

## 5) Git Workflow Essentials

- Branch from `develop` (or `main`):  
  ```bash
  git checkout -b feature/<name> develop
  ```
- Commit format (Conventional Commits):
  ```
  feat(input): add gamepad vibration
  fix(vr): correct head rotation on snap turn
  refactor(ai): split behaviour tree nodes
  docs(readme): clarify setup
  ```
- Sync often: `git pull --rebase` before pushes.  
- Resolve YAML merges using **UnityYAMLMerge**; avoid committing `Library/` etc.  
- Open a **PR** with a clear description, screenshots/gifs if UI/visual.

---

## 6) Testing

- **EditMode** tests: fast unit tests for pure logic (`Assets/_Project/Scripts/Tests/EditMode`).  
- **PlayMode** tests: scene‑level behaviour and integration (`Tests/PlayMode`).  
- **Guidelines**
  - One **Arrange/Act/Assert** per test.  
  - Avoid timing‑sensitive assertions; use `yield return` and explicit waits.  
  - Run tests in CI (if configured).

Sample EditMode test:
```csharp
using NUnit.Framework;

public class HealthTests
{
    [Test]
    public void Damage_ReducesHealth_ToMinimumZero()
    {
        var h = new Health(100);
        h.Damage(150);
        Assert.AreEqual(0, h.Value);
    }
}
```

---

## 7) Debugging & Profiling

- Use **Profiler** and **Deep Profile** sparingly; capture short windows.  
- **Frame Debugger** for render issues; check draw calls and overdraw.  
- **Memory Profiler** (package) for leaks and large textures/audio.  
- Use **Development Build** + **Autoconnect Profiler** on device.

---

## 8) Build & Release

- **Build targets** stored as **Build Profiles** (scripted or via `BuildSettings`).  
- Output to `Builds/<platform>/<version>/`.  
- **Versioning**: Semantic (`v<major>.<minor>.<patch>`).  
- Tag releases:
  ```bash
  git tag -a v1.0.0 -m "Initial release"
  git push origin v1.0.0
  ```
- Keep a `CHANGELOG.md` (optional: generated from commits).

---

## 9) Code Review Checklist (PR author)

- [ ] Branch up to date (`git pull --rebase`)  
- [ ] Builds locally, no console errors/warnings  
- [ ] Tests added/updated and passing  
- [ ] No changes in `ProjectSettings` or `Packages` unless intentional  
- [ ] Scenes and prefabs open without missing references  
- [ ] `.meta` files present; no `Library/`, `Temp/`, `Build/` committed  
- [ ] Docs updated (README/CONTRIBUTING if needed)  
- [ ] Screenshots/gifs for visual changes

---

## 10) Troubleshooting

**“Non‑fast‑forward” on push**  
- Run `git pull --rebase origin <branch>` then push.

**Binary bloat / giant repo**  
- Ensure LFS is tracking: `git lfs track`  
- Optionally migrate history:  
  `git lfs migrate import --include="*.psd,*.fbx,*.wav,*.png,*.mp4"`

**Scene merge conflicts**  
- Confirm `Force Text` + UnityYAMLMerge config.  
- Break scene into smaller additive scenes and use prefabs.

**Missing .meta files / broken links**  
- Set **Visible Meta Files**; reimport; avoid OS‑level renames.

---

## 11) Useful Commands (cheat sheet)

```bash
# start a feature
git checkout develop && git pull --rebase
git checkout -b feature/my-feature

# stage, commit, push
git add .
git commit -m "feat(core): add health system"
git push -u origin feature/my-feature

# update branch
git pull --rebase

# set upstream on first push (global convenience)
git config --global push.autoSetupRemote true

# create a tag
git tag -a v1.2.0 -m "QoL update"
git push origin v1.2.0
```

---

## 12) Quality Bars

- **No red console** in editor or build.  
- **< 100 ms** main thread spikes (target platform).  
- **Tests** exist for logic that can break silently.  
- **Docs** updated for any contributor‑facing change.

---

**Build thoughtfully. Test early. Commit small. Ship often. 🚀**
