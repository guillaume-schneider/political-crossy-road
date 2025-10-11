# 🧠 Unity Project – Git Version Control Setup

This document explains how to properly manage this Unity project using **Git** and **Git LFS**.  
It ensures lightweight commits, consistent asset tracking, and conflict-free collaboration.

---

## 🚀 Project Overview

This Unity project is configured for safe, efficient version control using:
- **Git** for code and asset versioning  
- **Git LFS** (Large File Storage) for binary files (textures, models, sounds, etc.)  
- **Unity Smart Merge** for resolving scene and prefab conflicts automatically  

---

## 🧩 Repository Structure

```
ProjectRoot/
│
├── Assets/                # Game assets, scripts, prefabs, scenes
├── Packages/              # Package dependencies
├── ProjectSettings/       # Unity project settings
├── UserSettings/          # Local-only editor prefs (ignored)
├── .gitignore             # Git ignore rules for Unity
├── .gitattributes         # LFS + Smart Merge rules
└── README.md              # This file
```

---

## ⚙️ Initial Setup

### 1. Clone the Repository
```bash
git clone https://github.com/<your-username>/<your-repo-name>.git
cd <your-repo-name>
```

### 2. Install Git LFS (if not already)
```bash
git lfs install
```

Git LFS automatically tracks large binary assets.  
You can confirm tracking rules with:
```bash
git lfs track
```

---

## 🧰 Unity Project Configuration

### Project Settings → Editor

| Setting | Value |
|----------|--------|
| Version Control | Visible Meta Files |
| Asset Serialization | Force Text |
| Line Endings | OS Native (recommended) |

This ensures Unity assets have `.meta` files (stable GUIDs) and text-based diffs.

---

## 🧹 .gitignore Summary

The repository’s `.gitignore` excludes:
- `Library/`, `Temp/`, `Build/`, `Logs/` (generated or build artifacts)  
- IDE files (`.vs/`, `.idea/`, `.sln`, etc.)  
- OS junk (`.DS_Store`, `Thumbs.db`)  
- User-specific settings  

Only necessary project files (`Assets/`, `ProjectSettings/`, `Packages/`) are committed.

---

## 💾 Git LFS Configuration

Binary files such as textures, audio, and 3D models are tracked via Git LFS:

```bash
git lfs track "*.psd" "*.png" "*.jpg" "*.wav" "*.fbx" "*.mp4" "*.mov"
```

To modify tracking, edit `.gitattributes` and commit the changes.

---

## 🧠 Unity Smart Merge (YAMLMerge)

Unity’s **Smart Merge tool** allows automatic merging of `.unity`, `.prefab`, `.mat`, and `.asset` files.

### 1. Configure Git to use UnityYAMLMerge

**Windows example:**
```bash
git config --global merge.unityyamlmerge.name "Unity Smart Merge (YAML)"
git config --global merge.unityyamlmerge.driver "\"C:/Program Files/Unity/Hub/Editor/<version>/Editor/Data/Tools/UnityYAMLMerge.exe\" merge -p -o \"$MERGED\" \"$BASE\" \"$REMOTE\" \"$LOCAL\""
```

**macOS example:**
```bash
git config --global merge.unityyamlmerge.name "Unity Smart Merge (YAML)"
git config --global merge.unityyamlmerge.driver "/Applications/Unity/Hub/Editor/<version>/Unity.app/Contents/Tools/UnityYAMLMerge merge -p -o \"$MERGED\" \"$BASE\" \"$REMOTE\" \"$LOCAL\""
```

### 2. File types using Smart Merge
Defined in `.gitattributes`:
```gitattributes
*.unity     merge=unityyamlmerge
*.prefab    merge=unityyamlmerge
*.mat       merge=unityyamlmerge
*.anim      merge=unityyamlmerge
*.asset     merge=unityyamlmerge
*.controller merge=unityyamlmerge
```

---

## 🧭 Workflow Guidelines

### Branching
- `main` → Stable, release-ready code  
- `dev` → Integration branch  
- `feat/<feature-name>` → New features or experiments  

### Commit & Push
```bash
git add .
git commit -m "Describe your change"
git pull --rebase
git push
```

### Best Practices
✅ Always pull before working to avoid conflicts  
✅ Commit frequently with meaningful messages  
✅ Don’t commit `Library/` or build folders  
✅ Never delete `.meta` files  
✅ Prefer prefab changes over scene edits  
✅ Coordinate when editing shared scenes  

---

## 🧩 Common Issues & Fixes

| Problem | Cause | Fix |
|----------|--------|-----|
| Repo too large | Large binaries not tracked by LFS | Run `git lfs migrate import --include="*.psd,*.fbx,*.wav"` |
| Missing `.meta` files | Hidden in Unity settings | Set “Visible Meta Files” |
| Scene merge conflicts | Not using text serialization | Enable “Force Text” and Smart Merge |
| Broken library on clone | Cached build data not needed | Delete `Library/` and reopen Unity |

---

## 🤝 Contributing

1. Create a branch:
   ```bash
   git checkout -b feat/your-feature
   ```
2. Commit your changes  
3. Push:
   ```bash
   git push origin feat/your-feature
   ```
4. Open a Pull Request on GitHub

---

## 🧾 License

```
MIT License  
© 2025 Guillaume Schneider
```

---

## 🧩 Recommended Unity Editor Settings (for all collaborators)

### Edit → Project Settings → Editor
- Version Control: **Visible Meta Files**
- Asset Serialization: **Force Text**

### Edit → Preferences → External Tools
- Link your preferred IDE (Visual Studio, Rider, VS Code)

### Edit → Preferences → General
- Enable **Auto Refresh** to detect Git changes automatically  
