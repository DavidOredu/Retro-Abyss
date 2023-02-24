using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine.UI;

public class AudioManager : SingletonDontDestroy<AudioManager>, IPointerEnterHandler, IPointerClickHandler
{
    public GameObject soundPrefab;
    private List<GameObject> soundObjects = new List<GameObject>();
    public Sound[] sounds;
    public SettingsData gameSettings;

    public AudioClip uiClip;
    public Sound currentMusic;
    public float nextSongDelay = 1f;

    public List<MMFeedbacks> feedbackIconSounds;
    public List<MMFeedbacks> feedbackUISounds;

    [Tooltip("Can the sound settings in the list affect already instantiated sound sources?")]
    public bool dynamicSoundSetting;

    private List<Sound> musicList = new List<Sound>();
    private List<Sound> sfxList = new List<Sound>();
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        gameSettings = Resources.Load<SettingsData>("SettingsData");
        foreach (var sound in sounds)
        {
            switch (sound.soundType)
            {
                case SoundType.Music:
              //      sound.loop = true;
                    break;
                case SoundType.SFX:
                    break;
                default:
                    break;
            }
        }
        foreach (Sound sound in sounds)
        {
            if(sound.soundType == SoundType.Music)
                CreateSoundSource(sound);
        }

        foreach (var sound in sounds)
        {
            if (sound.soundType == SoundType.Music)
                musicList.Add(sound);
            if (sound.soundType == SoundType.SFX)
                sfxList.Add(sound);
        }
    }
    private void Start()
    {
     //   SetFeedbackSounds(true);

        PlayRandomMusic();
        if (!gameSettings.music)
            currentMusic.source.Stop();
    }
    private void Update()
    {
        foreach (var sound in sounds)
        {
            switch (sound.soundType)
            {
                case SoundType.Music:
                    break;
                case SoundType.SFX:
                    break;
                default:
                    break;
            }
        }

        if (dynamicSoundSetting)
        {
            foreach (Sound sound in sounds)
            {
                if(sound.source == null) { continue; }

                sound.source.clip = sound.clip;
                sound.source.loop = sound.loop;
            }
        }

        if (!currentMusic.source.isPlaying && gameSettings.music)
            PlayRandomMusic(true, nextSongDelay);
    }
    private void CreateSoundSource(Sound sound)
    {
        var obj = Instantiate(soundPrefab, transform);
        var soundSource = obj.GetComponent<AudioSource>();
        sound.source = soundSource;
        sound.source.clip = sound.clip;
        sound.source.loop = sound.loop;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        if (sound.soundType == SoundType.Music)
            obj.AddComponent<MMAudioSourcePitchShaker>().AlwaysResetTargetValuesAfterShake = true;
    }
    //public void SetFeedbackSounds(bool firstTime)
    //{
    //    foreach (var sound in feedbackUISounds)
    //    {
    //        MMFeedbackSound soundFeedback = null;
    //        soundFeedback = sound.gameObject.GetComponent<MMFeedbackSound>();
    //        if (soundFeedback == null)
    //        {
    //            soundFeedback = sound.gameObject.AddComponent<MMFeedbackSound>();
    //            sound.Feedbacks.Add(soundFeedback);
    //        }
    //        var button = sound.gameObject.GetComponent<Button>();
    //        var toggle = sound.gameObject.GetComponent<Toggle>();

    //        if (firstTime)
    //        {
    //            if (button)
    //                button.onClick.AddListener(() => sound.PlayFeedbacks());
    //            if (toggle)
    //                toggle.onValueChanged.AddListener((x) => sound.PlayFeedbacks());
    //        }
    //        soundFeedback.Sfx = uiClip;
    //        soundFeedback.Active = gameSettings.sound;
    //    }
    //    foreach (var sound in feedbackIconSounds)
    //    {
    //        var soundFeedback = sound.gameObject.GetComponent<MMFeedbackSound>();
    //        soundFeedback.Active = gameSettings.sound;
    //    }
    //}
    

    
    // Update is called once per frame
    public void PlayRandomMusic(bool delayed = false, float delayTime = .1f)
    {
        var randomSound = musicList[UnityEngine.Random.Range(0, musicList.Count)];
        currentMusic = randomSound;
        if (delayed)
            PlayDelayedSound(randomSound.SoundName, delayTime);
        else
            PlaySound(randomSound.SoundName);
    }
    public void PlaySound(string name, Transform positionToMoveSound = null, bool makeChildOfNewPosition = false)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }

        if(s.soundType == SoundType.SFX && s.source == null)
        {
            CreateSoundSource(s);
        }
        //if (UIManager.GameIsPaused)
        //{
        //    // play pause sound
        //    s.source.Pause();
        //}
        if (positionToMoveSound != null)
        {
            MoveSoundToPosition(s, positionToMoveSound, makeChildOfNewPosition);
        }

        s.source.Play();
    }
    public void PlaySound(int index)
    {
        if(sounds.Length == 0) { return; }

        Sound s = sounds[index];
        s.source.Play();
    }
    public void PlayDelayedSound(string name, float delayTime)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (UIManager.GameIsPaused)
        {
            // play pause sound
            s.source.Pause();
        }
        s.source.PlayDelayed(delayTime);
    }
    public void PlayOneShotSound(string name, Transform positionToMoveSound = null, bool makeChildOfNewPosition = false)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (UIManager.GameIsPaused)
        {
            // play pause sound
            s.source.Pause();
        }

        if (s.soundType == SoundType.SFX && s.source == null)
        {
            CreateSoundSource(s);
        }

        if (positionToMoveSound != null)
        {
            MoveSoundToPosition(s, positionToMoveSound, makeChildOfNewPosition);
        }

        s.source.PlayOneShot(s.clip);
    }
    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.SoundName == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }
    public Sound FindSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.SoundName == name)
            {
                return s;
            }
            else
            {
                continue;
            }
        }
        Debug.LogError("Given name does not correspond to any sound name existing in the sound array. Make sure the spelling corresponds to the required sound name or add a sound to fit that name.");
        return null;
    }
    public IEnumerator PauseSFXSounds()
    {
        List<Sound> playingSounds = new List<Sound>();
        foreach (var sound in sfxList)
        {
            if (sound.source.isPlaying)
            {
                sound.source.Pause();
                playingSounds.Add(sound);
            }
        }
        yield return new WaitWhile(() => GameManager.instance.GameIsPaused);

        foreach (var sound in playingSounds)
        {
            sound.source.UnPause();
        }
    }
    public enum SoundType
    {
        Music,
        SFX,
    }
    public void MoveSoundToPosition(Sound sound, Transform transform, bool makeChildOfNewPosition)
    {
        sound.source.transform.position = transform.position;

        if (makeChildOfNewPosition)
            sound.source.transform.parent = transform;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
