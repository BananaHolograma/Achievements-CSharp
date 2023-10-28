#if TOOLS
using Godot;
using System;

[Tool]
public partial class GodotParadiseAchievementsPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		
	}

	public override void _ExitTree() => RemoveCustomType("GodotParadiseProjectileComponent");
}
#endif