using UnityEngine;

public class Sniper : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LineRenderer laserLine;
    public Transform aimOrigin;

    [Header("Sniper Settings")]
    public float range = 50f;
    public float knockbackForce = 5f;
    public float cooldown = 3f;
    

    [Header("Scanning")]
    public float scanAngle = 45f;
    public float scanSpeed = 1f;

    [Header("Aiming")]
    public float aimDelay = 1.2f;
    public float lostTargetCooldown = 2f;

    [Header("Fire Timing")]
    public float aimConeAngle = 5f;
    public float fireDelay = 0.6f;

    private bool firingPending;
    private float fireDelayTimer;


    [Header("Fire Conditions")]
     // Угол, при котором можно стрелять (чем меньше — тем точнее нужно навестись)

    public Transform playerCamera; // ← перетащишь сюда Main Camera в инспекторе

    public Transform laserDot;



    private bool trackingPlayer;
    private bool aimingStarted;
    private float timePlayerSpotted;
    private float timePlayerLost;
    private float lastShotTime;

    private float currentScanAngle; // текущий угол сканирования (в градусах)
    private int scanDirection = 1; // 1 или -1, для смены направления


    private Vector3 currentLaserDirection;
    private Vector3 lockedDirection; // для телеграфа

    private float scanTime;

    private Vector3 interpolatedDirection;
    public float laserInterpolationSpeed = 3f; // настраиваемая скорость поворота линии


    private void Start()
    {
        interpolatedDirection = transform.forward;

        currentLaserDirection = transform.forward;
    }

    private void Update()
    {
        Vector3 origin = aimOrigin.position;
        Vector3 cameraTarget = playerCamera.position;
        Vector3 directionToPlayer = cameraTarget - origin; // используем камеру как цель



        bool playerInSight = false;

        if (Physics.Raycast(origin, directionToPlayer.normalized, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Player"))
            {
                playerInSight = true;
            }
        }

        // 🧠 Обработка логики переключения состояния
        if (playerInSight)
        {
            if (!trackingPlayer)
            {
                // Игрок замечен впервые
                trackingPlayer = true;
                aimingStarted = false;
                timePlayerSpotted = Time.time;
                lockedDirection = transform.forward;
            }
        }
        else
        {
            if (trackingPlayer && Time.time - timePlayerSpotted > aimDelay + lostTargetCooldown)
            {
                trackingPlayer = false;
                aimingStarted = false;
            }
        }

        // 🔁 Что делаем в зависимости от состояния
        if (trackingPlayer)
        {
            float timeSinceSpotted = Time.time - timePlayerSpotted;

            if (timeSinceSpotted >= aimDelay)
            {
                aimingStarted = true;
            }

            if (aimingStarted)
            {
                RotateTowards(directionToPlayer);
                currentLaserDirection = directionToPlayer.normalized;

                float angleToPlayer = Vector3.Angle(interpolatedDirection, directionToPlayer.normalized);

// Условия: наведение завершено, игрок виден, перезарядка пройдена
                if (aimingStarted && playerInSight && Time.time - lastShotTime >= cooldown)
                {
                    if (angleToPlayer < aimConeAngle)
                    {
                        if (!firingPending)
                        {
                            // Лазер навёлся — начинаем задержку перед выстрелом
                            firingPending = true;
                            fireDelayTimer = fireDelay;
                        }
                        else
                        {
                            fireDelayTimer -= Time.deltaTime;

                            if (fireDelayTimer <= 0f)
                            {
                                // ВЫСТРЕЛ
                                KnockbackRelay relay = player.GetComponentInChildren<KnockbackRelay>();
                                if (relay != null)
                                {
                                    relay.ApplyKnockback(directionToPlayer, knockbackForce);
                                    lastShotTime = Time.time;
                                    firingPending = false;
                                }
                            }       
                        }
                    }
                    else
                    {
                        // Цель ушла с прицела — сбрасываем выстрел
                        firingPending = false;
                    }
                }
                else
                {
                    // Либо не видим игрока, либо не прицелились — сброс
                    firingPending = false;
                }
            }
            else
            {
                // Пока идёт задержка — лазер смотрит туда, где снайпер смотрел в момент обнаружения
                currentLaserDirection = lockedDirection;
            }
        }
        else
        {
            ScanArea();
        }

        // Интерполяция направления лазера к текущей цели
        interpolatedDirection = Vector3.Slerp(interpolatedDirection, currentLaserDirection, Time.deltaTime * laserInterpolationSpeed);

        // Обновление лазера
        UpdateLaser(interpolatedDirection);
        
    }

    void RotateTowards(Vector3 direction)
    {
        direction.y = 0;
        if (direction == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    void ScanArea()
    {
        // Плавное вращение угла туда-сюда
        float angleStep = scanSpeed * Time.deltaTime * 90f; // 90 — максимальная скорость (настраиваемая)
        currentScanAngle += angleStep * scanDirection;

        if (currentScanAngle >= scanAngle)
        {
            currentScanAngle = scanAngle;
            scanDirection = -1;
        }
        else if (currentScanAngle <= -scanAngle)
        {
            currentScanAngle = -scanAngle;
            scanDirection = 1;
        }

        transform.rotation = Quaternion.Euler(0f, currentScanAngle, 0f);
        currentLaserDirection = transform.forward;
    }

    void UpdateLaser(Vector3 direction)
    {
        if (!laserLine || !aimOrigin) return;

        Vector3 origin = aimOrigin.position;

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, range))
        {
            laserLine.SetPosition(0, origin);
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(0, origin);
            laserLine.SetPosition(1, origin + direction.normalized * range);
        }

        if (laserDot != null)
        {
            laserDot.position = hit.point + hit.normal * 0.01f;
            laserDot.forward = -hit.normal; // чтобы смотрел в сторону поверхности
        }

    }
}
