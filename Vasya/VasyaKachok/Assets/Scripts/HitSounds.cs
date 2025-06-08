using UnityEngine;

public class HitSounds : MonoBehaviour
{
    // Массив для хранения аудиоклипов (заполняется в инспекторе Unity)
    public AudioClip[] audioClips;

    private AudioSource audioSource;
    private int currentIndex = 0;
    
    void Start()
    {

        // Получаем компонент AudioSource
        audioSource = GetComponent<AudioSource>();

        // Проверяем, есть ли аудиоклипы
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("Массив аудиоклипов пуст! Добавьте аудиофайлы в инспекторе.");
        }
    }

    // Функция для воспроизведения следующего аудио
    public void PlayNextAudio()
    {
        // Проверяем, есть ли аудиоклипы
        if (audioClips.Length == 0) return;

        // Останавливаем текущее воспроизведение
        audioSource.Stop();

        // Устанавливаем текущий аудиоклип
        audioSource.clip = audioClips[currentIndex];

        // Воспроизводим аудио
        audioSource.Play();

        // Обновляем индекс для следующего аудио
        currentIndex = (currentIndex + 1) % audioClips.Length;
    }
}
