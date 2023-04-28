using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using StaticDictionary.Generator;

namespace StaticDictionary
{

    [Generator]
    public class StaticDictionaryGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            
            foreach(var tree in context.Compilation.SyntaxTrees)
            {
                var text = tree.GetText(context.CancellationToken).ToString();
                if(text.Contains(@"IStaticDictionaryFactoryDefinition<"))
                {
                    foreach(var source in DictionaryFactory.ParseFile(text))
                    {
                        context.AddSource(source.FileName, source.Source);
                    }
                    
                    
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
			if (!Debugger.IsAttached)
			{
				//Debugger.Launch();
			}
#endif
		}
	}
}
