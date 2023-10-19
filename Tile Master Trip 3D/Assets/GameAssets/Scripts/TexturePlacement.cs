using UnityEngine;

public class TexturePlacement : MonoBehaviour
{
    public Texture2D myTexture; // Texture bạn muốn sử dụng

    void Start()
    {

        // 2. Gắn Material vào GameObject (khối hộp 3D)
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = myTexture;
    }
}
