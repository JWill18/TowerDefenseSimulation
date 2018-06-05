using UnityEditor;
using System;

public class BuildManager {

	public static string[] args = Environment.GetCommandLineArgs();

	private static string GetArgument(string name)
	{
		string argumentValue = "";

		for(int i = 0; i < args.Length; i++)
		{
			if(args[i].Contains(name))
			{
				argumentValue = args[i + 1];
			}
		}

		return argumentValue;
	}

	private static BuildTarget GetBuildTarget()
	{
		var customTarget = GetArgument("customBuildTarget");

		return customTarget == "StandaloneWindows64" ? BuildTarget.StandaloneWindows64 : BuildTarget.NoTarget;
	}

	public static void PerformBuild()
	{
		string[] scenes = { "Assets/Scenes/Scn_Level_01.unity" };
		var buildOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			target = BuildTarget.StandaloneWindows64,
			targetGroup = BuildTargetGroup.Standalone,
			locationPathName = GetArgument("customBuildPath")
		};

		var results = BuildPipeline.BuildPlayer(buildOptions);

		if(results.summary.totalErrors == 0)
		{
			EditorApplication.Exit(0);
		}
		else
		{
			EditorApplication.Exit(1);
		}
	}
}
