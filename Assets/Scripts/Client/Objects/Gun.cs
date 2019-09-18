using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float coolDown;    //! Enter Values in Denominator to 1
    [SerializeField] private int clipSize;
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime;

    [Header("Components")]
    [SerializeField]private Transform spawnPoint;
    [SerializeField]private Bullet bulletPrefab;

    //!Private Variables
    private int currentBullets = 0;
    private int bulletsLeft;

    private float timer;

    void Start()
    {
        bulletsLeft = magazineSize;
        Reload();
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Fire()
    {
        if (currentBullets > 0)
        {
            if (timer > 0)
                return;
            Bullet b = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation) as Bullet;
            b.Launch();
            currentBullets -= 1;
            bulletsLeft -= 1;
            timer = coolDown;
        }
        else
        {
            Reload();
        }
    }

    public void Reload()
    {
        timer = reloadTime;
        Debug.Log("Reloading...");
        currentBullets = (bulletsLeft > clipSize) ? clipSize : bulletsLeft;
    }
}