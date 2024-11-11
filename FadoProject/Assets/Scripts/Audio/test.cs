using System.Collections;
using UnityEngine;

public class TestTracks : MonoBehaviour
{
    // Referência ao AudioManager
    private AudioManager audioManager;

    private void Start()
    {
        // Obtém a instância do AudioManager
        audioManager = AudioManager.Instance;

        // Inicia o teste da reprodução das músicas
        StartCoroutine(TestAllTracks());
    }

    private IEnumerator TestAllTracks()
    {
        yield return new WaitForSeconds(2f);
        // Obtém todos os valores do enum Musics
        foreach (Musics track in System.Enum.GetValues(typeof(Musics)))
        {
            // Configura o track atual no AudioManager
            audioManager.SetMusic(track);
            //Debug.Log($"Playing track: {track}");

            // Aguarda 6 segundos antes de reproduzir o próximo track
            yield return new WaitForSeconds(15f);
        }

        Debug.Log("All tracks tested.");
    }
}