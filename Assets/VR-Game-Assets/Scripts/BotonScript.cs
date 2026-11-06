using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BotonScript : MonoBehaviour
{
    public GameObject countdownPanel;
    public TMP_Text countdownText;
    public int countdownSeconds = 3;
    public string goText = "GO!";
    public float goDuration = 0.5f;
    public AudioClip tickSound;
    public AudioClip goSound;
    public float popScale = 1.5f;
    public float popDuration = 0.3f;
    // Texto encima del boton (vacio = sin texto, p.ej. el boton Back de GameScene)
    public string buttonLabel = "";
    public float labelHeight = 0.35f;
    public float labelFontSize = 1f;

    private bool counting = false;
    private Vector3 originalTextScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (countdownPanel != null) countdownPanel.SetActive(false);
        if (countdownText != null) originalTextScale = countdownText.transform.localScale;
        if (buttonLabel != "") ButtonLabel.Create(transform, buttonLabel, labelHeight, labelFontSize);
    }

    public void cargarScena(){
        // Evitamos que pulsar varias veces el boton lance varias cuentas atras
        if (counting) return;
        counting = true;
        StartCoroutine(CuentaAtras());
    }

    IEnumerator CuentaAtras()
    {
        // Cargamos la escena en segundo plano mientras dura la cuenta atras
        AsyncOperation carga = SceneManager.LoadSceneAsync("GameScene");
        carga.allowSceneActivation = false;

        countdownPanel.SetActive(true);
        for (int i = countdownSeconds; i > 0; i--)
        {
            ShowNumber(i.ToString(), tickSound);
            yield return new WaitForSeconds(1f);
        }

        ShowNumber(goText, goSound);
        yield return new WaitForSeconds(goDuration);

        carga.allowSceneActivation = true;
    }

    void ShowNumber(string text, AudioClip sound)
    {
        countdownText.text = text;
        if (sound != null) AudioSource.PlayClipAtPoint(sound, countdownText.transform.position);
        StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        float time = 0f;
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(popScale, 1f, time / popDuration);
            countdownText.transform.localScale = originalTextScale * scale;
            yield return null;
        }
        countdownText.transform.localScale = originalTextScale;
    }

    public void ir()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void volverLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
