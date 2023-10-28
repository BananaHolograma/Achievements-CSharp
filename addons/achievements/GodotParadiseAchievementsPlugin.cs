#if TOOLS
using Godot;

[Tool]
public partial class GodotParadiseAchievementsPlugin : EditorPlugin
{
	private const string AutoloadName = "GodotParadiseAchievements";

	public override void _EnterTree()
	{
		AddAutoloadSingleton(AutoloadName, "res://addons/achievements/achievements.tscn");
	}

	public override void _ExitTree() => RemoveAutoloadSingleton(AutoloadName);
}
#endif