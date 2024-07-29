using UnityEngine;


public class Resource : MonoBehaviour
{
    private float invincibleTime = 2.0f;
    private bool invincible = true;

    private void Update()
    {
        invincibleTime -= Time.deltaTime;
        if (invincibleTime <= 0.0f)
        {
            invincible = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !invincible)
        {
            Player player = other.GetComponent<Player>();
            if (player.score < 3)
            {
                player.GetResource(1);
                Destroy(gameObject);
                //AudioEat.PlayAudio();
            }
        }
    }
}
