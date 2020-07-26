# GodotXUnit

A plugin for executing Xunit tests within godot.

## Examples

you can look at all the examples in the ./tests directory.

#### Run Test On Scene

    [FactOnScene("res://test_scenes/SomeTestScene.tscn")]
    public void IsOnCorrectScene()
    {
        var scene = GodotXUnitEvents.Instance.GetTree().CurrentScene;
        Assert.Equal(typeof(SomeTestSceneRoot), scene?.GetType());
    }
 
#### Run In Different Thread Contexts

    [Fact]
    public async void AllThreadContextInOne()
    {
        Assert.False(Engine.IsInPhysicsFrame());

        await GodotXUnitEvents.OnPhysicsProcessAwaiter;
        Assert.True(Engine.IsInPhysicsFrame());

        await GodotXUnitEvents.OnProcessAwaiter;
        Assert.False(Engine.IsInPhysicsFrame());
    }

## Installation

I'm going to try and get this in the godot AssetLib. Until then,
you will need to checkout this repo and copy the GodotXUnit
dir into your projects addons directory.

Next, you need to add a project reference to GodotXUnitApi project located
at ./addons/GodotXUnit/GodotXUnitApi/GodotXUnitApi.csproj in your main project.
GodotXUnitApi needs to be its own assembly for how xunit's
XunitTestCaseDiscoverer (please correct me if I'm wrong).

Next, make sure to include the cs files that are not in the GodotXUnitApi project:
* addons/GodotXUnit/Plugin.cs
* addons/GodotXUnit/runner/*.cs
* addons/GodotXUnit/editor/*.cs

Next, you will need to add the xunit (2.4.1) as a dependency for
your main project.

Next, enable the plugin. If there are errors that are not obvious, please
report them to me. 

If there is a better way to do this, please let me know. I'd really like
to make the installation of this easier and remove the manual steps.

## How It Works

When you click on a run, the first thing is the run args gets written
to ./addons/GodotXUnit/_work/RunArgs.json. this allows the runner to
know what needs to be ran. 

then, the runner scene is started with this command: 

`OS.Execute(OS.GetExecutablePath(), new [] {"res://addons/GodotXUnit/runner/GodotTestRunnerScene.tscn"}, false);`

then, events are listened to with file system events pulled by the editor. events
are sent by the runner for tests starting/finishing, and the entire summary.
this way we can update the editor while tests come in. events are passed
at the ./addons/GodotXUnit/_work directory.

finally, the results will be written to res://TestSummary.json. you can change
the location with the GodotXUnit/results_summary ProjectSetting.

## Known Issues

* sometimes the editor will just stop being able to pull events. i don't know
  why this is. but if it happens, hit the stop button and it should clean everything
  up safely and you can attempt rerun.
* we can only run tests to the class level.
* this does not integrate with IDEs, so tests have to be executed in the godot editor.
  But, you can run the tests via the cli if you execute the 
  res://addons/GodotXUnit/runner/GodotTestRunnerScene.tscn scene.

## Next steps

* get this into AssetLib
* figure out how to run other assemblies so we can put tests in different projects
  instead of the main project.
* try to figure out how to get an cs script running from an assembly other 
  than the main. that way we can get rid of the manual includes.
* create a custom runner so we can have better control of what tests run.
* setup a way to automatically add a test to the tree if the test extends a godot node.
* make the results be in the junit format
