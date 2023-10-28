/*
Created by https://github.com/GodotParadise organization with LICENSE MIT
There are no restrictions on modifying, sharing, or using this component commercially
We greatly appreciate your support in the form of stars, as they motivate us to continue our journey of enhancing the Godot community
***************************************************************************************
Implement achievements in your game in a simple way and with minimal security practices now in C#.
*/
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

public partial class GodotParadiseAchievements : Node
{
	[Signal]
	public delegate void AchievementUnlockedEventHandler(string name, Dictionary achievement);
	[Signal]
	public delegate void AchievementUpdatedEventHandler(string name, Dictionary achievement);
	[Signal]
	public delegate void AchievementResetEventHandler(string name, Dictionary achievement);
	[Signal]
	public delegate void AllAchievementsUnlockedEventHandler();


	public HttpRequest httpRequest;
	public string SettingsPath = $"{ProjectSettings.GetSetting("application/config/name")}/config/achievements";

	public Dictionary CurrentAchievements = new();
	public Dictionary UnlockedAchievements = new();
	public List<string> AchievementKeys = new();


	/* Basic achievement dictionary structure
			"achievement-name": {
				"name": "MY achievement",
				"description": "This is my awesome achievement",
				"is_secret": false,
				"count_goal": 25,
				"current_progress": 0.0,
				"icon_path": "res://assets/icon/my-achievement.png",
				"unlocked": false,
				"active": true
			}
	*/

	public override void _Ready()
	{
		httpRequest = GetNode<HttpRequest>("HttpRequest");
		httpRequest.RequestCompleted += OnRequestCompleted;

		AchievementUpdated += OnAchievementUpdated;
		AchievementUnlocked += OnAchievementUpdated;

		CreateSaveDirectory((string)ProjectSettings.GetSetting($"{SettingsPath}/save_directory"));
		PrepareAchievements();
	}

	public Dictionary GetAchievement(string name)
	{
		if (CurrentAchievements.ContainsKey(name))
		{
			return (Dictionary)CurrentAchievements[name];
		}

		return new();
	}

	public void UpdateAchievement(string name, Dictionary data)
	{
		if (CurrentAchievements.ContainsKey(name))
		{
			((Dictionary)CurrentAchievements[name]).Merge(data, true);

			EmitSignal(SignalName.AchievementUpdated, name, data);
		}
	}

	public void UnlockAchievement(string name)
	{
		if (CurrentAchievements.ContainsKey(name))
		{
			Dictionary currentAchievement = (Dictionary)CurrentAchievements[name];

			if (!(bool)currentAchievement["unlocked"])
			{
				currentAchievement["unlocked"] = true;
				UnlockedAchievements[name] = currentAchievement;
				EmitSignal(SignalName.AchievementUnlocked, name, currentAchievement);
			}
		}
	}

	public void ResetAchievement(string name, Dictionary data)
	{
		if (CurrentAchievements.ContainsKey(name))
		{
			Dictionary currentAchievement = (Dictionary)CurrentAchievements[name];

			currentAchievement.Merge(data, true);
			currentAchievement["unlocked"] = false;
			currentAchievement["current_progress"] = 0.0f;

			if (UnlockedAchievements.ContainsKey(name))
			{
				UnlockedAchievements.Remove(name);
			}

			EmitSignal(SignalName.AchievementReset, name, currentAchievement);
			EmitSignal(SignalName.AchievementUpdated, name, currentAchievement);
		}
	}

	private void ReadFromLocalSource()
	{
		string localSourceFile = LocalSourceFilePath();

		if (FileAccess.FileExists(localSourceFile))
		{
			Dictionary content = JsonSerializer.Deserialize<Dictionary>(FileAccess.GetFileAsString(localSourceFile));

			if (content is not null)
			{
				CurrentAchievements = content;
				AchievementKeys = (List<string>)CurrentAchievements.Keys;
			}

			GD.PushError($"GodotParadiseAchievements: Failed reading achievement file {localSourceFile}");
		}
	}

	private async void ReadFromRemoteSource()
	{
		if (IsValidUrl(RemoteSourceUrl()))
		{
			httpRequest.Request(RemoteSourceUrl());
			await ToSignal(httpRequest, "RequestCompleted");
		}
	}

	private string LocalSourceFilePath()
	{
		return (string)ProjectSettings.GetSetting($"{SettingsPath}/local_source");
	}

	private string RemoteSourceUrl()
	{
		return (string)ProjectSettings.GetSetting($"{SettingsPath}/remote_source");
	}

	private string EncryptedSaveFilePath()
	{
		return $"{ProjectSettings.GetSetting($"{SettingsPath}/save_directory")}/{ProjectSettings.GetSetting($"{SettingsPath}/save_file_name")}";
	}

	private string GetPassword()
	{
		return (string)ProjectSettings.GetSetting($"{SettingsPath}/password");
	}

	private void CreateSaveDirectory(string path)
	{
		DirAccess.MakeDirAbsolute(path);
	}

	private void PrepareAchievements()
	{
		ReadFromLocalSource();
		ReadFromRemoteSource();
		SyncAchievementsWithEncryptedSavedFile();

		foreach (string key in CurrentAchievements.Keys)
		{
			Dictionary currentAchievement = (Dictionary)CurrentAchievements[key];

			if ((bool)currentAchievement["unlocked"])
			{
				UnlockedAchievements.Add(key, CurrentAchievements[key]);
			}
		}
	}

	private void SyncAchievementsWithEncryptedSavedFile()
	{
		string savedFilePath = EncryptedSaveFilePath();

		if (FileAccess.FileExists(savedFilePath))
		{
			FileAccess content = FileAccess.OpenEncryptedWithPass(savedFilePath, FileAccess.ModeFlags.Write, GetPassword());

			if (content is not null)
			{

			}

			Dictionary achievements = JsonSerializer.Deserialize<Dictionary>(content.GetAsText());

			if (achievements is not null)
			{
				CurrentAchievements.Merge(achievements, true);
			}
			return;
		}

		GD.PushError($"GodotParadiseAchievements: Failed reading saved achievement file {savedFilePath} with error {FileAccess.GetOpenError()}");

	}

	private bool CheckIfAllAchievementsAreUnlocked()
	{
		bool allUnlocked = UnlockedAchievements.Count == CurrentAchievements.Count;

		if (allUnlocked)
		{
			EmitSignal(SignalName.AllAchievementsUnlocked);
		}

		return allUnlocked;
	}

	private void UpdateEncryptedSaveFile()
	{
		if (CurrentAchievements.Count == 0)
		{
			return;
		}

		string savedFilePath = EncryptedSaveFilePath();

		FileAccess file = FileAccess.OpenEncryptedWithPass(savedFilePath, FileAccess.ModeFlags.Write, GetPassword());

		if (file is not null)
		{
			file.StoreString(JsonSerializer.Serialize(CurrentAchievements));
			file.Close();
			return;
		}

		GD.PushError($"GodotParadiseAchievements: Failed writing saved achievement file {savedFilePath} with error {FileAccess.GetOpenError()}");

	}

	private static bool IsValidUrl(string url)
	{
		return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
	}

	private void OnAchievementUpdated(string name, Dictionary achievement)
	{
		UpdateEncryptedSaveFile();
		CheckIfAllAchievementsAreUnlocked();
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		if (result == (long)HttpRequest.Result.Success)
		{
			Dictionary content = Json.ParseString(Encoding.UTF8.GetString(body)).AsGodotDictionary();

			if (content is not null)
			{
				CurrentAchievements.Merge(content, true);
			}

			return;
		}

		GD.PushError($"GodotParadiseAchievements: Failed request to {RemoteSourceUrl()} with response code {responseCode}");
	}
}
