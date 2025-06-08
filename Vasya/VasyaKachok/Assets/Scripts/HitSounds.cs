using UnityEngine;

public class HitSounds : MonoBehaviour
{
    // ������ ��� �������� ����������� (����������� � ���������� Unity)
    public AudioClip[] audioClips;

    private AudioSource audioSource;
    private int currentIndex = 0;
    
    void Start()
    {

        // �������� ��������� AudioSource
        audioSource = GetComponent<AudioSource>();

        // ���������, ���� �� ����������
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("������ ����������� ����! �������� ���������� � ����������.");
        }
    }

    // ������� ��� ��������������� ���������� �����
    public void PlayNextAudio()
    {
        // ���������, ���� �� ����������
        if (audioClips.Length == 0) return;

        // ������������� ������� ���������������
        audioSource.Stop();

        // ������������� ������� ���������
        audioSource.clip = audioClips[currentIndex];

        // ������������� �����
        audioSource.Play();

        // ��������� ������ ��� ���������� �����
        currentIndex = (currentIndex + 1) % audioClips.Length;
    }
}
