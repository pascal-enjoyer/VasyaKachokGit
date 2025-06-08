using UnityEngine;

public class DeathSoundHandler : MonoBehaviour
{
    public static void PlayDeathSound(HitSounds hitSounds)
    {
        // ���������, ���� �� ��������� HitSounds
        if (hitSounds == null)
        {
            Debug.LogError("��������� HitSounds �� �������!");
            return;
        }

        // �������� �������� ��������� �������
        Transform parent = hitSounds.transform.parent;

        // ������� ����� GameObject
        GameObject soundObject = new GameObject("DeathSoundObject");

        // ������������� ��������
        soundObject.transform.SetParent(parent, false);

        // ��������� ��� ������� ����� AudioSource
        AudioSource originalAudioSource = hitSounds.GetComponent<AudioSource>();
        AudioSource newAudioSource = soundObject.AddComponent<AudioSource>();

        if (originalAudioSource != null)
        {
            // �������� ��������� AudioSource
            newAudioSource.clip = originalAudioSource.clip;
            newAudioSource.volume = originalAudioSource.volume;
            newAudioSource.pitch = originalAudioSource.pitch;
            newAudioSource.spatialBlend = originalAudioSource.spatialBlend;
            newAudioSource.loop = originalAudioSource.loop;
            newAudioSource.playOnAwake = originalAudioSource.playOnAwake;
            // �������� ������ ���������, ���� �����
        }
        else
        {
            // ��������� �� ��������� ��� ������ AudioSource
            newAudioSource.volume = 1.0f;
            newAudioSource.pitch = 1.0f;
            newAudioSource.spatialBlend = 0.0f; // 2D ���� �� ���������
            newAudioSource.loop = false;
            newAudioSource.playOnAwake = false;
            Debug.LogWarning("AudioSource �� ������ �� �������� �������. ������ ����� AudioSource � ����������� �� ���������.");
        }

        // ��������� ��������� HitSounds
        HitSounds newHitSounds = soundObject.AddComponent<HitSounds>();
        newHitSounds.audioClips = hitSounds.audioClips; // �������� ������ �����������

        // ������������� ����
        newHitSounds.PlayNextAudio();

        // ���������� ������ ����� 10 ������
        Destroy(soundObject, 10f);
    }
}