using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;
    public GameObject PauseMenu;

    public Text AltitudeText;

    public AudioClip ShootSound;

    public AudioSource AudioSource;

    [SerializeField]
    float BulletSpeedAtStart, BulletSpeed, DistanceOfChange, ValueOfChange, TimeChange, ReloadTime;

    [SerializeField]
    Transform Girl, GirlStartPosition;
    [SerializeField]
    bool ChangeByDistance = true;

    [SerializeField]
    GameObject BulletPrefab, RobotMuzzle, RobotGunPivot, Robot;

    AudioSource robotAudioSource;

    public int Altitude { get; protected set; }

    
    //Variables added by Connor Simmons:
    public bool platformInvulnerable = false;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BulletSpeed = BulletSpeedAtStart;
        robotAudioSource = Robot.GetComponent<AudioSource>();
        AudioSource = GetComponent<AudioSource>();
        SetBulletSpeedByDist();
        if (!ChangeByDistance) StartCoroutine(SpeedUpTimer());
        StartCoroutine(BulletShooter());
    }

    void Update()
    {
        SetBulletSpeedByDist();
        TurnRobotGun();

        Altitude = Mathf.FloorToInt(Mathf.Max(0, Girl.position.y - GirlStartPosition.position.y));
        AltitudeText.text = Altitude.ToString();

        if (Input.GetButtonDown("Cancel"))
        {
            TogglePauseMenu();
        }
    }

    internal float GetBottomOfScreen()
    {
        var cam = Camera.main;
        return cam.transform.position.y - cam.orthographicSize;
    }
    internal float GetTopOfScreen()
    {
        var cam = Camera.main;
        return cam.transform.position.y + cam.orthographicSize;
    }

    private void TurnRobotGun()
    {
        Vector3 girlToRobot = (Robot.transform.position - Girl.position).normalized;
        float gunAngle = Vector3.Angle(Vector3.up, girlToRobot);
        if (girlToRobot.x > 0) gunAngle *= -1;
        var gunEuler = RobotGunPivot.transform.rotation.eulerAngles;
        gunEuler.z = gunAngle;
        RobotGunPivot.transform.rotation = Quaternion.Euler(gunEuler);
    }

    public void TogglePauseMenu()
    {
        ShowPauseMenu(!PauseMenu.activeSelf);
    }

    public void ShowPauseMenu(bool show)
    {
        PauseMenu.SetActive(show);
        if (show) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    internal float GetBulletSpeed() => BulletSpeed;

    private IEnumerator BulletShooter()
    {
        while (true)
        {
            float height = Mathf.Max(0, Camera.main.transform.position.y / 50);
            float adjustedReloadTime = ReloadTime * 2 / (Mathf.Pow(height, 2) + 2);
            yield return new WaitForSeconds(adjustedReloadTime);
            Shoot();
        }
    }

    private void Shoot()
    {
        robotAudioSource.PlayOneShot(ShootSound);
        var bullet = Instantiate(BulletPrefab, RobotMuzzle.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Velocity = -RobotGunPivot.transform.up * BulletSpeed;
        bullet.transform.up = RobotGunPivot.transform.up;
    }

    

    public IEnumerator SpeedUpTimer()
    {
        while(true){
            BulletSpeed += ValueOfChange;
            yield return new WaitForSeconds(TimeChange);
        }
    }

    private void SetBulletSpeedByDist()
    {
        if (ChangeByDistance)
        {
            var DistanceFromStart = Vector2.Distance(Girl.position, GirlStartPosition.position);
            BulletSpeed = BulletSpeedAtStart + ((int)(DistanceFromStart / DistanceOfChange) * ValueOfChange);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
