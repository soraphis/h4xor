using System.Collections;
using UnityEngine;

// Requirements:
// Unity 5
// testet with version: 5.3.0f4

// does need UnityStandardAssets > ImageEffects
// does not work if you use the ImageEffect "ScreenOverlay" on your camera for visual effects

// Usage:
// From a MonoBehaviour-Class: StartCoroutine(CameraFade.FadeOut(0.8f));
// From inside a coroutine (if you want to wait for finished fading): yield return CameraFade.FadeOut(0.5f);
// From inside a coroutine (if you dont want to wait): CameraFade.FadeOut(0.5f);

class CameraFadeScript : MonoBehaviour {
    
    [SerializeField] private Shader shader;
    private Material material;
    
    [Range(0, 1)] public float intensity;
    
    void Awake() {
        shader = shader ?? Shader.Find("Hidden/FadeToBlack");
        material = new Material(shader);
        material.SetColor("_Color", Color.black); 
    }

    void Update() {
        material = new Material(shader);
        material.SetFloat("_Intensity", intensity);
    }
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, material);
    }
    
    public static CameraFadeScript SetupCameraFade() {
        var x = Camera.main.GetComponent<CameraFadeScript>();
        if (x == null) {
            x = Camera.main.gameObject.AddComponent<CameraFadeScript>();
        }
        return x;
    }

    public static IEnumerator FadeOut(float seconds) {
        var x = SetupCameraFade();
        x.intensity = 0; // fade out: 0 -> 1

        for(float i = 0; i < seconds; i+= Time.unscaledDeltaTime) {
            x.intensity = i / seconds;
            yield return null;
        }
        x.intensity = 1;
        yield return null;
    }

    public static IEnumerator FadeIn(float seconds) {
        var x = SetupCameraFade();
        x.intensity = 1; // fade in: 1 -> 0

        for (float i = 0; i < seconds; i += Time.unscaledDeltaTime) {
            x.intensity = 1 - i / seconds;
            yield return null;
        }
        x.intensity = 0;
        yield return null;
    }

}
