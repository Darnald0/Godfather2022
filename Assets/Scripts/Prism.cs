using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct exit {
    public Vector3 position;
    public float angle;
}
public class Prism : MonoBehaviour
{
    [SerializeField] private List<exit> exitsList = new List<exit>();

    [SerializeField] private Projectile beamPrefab;
    [SerializeField] private UnityEvent onPrismHit;

    private AudioSource audioSource;

    void Start() {
        onPrismHit.AddListener(PrismHit);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            TestSpawn();
        }
    }

    private void TestSpawn() {
        for (int i = 0; i < exitsList.Count; i++) {
            Spawn(beamPrefab, exitsList[i].angle, exitsList[i].position);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (collision.gameObject.tag == "Beam" && !projectile.isFromPrism) {
            for (int i = 0; i < exitsList.Count; i++) {
                Spawn(beamPrefab, exitsList[i].angle, exitsList[i].position + transform.position);
            }

            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            onPrismHit.Invoke();
            StartCoroutine(Destruction(collision.gameObject, collision.gameObject.GetComponent<TrailRenderer>().time));
        }
    }

    static public void Spawn(Projectile prefab, float direction, Vector3 position) {
        Projectile newProjectile = Instantiate(prefab);
        newProjectile.isFromPrism = true;
        newProjectile.transform.position = position;
        newProjectile.angleDirection = direction;
    }

    private void PrismHit() {
        audioSource.PlayOneShot(audioSource.clip);
    }

    IEnumerator Destruction(GameObject beam, float duration) {
        yield return new WaitForSeconds(duration);
        Destroy(beam);
        Destroy(gameObject);
        yield return null;
    }
}
