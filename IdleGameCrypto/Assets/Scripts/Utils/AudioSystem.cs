using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


	public class AudioSystem : MonoBehaviour
	{
		public enum ECategory
		{
			None,
			Ui,
			Gameplay,
			Fx,
			Music
		}

		public enum EPlayMode
		{
			OneShot,
			Loop
		}

		public enum EPriority
		{
			High,
			Low
		}

		[Serializable]
		public struct CategorySetupNode
		{
			public ECategory category;

			public int maxSimultaneousSounds;

			public AudioMixerGroup mixer;
		}

		[Serializable]
		public struct AudioLibraryNode
		{
			public ECategory category;

			public AudioClip[] clips;
		}

		[Header("Setup")]
		public CategorySetupNode[] categoriesSetup;

		[Header("Resources")]
		public AudioLibraryNode[] audioLibrary;

		private Dictionary<string, AudioClip> _clips;

		private Dictionary<ECategory, AudioSource[]> _audioPlayers;

		private static readonly string[] SONGS = new string[1]
		{
			"market_music_1"
			
		};

		public const string KEY_IS_MUSIC_ENABLED = "IsMusicEnabled";

		public const string KEY_IS_SFX_ENABLED = "IsSfxEnabled";

		public static bool isMusicEnabled
		{
			get
			{
				return PlayerPrefsX.GetBool("IsMusicEnabled", defaultValue: true);
			}
			set
			{
				PlayerPrefsX.SetBool("IsMusicEnabled", value);
			}
		}

		public static bool isSfxEnabled
		{
			get
			{
				return PlayerPrefsX.GetBool("IsSfxEnabled", defaultValue: true);
			}
			set
			{
				PlayerPrefsX.SetBool("IsSfxEnabled", value);
			}
		}

		public static AudioSystem _instance;

		public void Initialize()
		{
			_audioPlayers = new Dictionary<ECategory, AudioSource[]>(categoriesSetup.Length);
			CategorySetupNode[] array = categoriesSetup;
			for (int i = 0; i < array.Length; i++)
			{
				CategorySetupNode categorySetupNode = array[i];
				if (categorySetupNode.category == ECategory.None)
				{
					UnityEngine.Debug.LogError("Audio system does not support the 'None' category. It's used for error checking, don't setup any sounds in that category");
					continue;
				}
				GameObject gameObject = new GameObject(categorySetupNode.category.ToString());
				gameObject.transform.SetParent(base.transform);
				AudioSource[] array2 = new AudioSource[categorySetupNode.maxSimultaneousSounds];
				for (int j = 0; j < categorySetupNode.maxSimultaneousSounds; j++)
				{
					GameObject gameObject2 = new GameObject(categorySetupNode.category.ToString() + "_" + j);
					gameObject2.transform.SetParent(gameObject.transform);
					AudioSource audioSource = gameObject2.AddComponent<AudioSource>();
					audioSource.bypassEffects = true;
					audioSource.bypassReverbZones = true;
					audioSource.playOnAwake = false;
					audioSource.loop = false;
					audioSource.outputAudioMixerGroup = categorySetupNode.mixer;
					array2[j] = audioSource;
				}
				_audioPlayers.Add(categorySetupNode.category, array2);
			}
			_clips = new Dictionary<string, AudioClip>();
			for (int k = 0; k < audioLibrary.Length; k++)
			{
				AudioClip[] clips = audioLibrary[k].clips;
				foreach (AudioClip audioClip in clips)
				{
					_clips.Add(audioClip.name, audioClip);
				}
			}
		}

		public void SetAllVolumes(float targetVolume, float tweenDuration = 1f)
		{
			SetVolume(ECategory.Fx, targetVolume, tweenDuration);
			SetVolume(ECategory.Gameplay, targetVolume, tweenDuration);
			SetVolume(ECategory.Music, targetVolume, tweenDuration);
			SetVolume(ECategory.Ui, targetVolume, tweenDuration);
		}

		public void SetVolume(ECategory category, float targetVolume, float tweenDuration = 1f)
		{
			float volume = GetVolume(category);
			AudioMixerGroup audioMixerGroup = FindMixer(category);
			AudioMixer mixer = audioMixerGroup.audioMixer;
			UnityEngine.Debug.Log("#Audio# Found mixer " + audioMixerGroup.name + " for category " + category + " to set volume to " + targetVolume);
		 
			DOTween.To(() => volume, delegate(float x)
			{
				volume = x;
				float value = ConvertVolumeToAttenuation(volume);
				mixer.SetFloat("Volume", value);
			}, targetVolume, tweenDuration);
		}

		private float ConvertVolumeToAttenuation(float volume)
		{
			return Mathf.Log(Mathf.Lerp(0.001f, 1f, volume)) * 20f;
		}

		private float ConvertAttenuationToVolume(float attenuation)
		{
			return Mathf.Pow(attenuation - -0.8f, 2f);
		}

		public float GetVolume(ECategory category)
		{
			AudioMixerGroup audioMixerGroup = FindMixer(category);
			float value = 0f;
			if (!audioMixerGroup.audioMixer.GetFloat("Volume", out value))
			{
				UnityEngine.Debug.Log("#Audio# Failed to find the mixer for category " + category + " when asking for the current volume");
			}
			return ConvertAttenuationToVolume(value);
		}

		public AudioMixerGroup FindMixer(ECategory category)
		{
			return FindSetup(category).mixer;
		}

		public CategorySetupNode FindSetup(ECategory category)
		{
			CategorySetupNode[] array = categoriesSetup;
			foreach (CategorySetupNode categorySetupNode in array)
			{
				if (categorySetupNode.category == category)
				{
					return categorySetupNode;
				}
			}
			return default(CategorySetupNode);
		}

		public bool Play(string soundName, ECategory category, float pitch = 1f, EPlayMode playmode = EPlayMode.OneShot, EPriority priority = EPriority.Low)
		{
			AudioSource audioSource = FindAvailablePlayer(category, priority);
			if (audioSource == null)
			{
				return false;
			}
			AudioClip audioClip = FindClip(soundName, category);
			if (audioClip == null)
			{
				return false;
			}
			audioSource.loop = ((playmode == EPlayMode.Loop) ? true : false);
			audioSource.clip = audioClip;
			audioSource.pitch = pitch;
			audioSource.Play();
			return true;
		}

		public bool Play(string soundName, float pitch = 1f, EPlayMode playMode = EPlayMode.OneShot, EPriority priority = EPriority.Low)
		{
			ECategory eCategory = FindCategory(soundName);
			if (eCategory == ECategory.None)
			{
				return false;
			}
			return Play(soundName, eCategory, pitch, playMode, priority);
		}

		public bool Play(string soundName)
		{
			ECategory eCategory = FindCategory(soundName);
			if (eCategory == ECategory.None)
			{
				return false;
			}
			return Play(soundName, eCategory);
		}

		public bool Play(string soundName, float pitch)
		{
			ECategory eCategory = FindCategory(soundName);
			if (eCategory == ECategory.None)
			{
				return false;
			}
			return Play(soundName, eCategory, pitch);
		}

		public bool Play(string soundName, EPlayMode playMode)
		{
			ECategory eCategory = FindCategory(soundName);
			if (eCategory == ECategory.None)
			{
				return false;
			}
			return Play(soundName, eCategory, 1f, playMode);
		}

		public bool Play(string soundName, EPriority priority)
		{
			ECategory eCategory = FindCategory(soundName);
			if (eCategory == ECategory.None)
			{
				return false;
			}
			return Play(soundName, eCategory, 1f, EPlayMode.OneShot, priority);
		}

		public bool Stop(string soundName)
		{
			ECategory key = FindCategory(soundName);
			if (!_audioPlayers.TryGetValue(key, out AudioSource[] value))
			{
				return false;
			}
			AudioSource[] array = value;
			foreach (AudioSource audioSource in array)
			{
				if (!(audioSource.clip.name != soundName))
				{
					audioSource.Stop();
					return true;
				}
			}
			return false;
		}

		public bool Stop(ECategory category)
		{
			if (!_audioPlayers.TryGetValue(category, out AudioSource[] value))
			{
				return false;
			}
			AudioSource[] array = value;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
			return true;
		}

		public void StopAll()
		{
			foreach (KeyValuePair<ECategory, AudioSource[]> audioPlayer in _audioPlayers)
			{
				AudioSource[] value = audioPlayer.Value;
				for (int i = 0; i < value.Length; i++)
				{
					value[i].Stop();
				}
			}
		}

		private ECategory FindCategory(string soundName)
		{
			AudioLibraryNode[] array = audioLibrary;
			foreach (AudioLibraryNode audioLibraryNode in array)
			{
				AudioClip[] clips = audioLibraryNode.clips;
				for (int j = 0; j < clips.Length; j++)
				{
					if (clips[j].name == soundName)
					{
						return audioLibraryNode.category;
					}
				}
			}
			return ECategory.None;
		}

		private AudioSource FindAvailablePlayer(ECategory category, EPriority priority = EPriority.Low)
		{
			if (!_audioPlayers.TryGetValue(category, out AudioSource[] value))
			{
				UnityEngine.Debug.LogError("No audio players setup for audio category " + category + ". Check Audio System");
				return null;
			}
			bool flag = false;
			AudioSource[] array = value;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource == null)
				{
					flag = true;
				}
				else if (!audioSource.isPlaying)
				{
					return audioSource;
				}
			}
			if (flag)
			{
				UnityEngine.Debug.LogWarning("An audio player from the audio system was destroyed. This may trigger during the game exit, but check for errors if it happens somewhere else.");
				return null;
			}
			if (priority == EPriority.High)
			{
				return value[UnityEngine.Random.Range(0, value.Length)];
			}
			return null;
		}

		public AudioClip FindClip(string soundName, ECategory category)
		{
			if (!_clips.TryGetValue(soundName, out AudioClip value))
			{
				UnityEngine.Debug.LogError("Sound " + soundName + " was not found in category " + category + ". Check Audio System");
				return null;
			}
			return value;
		}

		private void Awake()
		{
			_instance = this;
			Initialize();
		}
    private void Update()
    {
		UpdateSystem(Time.deltaTime);
    }

    public bool IsMute(ECategory category)
		{
			if (category == ECategory.Music)
			{
				return !isMusicEnabled;
			}
			return !isSfxEnabled;
		}

		public void Mute(ECategory category)
		{
			SetVolume(category, 0f);
			if (category == ECategory.Music)
			{
				isMusicEnabled = false;
			}
			else
			{
				isSfxEnabled = false;
			}
		}

		public void Unmute(ECategory category)
		{
			SetVolume(category, 1f);
			if (category == ECategory.Music)
			{
				isMusicEnabled = true;
			}
			else
			{
				isSfxEnabled = true;
			}
		}

		public AudioSource[] GetAudioPlayers(ECategory category)
		{
			if (!_audioPlayers.TryGetValue(category, out AudioSource[] value))
			{
				UnityEngine.Debug.LogError("No audio players setup for audio category " + category + ". Check Audio System");
				return null;
			}
			return value;
		}

		public void UpdateSystem(float deltaTime)
		{
			AudioSource[] audioPlayers = GetAudioPlayers(ECategory.Music);
			bool flag = true;
			AudioSource[] array = audioPlayers;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].isPlaying)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				PlayRandomMusic();
			}
		}

		public void PlayRandomMusic()
		{
			int num = UnityEngine.Random.Range(0, SONGS.Length);
			string soundName = SONGS[num];
			Play(soundName, ECategory.Music);
		}

	   public void InitializeSounds()
	   {
		bool flag = IsMute(AudioSystem.ECategory.Music);
		SetVolume(AudioSystem.ECategory.Music, (!flag) ? 1 : 0, 0f);
		bool flag2 = IsMute(AudioSystem.ECategory.Ui);
		SetVolume(AudioSystem.ECategory.Ui, (!flag2) ? 1 : 0, 0f);
		SetVolume(AudioSystem.ECategory.Music, (!flag) ? 1 : 0, 0f);
		PlayRandomMusic();
	   }

}

