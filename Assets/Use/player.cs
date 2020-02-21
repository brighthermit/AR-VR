using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    public Rigidbody rigid;
    public float forceMove = 4f;
    public float forceJump = 3f;
    public bool onFloor = false;
    public GameObject coinPrefab;
    public GameObject[] coinArray;
    public int coinArraySize = 3;
    public TMPro.TextMeshProUGUI tmp;
    public int score = 0;
    public TMPro.TextMeshProUGUI tmpTimer;
    public float timer = 10;
    public Animator animator;
    public AudioClip clipCoinCollect;
    public AudioClip clipCoinRespawn;
    private AudioSource audioSource;
    public AudioClip clipOnFloor;
    public TMPro.TextMeshProUGUI tmpResult;
    public int scoreTarget =0;
    public RectTransform loseOrWon;

    public float rany = 3f;
    public float ranyPos = 1f;
    public float ranx = 3f;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        coinArray = new GameObject[coinArraySize];
        for(int i = 0; i < coinArraySize; i++)
        {
           
            coinArray[i] = Instantiate(coinPrefab, RandomizePos(), coinPrefab.transform.rotation);

        }
        
  
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = "Score: " + score;
        tmpTimer.text = " Timer: " + Mathf.Round(timer);
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            tmpTimer.text = "Game Over";



            StartCoroutine(Done());
            StartCoroutine(LoadScene());


        }

        if (Input.GetKey(KeyCode.A))
        {
            rigid.AddForce(new Vector3(-1f * forceMove, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigid.AddForce(new Vector3(1f * forceMove, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.Space) && onFloor)
        {
            rigid.AddForce(new Vector3(0f, 1f * forceJump, 0f), ForceMode.Impulse);
        }
    }
    public bool wonlose
    {
        get
        {
            if (score < scoreTarget)
                return false;
            return true;
        }
    }
    private void OnGameOver()
    {
        loseOrWon.gameObject.SetActive(true);
        tmpResult.text = wonlose ? "You Won" : "You Lose";
    }

    IEnumerator Done()
    {
        animator.SetBool("yeah", true);
        OnGameOver();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("SampleScene");
    }
    IEnumerator LoadScene()
    {
        animator.SetBool("yeah",true);
        rigid.isKinematic = true;
        //yield return new WaitForSeconds(1f);
        //SceneManager.LoadScene("newscene");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SampleScene");
    }

    private Vector3 RandomizePos()
    {
        Vector3 tempPos = new Vector3(Random.Range(-ranx, ranx), rany + Random.Range(-ranyPos, ranyPos), 0f);
        return tempPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            onFloor = true;
            audioSource.PlayOneShot(clipOnFloor);
        }
    }
    private void OnCollisionExit(Collision collision)
     {
         if (collision.gameObject.tag == "Floor")
         {
             onFloor = false;
         }
     }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin") {
            Debug.Log("Coin collect");
            score++;
            StartCoroutine(RespawnCoin(other.gameObject));
        }
    }

    IEnumerator RespawnCoin(GameObject coin)
    {
        coin.SetActive(false);
        audioSource.PlayOneShot(clipCoinCollect);
        yield return new WaitForSeconds(3f);

        coin.transform.position = RandomizePos();
        coin.SetActive(true);
        audioSource.PlayOneShot(clipCoinRespawn);
    }
}
