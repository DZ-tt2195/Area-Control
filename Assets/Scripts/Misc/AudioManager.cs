using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MyBox;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Foldout("Play audio", true)]
    public static AudioManager instance;
    AudioSource audioPlayer;
    public AudioMixer mixer;
    [Foldout("Sound effects", true)]
    [SerializeField] AudioClip menuSound; public void Menu(float volume = 0.3f) => PlaySound(menuSound, volume);
    [SerializeField] AudioClip cardSound; public void Card(float volume = 0.3f) => PlaySound(cardSound, volume);
    [SerializeField] AudioClip tokenSound; public void Token(float volume = 0.3f) => PlaySound(tokenSound, volume);
    [SerializeField] AudioClip gameOverSound; public void GameOver(float volume = 0.3f) => PlaySound(gameOverSound, volume);
    private void Awake()
    {
        if (instance == null)
        {
            audioPlayer = GetComponent<AudioSource>();
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void PlaySound(AudioClip audio, float volume = 0.3f)
    {
        audioPlayer.PlayOneShot(audio, volume);
    }
}
