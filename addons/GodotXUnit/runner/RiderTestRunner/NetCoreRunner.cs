using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Godot;
using GodotXUnitApi;
using GodotXUnitApi.Internal;
using Environment = System.Environment;
using Thread = System.Threading.Thread;

// ReSharper disable once CheckNamespace
namespace RiderTestRunner
{
    public static class NetRunnerHack{
        public static void SetGdu(NetCoreRunner runner)
        {
            GDU.Instance = runner;
        }
    }
    
    // ReSharper disable once UnusedType.Global
    public partial class NetCoreRunner : GodotXUnitRunnerBase // for GodotXUnit use: public partial class Runner : GodotTestRunner. https://github.com/fledware/GodotXUnit/issues/8#issuecomment-929849478
    {
        private string _runnerAssemblyPath;
        public override async void _Ready()
        {
            var pluginCtx = AssemblyLoadContext.GetLoadContext(GetType().Assembly);
            var ourCtx = AssemblyLoadContext.Default;
            
            ourCtx.Resolving += (context, name) =>
            {
                if (name.FullName.Contains("GodotXUnit"))
                {
                    var basePath = "/home/ivan-shakhov/Work/GodotXUnit/.godot/mono/temp/bin/Debug/";
                    var filePath = Path.Combine(basePath, name.Name + ".dll");
                    if (File.Exists(filePath))
                        return context.LoadFromAssemblyPath(filePath);
            
                    //var asm = pluginCtx.LoadFromAssemblyName(name);
                    //if (asm != null)
                        //return asm;
                }
            
                return null;
            };
            
            var gduAssembly = ourCtx.LoadFromAssemblyName(typeof(NetRunnerHack).Assembly.GetName());
            var gduType = gduAssembly.GetType(typeof(NetRunnerHack).FullName);
            gduType.GetMethod(nameof(NetRunnerHack.SetGdu)).Invoke(null, new []{this});
            
            Console.WriteLine("AssemblyLoadContext_Ready_This:"+ ourCtx);
            Console.WriteLine("AssemblyLoadContext_Ready:"+ AssemblyLoadContext.GetLoadContext(typeof(GDU).Assembly));
            Console.WriteLine("AppDomain_Ready: "+ AppDomain.CurrentDomain.Id);
            Console.WriteLine("CurrentProcess_Ready: "+ System.Diagnostics.Process.GetCurrentProcess().Id);
            
            //GDU.Instance = this; // for GodotXUnit https://github.com/fledware/GodotXUnit/issues/8#issuecomment-929849478
   
            var textNode = GetNode<RichTextLabel>("RichTextLabel");
            foreach (var arg in OS.GetCmdlineArgs())
            {
                textNode.Text += Environment.NewLine + arg;
            }

            if (OS.GetCmdlineArgs().Length < 4)
                return;
            
            var unitTestArgs = OS.GetCmdlineArgs()[4].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
            _runnerAssemblyPath = OS.GetCmdlineArgs()[2];
            
            ourCtx!.Resolving += OnResolving;

            var assembly = ourCtx.LoadFromAssemblyPath(_runnerAssemblyPath);
            assembly.EntryPoint.Invoke(null, new []{unitTestArgs});
            //AppDomain.CurrentDomain.ExecuteAssembly(_runnerAssemblyPath, unitTestArgs);
            //AppDomain.CurrentDomain.ExecuteAssemblyByName(assembly.GetName(), unitTestArgs); 
            GetTree().Quit();
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName)
        { 
            var dir = new FileInfo(_runnerAssemblyPath).Directory;
            if (dir == null) return null;
            var file = new FileInfo(Path.Combine(dir.FullName, $"{assemblyName.Name}.dll"));
            if (!file.Exists)
                file = new FileInfo(Path.Combine(dir.FullName, $"{assemblyName.Name}.exe"));
            if (file.Exists) 
                return context.LoadFromAssemblyPath(file.FullName);
            return null;
        }

        private async void WaitForThreadExit(Thread thread)
        {
            while (thread.IsAlive)
            {
                await ToSignal(GetTree().CreateTimer(100), "timeout");
            }
        }
    }
}