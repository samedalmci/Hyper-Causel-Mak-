using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;

    public float runningSpeed;
    private float _currentRunningSpeed;
    public float limitx;
    public float xspeed;

    public GameObject riddingcylinderprefab;
    public List<RidingCylinder> cylinders;

    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;

    private bool _finished;

    private float _scoreTimer = 0;

    public Animator animator;

    private float _dropSoundTimer;

    public AudioSource cylinderAudioSource, triggerAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip, coinAudioClip;

    void Start()
    {
        Current = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.gameActive)
        {
            return;
        }
        float newx = 0;
        float touchxDelta = 0;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {

            touchxDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;


        }
        else if (Input.GetMouseButton(0))
        {

            touchxDelta = Input.GetAxis("Mouse X");

        }

        newx = transform.position.x + xspeed * touchxDelta * Time.deltaTime;
        newx = Mathf.Clamp(newx, -limitx, limitx);

        Vector3 newPosition = new Vector3(newx, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (_spawningBridge)
        {
            PlayerDropSound();

            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementClyindervolume(-0.01f);
                GameObject creatBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.EndReference.transform.position - _bridgeSpawner.StartReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                creatBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.StartReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePozition = _bridgeSpawner.StartReference.transform.position + direction * characterDistance;
                newPiecePozition.x = transform.position.x;
                creatBridgePiece.transform.position = newPiecePozition;

                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.2f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }


    }

    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == ("AddCylinder"))
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip, 0.1f);
            IncrementClyindervolume(0.1f);
            Destroy(other.gameObject);

        }
        else if (other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());

        }
        else if (other.tag == "StopSpawnBridge")
        {
            StopSpawningBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if (other.tag == "Finish")
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            other.tag = "Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.gameActive)
        {
            if (other.tag == ("Trap"))
            {
                PlayerDropSound();

                IncrementClyindervolume(-Time.fixedDeltaTime);
            }
        }

    }

    public void IncrementClyindervolume(float value)
    {
        if (cylinders.Count == 0)
        {
            if (value > 0)
            {
                createCylinder(value);
            }
            else
            {
                if (_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylindervolume(value);
        }

    }

    public void Die()
    {
        animator.SetBool("dead", true);
        gameObject.layer = 8;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();

    }

    public void createCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(riddingcylinderprefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylindervolume(value);

    }

    public void DestroyCyliner(RidingCylinder cylinder)
    {

        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);


    }

    public void StartSpawningBridge(BridgeSpawner spawner)
    {

        _bridgeSpawner = spawner;
        _spawningBridge = true;

    }

    public void StopSpawningBridge()
    {
        _spawningBridge = false;
    }

    public void PlayerDropSound()
    {
        _dropSoundTimer -= Time.deltaTime;
        if (_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);

        }

    }

   

}
