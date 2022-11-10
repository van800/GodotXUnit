using Godot;
using GodotXUnitApi;
using Xunit;

namespace SubProjectForIntegrationTests
{
    public class SomeIntegrationTest
    {
        [GodotFact(Frame = GodotFactFrame.Process)]
        public void IsOnCorrectScene()
        {
            // var scene = GDU.CurrentScene;
            // Assert.Equal(typeof(SomeTestSceneRoot), scene?.GetType());
            GDU.WaitForFrames(1);
            GD.Print("tests");
        }
        //
        //
        // [GodotFact(Scene = "res://test_scenes/SomeTestScene.tscn")]
        // public void IsOnCorrectScene()
        // {
        //     var scene = GDU.CurrentScene;
        //     Assert.Equal(typeof(SomeTestSceneRoot), scene?.GetType());
        // }
    }
    
    public partial class SomeTestSceneRoot : Node2D
    {
        
    }
}