# 🧭 Git & Branching Standards

This document defines the **Git workflow and branch naming conventions** used in this project.  
It ensures clean collaboration, readable history, and consistent release management.

---

## 1️⃣ Core Branch Structure

| Branch | Purpose |
|---------|----------|
| `main` | Production-ready, release code |
| `develop` | Integration branch for stable tested features |
| `feature/*` | New features or improvements |
| `fix/*` | Non-urgent bug fixes |
| `hotfix/*` | Urgent production fixes |
| `release/*` | Pre-release stabilization and version prep |

---

## 2️⃣ Naming Conventions

Use lowercase, hyphen-separated branch names with a clear prefix:

```
feature/<description>
fix/<description>
hotfix/<description>
release/<version>
experiment/<description>
```

**Examples:**
```
feature/vr-visualisation
fix/ui-button-interaction
hotfix/scene-loading-bug
release/v1.0.0
```

---

## 3️⃣ Commit Message Convention

Format:
```
<type>(<scope>): <short description>
```

| Type | Meaning |
|------|----------|
| `feat` | New feature |
| `fix` | Bug fix |
| `refactor` | Code cleanup or optimization |
| `style` | Non-functional changes (formatting, naming) |
| `docs` | Documentation updates |
| `test` | Adding or updating tests |
| `build` | Build or dependency changes |
| `chore` | Maintenance (no production change) |

**Examples:**
```
feat(audio): add spatial sound support
fix(vr): correct camera rotation
refactor(player): simplify movement code
docs(readme): update usage section
```

---

## 4️⃣ Branch Workflow

1. **Create a new branch** from `develop` (or `main` if `develop` is not used):
   ```bash
   git checkout -b feature/vr-visualisation develop
   ```

2. **Work, commit, and push** your branch:
   ```bash
   git add .
   git commit -m "feat(vr): implement first visualisation"
   git push -u origin feature/vr-visualisation
   ```

3. **Open a Pull Request (PR)** on GitHub for review.  
   Merge only after code review and testing.

4. **Merge target:**
   - `feature/*` → `develop`
   - `fix/*` → `develop`
   - `hotfix/*` → `main` (then merge back to `develop`)
   - `release/*` → both `main` and `develop`

---

## 5️⃣ Merge Strategy

| Strategy | Usage |
|-----------|--------|
| **Pull Request (Merge)** | Recommended for teams. Keeps full history. |
| **Squash and Merge** | Clean history (one commit per feature). |
| **Rebase** | For solo or advanced workflows (linear history). |
| **Fast-forward** | When the branch has no divergence. |

**Recommended:** use Pull Requests + Squash Merge for clarity.

---

## 6️⃣ Tags and Versioning

Use **Semantic Versioning**:
```
v<major>.<minor>.<patch>
```

**Examples:**
```
v1.0.0   → first stable release
v1.1.0   → feature update
v1.1.1   → patch fix
```

Tag creation:
```bash
git tag -a v1.0.0 -m "Initial release"
git push origin v1.0.0
```

---

## 7️⃣ Best Practices

✅ Always pull before starting work (`git pull --rebase`)  
✅ Commit often, push regularly  
✅ Never commit generated folders (`Library/`, `Build/`, etc.)  
✅ Never delete `.meta` files in Unity  
✅ Use meaningful branch names and commit messages  
✅ Prefer feature branches over direct commits to `main`  

---

## ✅ TL;DR Cheat Sheet

| Element | Standard | Example |
|----------|-----------|----------|
| Default branch | `main` | — |
| Integration branch | `develop` | — |
| Feature branch | `feature/<name>` | `feature/vr-visualisation` |
| Bug fix | `fix/<name>` | `fix/ui-overlap` |
| Hotfix | `hotfix/<name>` | `hotfix/startup-crash` |
| Commit format | `<type>(<scope>): <desc>` | `feat(vr): add teleportation` |
| Tag format | `v<major>.<minor>.<patch>` | `v1.0.2` |

---

**Clean Git, clean mind 🧘 — keep history readable and merges painless.**
