 ///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var deployDir = Argument<string>("deployDir","");
///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles("./**/*.sln");
var solutionPaths = solutions.Select(solution => solution.GetDirectory());
var destDirectory = String.IsNullOrEmpty(deployDir)?"C:\\Poller":deployDir;
var statusPollConsoleDir = "./StatusPollConsole/bin/Release";
///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    // Clean solution directories.
    foreach(var path in solutionPaths)
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/**/bin/" + configuration);
        CleanDirectories(path + "/**/obj/" + configuration);
    }
});

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}...", solution);
        NuGetRestore(solution);
    }
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{

    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);
        MSBuild(solution, settings =>
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                .WithProperty("TreatWarningsAsErrors","true")
                .WithTarget("Build")
                .SetConfiguration(configuration));
    }
});


Task("Deploy")
	.Description("Copies the files in the bin output directory to deployment location, if the dest direct doesn't exist, it will create it")
	.IsDependentOn("Build")
	.Does(()=>
	{
	//copy all fiels in release directory to dest directory
	var dir = new DirectoryInfo(destDirectory);
    if (dir.Exists) {
	 foreach(var f in System.IO.Directory.GetFiles(statusPollConsoleDir,"*")) {
	  System.IO.File.Copy(f,String.Concat(destDirectory,"\\", new System.IO.FileInfo(f).Name)) ;
	 }
    }else {
		 System.IO.Directory.CreateDirectory(dir.FullName); 
		 foreach(var f in System.IO.Directory.GetFiles(statusPollConsoleDir,"*")) {
			Console.WriteLine(f); 
			System.IO.File.Copy(f,String.Concat(destDirectory,"\\", new System.IO.FileInfo(f).Name)) ;
	     }
    }
 });
///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("This is the default task which will be ran if no specific target is passed in.")
    .IsDependentOn("Deploy");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);