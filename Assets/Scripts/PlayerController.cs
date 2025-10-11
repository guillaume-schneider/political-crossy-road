using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GridPlayer : MonoBehaviour
{
    public float moveSpeed = 8f;        // vitesse vers la case cible (monde)
    public Transform visual;            // optionnel: modèle à déplacer/faire tourner

    private PlayerControls controls;
    private Rigidbody rb;

    // Position logique dans la grille (coordonnées CENTRÉES: ..., -1, 0, +1, ...)
    private Vector2Int gridPos;

    // Cible monde actuelle
    private Vector3 targetWorldPos;
    private bool isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints |= RigidbodyConstraints.FreezeRotation; // on bouge sur XZ uniquement

        controls = new PlayerControls();
    }

    void OnEnable()
    {
        // IMPORTANT: configure ton action "Move" en "PassThrough/Value(Vector2)" + interaction "Press (press-only)"
        // pour ne recevoir qu'un event par appui. Sinon, on gère ça côté code (ci-dessous).
        controls.Player.Move.performed += OnMove;   // déclenché à l'appui
        controls.Player.Move.Enable();

        // Initialise la position grille depuis la position monde actuelle (snap)
        if (WorldGrid.Instance.WorldToGridSnap(transform.position, out int gx, out int gy))
            gridPos = new Vector2Int(gx, gy);
        else
            gridPos = Vector2Int.zero; // fallback

        targetWorldPos = WorldGrid.Instance.GridToWorld(gridPos.x, gridPos.y);
        rb.position = targetWorldPos;
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.Disable();
    }

    // Reçoit un Vector2 (x,y) depuis WASD/ZQSD. On le convertit en pas discrêt (-1,0,+1)
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (isMoving) return; // on ignore tant que la case précédente n'est pas atteinte

        Vector2 v = ctx.ReadValue<Vector2>();

        // Discrétise : on ne garde QUE une direction cardinale (pas de diagonale)
        int dx = Mathf.Abs(v.x) > Mathf.Abs(v.y) ? (int)Mathf.Sign(v.x) : 0;
        int dy = (dx == 0) ? (int)Mathf.Sign(v.y) : 0;

        if (dx == 0 && dy == 0) return;

        Vector2Int next = gridPos + new Vector2Int(dx, dy);

        // Optionnel: bornes & walkable
        if (!IsInside(next)) return;
        // if (!WorldGrid.Instance.IsWalkable(next.x, next.y)) return;

        gridPos = next;
        targetWorldPos = WorldGrid.Instance.GridToWorld(gridPos.x, gridPos.y);
        isMoving = true;

        // Optionnel: orienter le visuel
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

        if ((p - targetWorldPos).sqrMagnitude < 0.0001f)
        {
            rb.MovePosition(targetWorldPos);
            isMoving = false;
        }
    }

    private bool IsInside(Vector2Int g)
    {
        // Adapté au référentiel CENTRÉ (coords négatives/positives)
        int halfW = WorldGrid.Instance.width / 2;
        int halfH = WorldGrid.Instance.height / 2;
        return g.x >= -halfW && g.x <= halfW && g.y >= -halfH && g.y <= halfH;
    }
}
