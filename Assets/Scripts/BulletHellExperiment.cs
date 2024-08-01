using UnityEngine;

public class BulletHellExperiment : MonoBehaviour
{
    public GameObject bulletPrefab;
    private int currentSpiralAngleOffset = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            GenerateLinearBullets(currentSpiralAngleOffset, 60, 12);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GenerateSpiralBullets(currentSpiralAngleOffset, 60, 12);
            currentSpiralAngleOffset += 10;
        }
    }

    public void GenerateLinearBullets(int a, int d, int numberOfBullets)
    {
        for (int n = 0; n < numberOfBullets; n++)
        {
            float angle = a + n * d;

            float angleInRadians = angle * (Mathf.PI / 180);

            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            bullet.GetComponent<Projectile>().SetDirection(direction);
        }
    }

    public void GenerateSpiralBullets(int initialAngle, float angleIncrement, int numberOfBullets)
    {
        float angle = initialAngle;

        for (int n = 0; n < numberOfBullets; n++)
        {
            float angleInRadians = angle * (Mathf.PI / 180);

            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            bullet.GetComponent<Projectile>().SetDirection(direction);

            angle += angleIncrement;
        }
    }
}
