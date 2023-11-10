using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager instance = null;

    [SerializeField]
    private AudioSource[] audio;

    [SerializeField]
    private Slider volumeSlider;

    public TextMeshProUGUI volumeText;

    private const string VolumeKey = "Volume"; // Key để lưu giá trị âm lượng

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume(); // Lấy giá trị volume từ PlayerPrefs
            return;
        }
        if (instance == this) return;
        Destroy(gameObject);
    }

    void Start()
    {
        audio[0].enabled = true;
        audio[1].enabled = false;

        // Đặt giá trị âm lượng ban đầu từ PlayerPrefs 
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            float volume = PlayerPrefs.GetFloat(VolumeKey);
            SetVolume(volume);
            volumeSlider.value = volume;
        }

        volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 0)
        {
            audio[0].enabled = true;
            audio[1].enabled = false;
        }
        else
        {
            audio[0].enabled = false;
            audio[1].enabled = true;
        }
    }

    // Hàm xử lý sự kiện thay đổi giá trị của Slider
    void OnVolumeSliderValueChanged(float value)
    {
        SetVolume(value);
        // Lưu giá trị âm lượng vào PlayerPrefs
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    // Thiết lập giá trị âm lượng cho tất cả các AudioSource
    void SetVolume(float value)
    {
        foreach (AudioSource source in audio)
        {
            source.volume = value;
        }

        // Hiển thị chỉ số âm lượng
        volumeText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    // Lấy giá trị volume từ PlayerPrefs thiết lập cho AudioSource và Slider
    private void LoadVolume ()
    {
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            float volume = PlayerPrefs.GetFloat(VolumeKey);
            SetVolume(volume);
            volumeSlider.value = volume;
        } else
        {
            SetVolume(1f); // Giá trị mặc định khi volume chưa được lưu trữ
            volumeSlider.value = 1f;
        }
    }
}
