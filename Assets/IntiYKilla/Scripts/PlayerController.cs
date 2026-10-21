using System.Collections.Generic;
using UnityEngine;

namespace IntiYKilla
{
    // Sisa, la pastora. Corre, salta (más alto si se mantiene la tecla),
    // cambia entre día y noche (Q), interactúa (E), lanza piedras con su huaraca (F)
    // y reaparece al caer.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public float runSpeed = 7f;
        public float acceleration = 60f;
        public float jumpVelocity = 13f;
        public float riseGravity = 3.2f;
        public float fallGravity = 5f;
        public float maxFallSpeed = 18f;
        public float coyoteTime = 0.1f;
        public float jumpBuffer = 0.12f;
        public float killY = -7f;

        public Vector2 RespawnPoint { get; set; }
        public bool InputEnabled { get; set; }
        public int Facing { get; private set; } = 1;
        public bool Grounded => grounded;
        public int Deaths { get; private set; }

        // Historial de posiciones: la llama camina siguiendo el mismo recorrido.
        public readonly List<Vector3> Trail = new List<Vector3>();

        public float attackCooldown = 0.35f;

        Rigidbody2D rb;
        [SerializeField] SpriteRenderer sr;
        [SerializeField] Sprite idle, walk;
        float attackLeft;
        bool grounded, groundContact;
        float coyoteLeft, bufferLeft, walkAnim, invulnerable, squash, lastFallSpeed;
        bool jumpHeld;
        float moveInput, vertInput;

        // Trepar enredaderas (Bosque).
        public float climbSpeed = 4.5f;
        bool climbing, onVine, vineTouch, jumpOffVine;
        float vineX;
        public bool Climbing => climbing;

        // La enredadera llama a esto mientras Sisa la toca y está crecida.
        public void TouchVine(float x)
        {
            vineTouch = true;
            vineX = x;
        }

        Vector2 Velocity
        {
#if UNITY_6000_0_OR_NEWER
            get => rb.linearVelocity;
            set => rb.linearVelocity = value;
#else
            get => rb.velocity;
            set => rb.velocity = value;
#endif
        }

        public void Init(SpriteRenderer renderer)
        {
            sr = renderer;
            idle = SpriteFactory.SisaIdleSprite();
            walk = SpriteFactory.SisaWalkSprite();
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            RespawnPoint = transform.position;
            if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
            if (idle == null) idle = SpriteFactory.SisaIdleSprite();
            if (walk == null) walk = SpriteFactory.SisaWalkSprite();
            // Sin fricción para que Sisa no se quede pegada a las paredes.
            var col = GetComponent<Collider2D>();
            if (col != null && col.sharedMaterial == null)
                col.sharedMaterial = new PhysicsMaterial2D("SinFriccion") { friction = 0f, bounciness = 0f };
        }

        public float FallingSpeed => Velocity.y;

        // Rebote al aplastar a un enemigo.
        public void Bounce()
        {
            var v = Velocity;
            v.y = jumpVelocity * 0.75f;
            Velocity = v;
            squash = -0.2f;
        }

        void Update()
        {
            moveInput = 0f;
            vertInput = 0f;
            if (InputEnabled)
            {
                moveInput = GameInput.Horizontal();
                vertInput = GameInput.Vertical();
                if (onVine && !climbing && vertInput > 0.3f)
                {
                    climbing = true;
                    bufferLeft = 0f;
                }
                if (climbing)
                {
                    // En la enredadera, W/arriba sirve para trepar; Espacio para soltarse saltando.
                    if (GameInput.JumpButtonPressed()) jumpOffVine = true;
                }
                else if (GameInput.JumpPressed()) bufferLeft = jumpBuffer;
                jumpHeld = GameInput.JumpHeld();
                if (GameInput.SwitchPressed() && DayNightManager.Instance != null)
                    DayNightManager.Instance.TrySwitch();
                if (GameInput.RestartPressed()) Die(true);
                attackLeft -= Time.deltaTime;
                if (GameInput.AttackPressed() && attackLeft <= 0f && !climbing)
                {
                    attackLeft = attackCooldown;
                    SlingStone.Throw(transform.position + new Vector3(Facing * 0.5f, 0.6f, 0f), Facing);
                    Sfx.Sling();
                    squash = 0.12f;
                }
            }
            else jumpHeld = false;

            if (moveInput > 0.1f) Facing = 1;
            else if (moveInput < -0.1f) Facing = -1;

            // Animación simple de caminar: alterna dos cuadros.
            if (sr != null)
            {
                sr.flipX = Facing < 0;
                bool moving = (Mathf.Abs(Velocity.x) > 0.5f && grounded) || (climbing && Mathf.Abs(Velocity.y) > 0.3f);
                walkAnim = moving ? walkAnim + Time.deltaTime * 8f : 0f;
                sr.sprite = moving ? (((int)walkAnim % 2 == 1) ? walk : idle) : (!grounded && !climbing ? walk : idle);
                sr.enabled = !(invulnerable > 0f && Mathf.Sin(Time.time * 40f) < 0f);
                // Estirar al saltar y aplastar al caer, para que el movimiento se sienta vivo.
                squash = Mathf.MoveTowards(squash, 0f, Time.deltaTime * 1.5f);
                sr.transform.localScale = new Vector3(1f + squash, 1f - squash, 1f);
            }

            invulnerable = Mathf.Max(0f, invulnerable - Time.deltaTime);
            if (transform.position.y < killY) Die(true);

            Trail.Add(transform.position);
            if (Trail.Count > 240) Trail.RemoveAt(0);
        }

        void FixedUpdate()
        {
            bool wasGrounded = grounded;
            grounded = groundContact;
            if (grounded && !wasGrounded && lastFallSpeed < -6f) squash = 0.22f;
            lastFallSpeed = Velocity.y;
            groundContact = false;

            float dt = Time.fixedDeltaTime;
            coyoteLeft = grounded ? coyoteTime : coyoteLeft - dt;
            bufferLeft -= dt;

            onVine = vineTouch;
            vineTouch = false;
            var v = Velocity;

            if (climbing)
            {
                if (jumpOffVine)
                {
                    climbing = false;
                    jumpOffVine = false;
                    v = new Vector2(moveInput * runSpeed, jumpVelocity * 0.85f);
                    squash = -0.25f;
                    Sfx.Jump();
                    rb.gravityScale = riseGravity;
                    Velocity = v;
                    return;
                }
                if (!onVine)
                {
                    // Se acabó la enredadera (o se secó al anochecer).
                    climbing = false;
                    if (vertInput > 0.3f) v.y = 7f;
                }
                else
                {
                    rb.gravityScale = 0f;
                    v.y = vertInput * climbSpeed;
                    v.x = moveInput * runSpeed * 0.45f + (vineX - transform.position.x) * (Mathf.Abs(moveInput) > 0.1f ? 0f : 6f);
                    coyoteLeft = 0f;
                    Velocity = v;
                    return;
                }
            }
            jumpOffVine = false;

            v.x = Mathf.MoveTowards(v.x, moveInput * runSpeed, acceleration * dt);

            if (bufferLeft > 0f && coyoteLeft > 0f)
            {
                v.y = jumpVelocity;
                squash = -0.25f;
                Sfx.Jump();
                bufferLeft = 0f;
                coyoteLeft = 0f;
                grounded = false;
            }

            // Salto variable: si se suelta la tecla mientras sube, el salto se corta.
            if (v.y > 0f && !jumpHeld) v.y *= 0.55f;

            rb.gravityScale = v.y > 0f ? riseGravity : fallGravity;
            if (v.y < -maxFallSpeed) v.y = -maxFallSpeed;
            Velocity = v;
        }

        void OnCollisionStay2D(Collision2D col) => CheckGround(col);
        void OnCollisionEnter2D(Collision2D col) => CheckGround(col);

        void CheckGround(Collision2D col)
        {
            for (int i = 0; i < col.contactCount; i++)
                if (col.GetContact(i).normal.y > 0.6f) groundContact = true;
        }

        // Fallar sin castigo fuerte: vuelve al último punto seguro.
        public void Die(bool force = false)
        {
            if (invulnerable > 0f && !force) return;
            Deaths++;
            Sfx.Hurt();
            climbing = false;
            transform.position = RespawnPoint;
            Velocity = Vector2.zero;
            invulnerable = 1.2f;
            Trail.Clear();
            GameManager.Instance?.OnPlayerRespawn();
        }

        public bool IsInvulnerable => invulnerable > 0f;
    }
}
