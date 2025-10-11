using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GridPlayer : MonoBehaviour
{
    [Header("Mouvement")]
    [Tooltip("Vitesse vers la case cible (monde).")]
    public float moveSpeed = 8f;

    [Tooltip("Transform visuel (facultatif) à orienter selon la direction.")]
    public Transform visual;

    [Tooltip("Empêcher le déplacement tant que l'on n'a pas atteint la case suivante.")]
    public bool lockUntilReached = true;

    [Header("Options")]
    [Tooltip("Si coché, on appelle WorldGrid.IsWalkable(x,y) avant de valider un pas.")]
    public bool useWalkableCheck = false;

    private PlayerControls controls;
    private Rigidbody rb;
    private WorldGrid grid;           // référence à la grille (singleton ou trouvée)
    private Vector2Int gridPos;       // position logique (centrée autour de 0)
    private Vector3 targetWorldPos;   // centre de la case visée (monde)
    private bool isMoving;

    // Bornes robustes (supporte width/height pairs et impairs)
    int MinX => -(grid.width  - 1) / 2;
    int MaxX =>  (grid.width) / 2;
    int MinY => -(grid.height - 1) / 2;
    int MaxY =>  (grid.height) / 2;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Figer l’axe Y + rotations pour éviter la "pompe" en hauteur
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        controls = new PlayerControls(); // classe auto-générée du New Input System
    }

    void OnEnable()
    {
        // Récupère la grille (singleton ou recherche)
        grid = WorldGrid.Instance != null ? WorldGrid.Instance : FindObjectOfType<WorldGrid>();
        if (grid == null)
        {
            Debug.LogError("[GridPlayer] Aucune WorldGrid active dans la scène.");
            enabled = false;
            return;
        }

        // Abonne l'input (idéalement, configure l'action Move en 'Press (press-only)')
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.Enable();

        // Aligne la position grille depuis la position monde actuelle (snap au centre)
        if (grid.WorldToGridSnap(transform.position, out int gx, out int gy))
            gridPos = new Vector2Int(gx, gy);
        else
            gridPos = Vector2Int.zero;

        // Positionne directement le rigidbody au centre de la case, en conservant le Y courant
        Vector3 start = grid.GridToWorld(gridPos.x, gridPos.y);
        start.y = rb.position.y;
        rb.position = start;
        targetWorldPos = start;
        isMoving = false;
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.Disable();
    }

    // Reçoit un Vector2 (x,y) quand tu appuies sur WASD/ZQSD
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (grid == null) return;

        // Si on veut strictement un pas à la fois
        if (lockUntilReached && isMoving) return;

        Vector2 v = ctx.ReadValue<Vector2>();

        // Discrétise en direction CARDINALE (pas de diagonales)
        int dx = 0, dy = 0;
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            dx = (int)Mathf.Sign(v.x);
        else
            dy = (int)Mathf.Sign(v.y);

        if (dx == 0 && dy == 0) return;

        Vector2Int next = gridPos + new Vector2Int(dx, dy);

        // Bornes
        if (!IsInside(next)) return;

        // (Optionnel) Walkable
        if (useWalkableCheck)
        {
            // Exige que WorldGrid expose IsWalkable(int,int)
            // if (!grid.IsWalkable(next.x, next.y)) return;
        }

        // Valide le pas
        gridPos = next;

        // Nouvelle cible monde (on conserve l'altitude actuelle du rigidbody)
        Vector3 t = grid.GridToWorld(gridPos.x, gridPos.y);
        t.y = rb.position.y;
        targetWorldPos = t;

        isMoving = true;

        // Oriente le visuel si demandé
        if (visual)
        {
            Vector3 fwd = new Vector3(dx, 0f, dy);
            if (fwd.sqrMagnitude > 0f)
                visual.forward = fwd;
        }
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        Vector3 p = Vector3.MoveTowards(rb.position, targetWorldPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(p);

        // Arrêt propre quand on atteint la cible
        if ((p - targetWorldPos).sqrMagnitude < 0.0001f)
        {
            rb.MovePosition(targetWorldPos);
            isMoving = false;
        }
    }

    private bool IsInside(Vector2Int g)
    {
        return g.x >= MinX && g.x <= MaxX && g.y >= MinY && g.y <= MaxY;
    }
}
