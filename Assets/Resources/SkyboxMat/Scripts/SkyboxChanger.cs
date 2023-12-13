using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material[] Skyboxes;
    public Camera Camera;

    private Skybox skyboxComponent;

    private void Start()
    {
        skyboxComponent = Camera.GetComponent<Skybox>();

        if (Skyboxes.Length > 0)
        {
            int randomIndex = Random.Range(0, Skyboxes.Length);
            ChangeSkybox(Skyboxes[randomIndex]);
        }
    }

    public void ChangeSkybox(Material skyboxMaterial)
    {
        skyboxComponent.material = skyboxMaterial;
    }
}
