using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [HideInInspector] public bool wasThrown;
    public float maxSpeed = 10.0f;
    public float speed = 0.25f;
    public float velocity = 3.0f;
    public int armor = 10;
    public int damage = 1;
    public int health = 100;
    public GameObject particle;

    private Animator animator;
    private bool isAttacking;
    private ParticleSystem coinParticle;
    private Rigidbody rigidBody;
    private Vector2 movement;
    private Vector3 startPosition;

    private const int MAX_ARMOR = 100;

    void Awake() {
        animator = GetComponent<Animator>();
        animator.enabled = true;
        isAttacking = false;
        rigidBody = GetComponent<Rigidbody>();
        wasThrown = false;
    }

    void FixedUpdate() {
        if (isAttacking == false) {
            Move();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bounce") {
            rigidBody.freezeRotation = true;
        }

        if (collision.gameObject.tag == "CatchAll") {
            LoseHitPoints(health);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Castle" && wasThrown == false) {
            StartAttack();
            InvokeRepeating("Attack", 0, speed);
        }

        if (other.tag == "Boundary" && wasThrown) {
            rigidBody.useGravity = false;
            rigidBody.velocity = new Vector3(0, 0, 0);
            coinParticle = Instantiate(particle, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            coinParticle.transform.SetParent(transform);
            coinParticle.Play();
            LoseHitPoints(health);
        }
    }

    void OnTriggerExit(Collider other) {
        if (coinParticle) {
            coinParticle.Stop();
        }
        CancelInvoke("Attack");
    }

    private void OnMouseDown() {
        if (wasThrown == false) {
            Vector3 mousePosition = Input.mousePosition;
            startPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = true;
        }
    }

    private void OnMouseDrag() {
        if (wasThrown == false) {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
            newPosition.z = 0;
            transform.position = newPosition;
            transform.Find("shadow").gameObject.SetActive(false);
        }
    }

    private void OnMouseUp() {
        if (wasThrown == false) {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
            Vector3 newVelocity = (targetPosition - startPosition) * velocity;
            
            float speed = Vector3.Magnitude(newVelocity);

            if (speed < 1) {
                newVelocity = new Vector3(0, maxSpeed / 1.5f, 0);
            }

            newVelocity.z = 0;
            rigidBody.isKinematic = false;
            rigidBody.velocity = newVelocity;

            if (speed > maxSpeed) {
                float brakeSpeed = speed - maxSpeed;
                Vector3 brakeVelocity = rigidBody.velocity.normalized * brakeSpeed;
                rigidBody.AddForce(-brakeVelocity);
            }

            gameObject.tag = "EnemyThrown";
            wasThrown = true;
        }
    }

    private void Move() {
        rigidBody.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
    }

    private void SelfDestruct() {
        Destroy(transform.parent.gameObject);
    }

    private void CheckIfDead() {
        if (health <= 0) {
            animator.SetTrigger("dead");
            isAttacking = false;
            enabled = false;
            Invoke("SelfDestruct", 5f);
        }
    }

    public void LoseHitPoints(int damageAmount) {
        float reduction = armor / MAX_ARMOR;
        health -= (int) (reduction <= 0 ? 1 : 1 - reduction) * damageAmount;
        CheckIfDead();
    }

    private void StartAttack() {
        animator.SetTrigger("attack1");
        isAttacking = true;
    }

    private void Attack() {
        // dont decrement past 0
        float reduction = Game.Instance.data.currentArmor-- / Game.Instance.data.maxArmor;
        Game.Instance.data.currentHealth -= (int) (reduction <= 0 ? 1 : 1 - reduction) * damage;
    }
}
