using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace MediatoR.Alternative.Lite.Tests.Helpers
{
    public class AssemblyCompiler
    {
        public Assembly Compile(string assemblyName, Assembly[] assemblies, params string[] sources)
        {
            //var externalAssemblies = new[]
            //{
            //    typeof(ICommand<>).Assembly,        // MediatoR.Alternative.Lite
            //    typeof(Result).Assembly             // FluentResults
            //};

            var references = new List<MetadataReference>();

            foreach (var assembly in assemblies.Distinct())
            {
                if (assembly == null || string.IsNullOrEmpty(assembly.Location)) continue;
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }

            //foreach (var assembly in externalAssemblies.Distinct())
            //{
            //    if (assembly == null || string.IsNullOrEmpty(assembly.Location)) continue;
            //    references.Add(MetadataReference.CreateFromFile(assembly.Location));
            //}

            try
            {
                var runtimePath = Path.Combine(
                    Path.GetDirectoryName(typeof(object).Assembly.Location)!,
                    "System.Runtime.dll"
                );

                if (File.Exists(runtimePath))
                {
                    references.Add(MetadataReference.CreateFromFile(runtimePath));
                }
            }
            catch { }

            var compilation = CSharpCompilation.Create(
                assemblyName: assemblyName,
                syntaxTrees: sources.Select(s => CSharpSyntaxTree.ParseText(s)),
                references: references.Distinct(),
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug)
            );

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => $"{d.GetMessage()} (Line: {d.Location.GetLineSpan().StartLinePosition.Line})");

                var loadedRefs = references.Select(r => r.Display).ToList();

                throw new InvalidOperationException(
                    $"Compilation failed:\n{string.Join("\n", errors)}\n\n" +
                    $"Loaded references:\n{string.Join("\n", loadedRefs)}\n\n" +
                    $"Full source:\n{string.Join("\n", sources)}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }
    }
}