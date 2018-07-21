using System;
using Hakon.Core;
using Hakon.Core.Brain.Cortex;

namespace Hakon.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.Write("Initializing concept network ... ");
            var cortex = new ConceptNetworkCortex();
            Console.Write("Complete");
            Console.WriteLine();
            Console.WriteLine("Awaiting input.");

            var userEntry = string.Empty;
            while(!userEntry.Equals("exit")){
                userEntry = Console.ReadLine();
                cortex.AddEntry(userEntry);

                Console.WriteLine(cortex.GenerateResponse().Message);
            }
        }
    }
}

