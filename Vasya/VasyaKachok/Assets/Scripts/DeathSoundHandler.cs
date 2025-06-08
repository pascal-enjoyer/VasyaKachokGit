using UnityEngine;

public class DeathSoundHandler : MonoBehaviour
{
    public static void PlayDeathSound(HitSounds hitSounds)
    {
        // Проверяем, есть ли компонент HitSounds
        if (hitSounds == null)
        {
            Debug.LogError("Компонент HitSounds не передан!");
            return;
        }

        // Получаем родителя исходного объекта
        Transform parent = hitSounds.transform.parent;

        // Создаем новый GameObject
        GameObject soundObject = new GameObject("DeathSoundObject");

        // Устанавливаем родителя
        soundObject.transform.SetParent(parent, false);

        // Переносим или создаем новый AudioSource
        AudioSource originalAudioSource = hitSounds.GetComponent<AudioSource>();
        AudioSource newAudioSource = soundObject.AddComponent<AudioSource>();

        if (originalAudioSource != null)
        {
            // Копируем настройки AudioSource
            newAudioSource.clip = originalAudioSource.clip;
            newAudioSource.volume = originalAudioSource.volume;
            newAudioSource.pitch = originalAudioSource.pitch;
            newAudioSource.spatialBlend = originalAudioSource.spatialBlend;
            newAudioSource.loop = originalAudioSource.loop;
            newAudioSource.playOnAwake = originalAudioSource.playOnAwake;
            // Копируем другие параметры, если нужно
        }
        else
        {
            // Настройки по умолчанию для нового AudioSource
            newAudioSource.volume = 1.0f;
            newAudioSource.pitch = 1.0f;
            newAudioSource.spatialBlend = 0.0f; // 2D звук по умолчанию
            newAudioSource.loop = false;
            newAudioSource.playOnAwake = false;
            Debug.LogWarning("AudioSource не найден на исходном объекте. Создан новый AudioSource с настройками по умолчанию.");
        }

        // Переносим компонент HitSounds
        HitSounds newHitSounds = soundObject.AddComponent<HitSounds>();
        newHitSounds.audioClips = hitSounds.audioClips; // Копируем массив аудиоклипов

        // Воспроизводим звук
        newHitSounds.PlayNextAudio();

        // Уничтожаем объект через 10 секунд
        Destroy(soundObject, 10f);
    }
}