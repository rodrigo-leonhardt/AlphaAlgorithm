using System;
using System.Collections.Generic;
using System.IO;

namespace AlphaMiner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inform the path to event log:");
            var filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }


            Console.WriteLine("Print simple event log? (Y/N)");            
            var printSimpleEventLog = Console.ReadLine().ToUpper();

            Console.WriteLine("Print relationships between activities? (Y/N)");            
            var printRelationships = Console.ReadLine().ToUpper();

            Console.WriteLine();

            Console.WriteLine("{0:HH:mm:ss.fff} - Loading event log", DateTime.Now);
            var eventLog = MXMLFile.LoadEventLog(filePath);
            //var eventLog = EventLogManual1(); //aula05.pdf pg 10
            //var eventLog = EventLogManual2(); //aula03.pdf pg 33
            //var eventLog = EventLogManual3(); //aula05.pdf L3 pg 17
            //var eventLog = EventLogManual4(); //aula05.pdf L4 pg 17


            Console.WriteLine();
            Console.WriteLine("{0:HH:mm:ss.fff} - Building simple event log", DateTime.Now);
            var simpleEventLog = new SimpleEventLog(eventLog);

            if (printSimpleEventLog == "Y")
              PrintSimpleEventLog(simpleEventLog);


            Console.WriteLine();
            Console.WriteLine("{0:HH:mm:ss.fff} - Searching relationships between activities", DateTime.Now);
            var listActivities = simpleEventLog.Activities();
            var relashionshipMatrix = simpleEventLog.SearchRelationships();

            if (printRelationships == "Y")
                PrintRelationshipMatrix(relashionshipMatrix, listActivities);


            Console.WriteLine();
            Console.WriteLine("{0:HH:mm:ss.fff} - Building Petri Net", DateTime.Now); 
            var petriNet = new PetriNet(simpleEventLog);
            var pnDefinition = petriNet.Definition();

            PrintPetriNetDefinition(pnDefinition);


            Console.WriteLine();
            Console.WriteLine("{0:HH:mm:ss.fff} - Done", DateTime.Now);            
        }

        static EventLog EventLogManual1()
        {
            var result = new EventLog();

            var newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "d";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "d";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "d";

            return result;
        }

        static EventLog EventLogManual2()
        {
            var result = new EventLog();

            var newCase = result.AddCase();
            newCase.AddEvent().Activity = "t1";
            newCase.AddEvent().Activity = "t2";
            newCase.AddEvent().Activity = "t3";
            newCase.AddEvent().Activity = "t4";
            newCase.AddEvent().Activity = "t5";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "t1";
            newCase.AddEvent().Activity = "t2";
            newCase.AddEvent().Activity = "t4";
            newCase.AddEvent().Activity = "t3";
            newCase.AddEvent().Activity = "t5";            

            return result;
        }

        static EventLog EventLogManual3()
        {
            var result = new EventLog();

            var newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "d";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "f";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "d";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "g";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "d";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "g";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "d";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "f";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "d";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "f";
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "d";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "e";
            newCase.AddEvent().Activity = "g";

            return result;
        }

        static EventLog EventLogManual4()
        {
            var result = new EventLog();

            var newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "d";            

            newCase = result.AddCase();            
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "d";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "a";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "e";

            newCase = result.AddCase();
            newCase.AddEvent().Activity = "b";
            newCase.AddEvent().Activity = "c";
            newCase.AddEvent().Activity = "e";

            return result;
        }

        static void PrintPetriNetDefinition(PetriNetDefinition definition)
        {
            //Places
            Console.Write("P = {");
            var i = 0;

            foreach (var place in definition.Places.Values)
            {
                if (i > 0)
                    Console.Write(", ");

                Console.Write(place.Name);
                i++;
            }

            Console.WriteLine("}");


            //Transitions
            Console.Write("T = {");
            i = 0;

            foreach (var transition in definition.Transitions.Values)
            {
                if (i > 0)
                    Console.Write(", ");

                Console.Write(transition.Name);
                i++;
            }

            Console.WriteLine("}");


            //Flows
            Console.Write("F = {");
            i = 0;

            foreach (var flow in definition.Flows.Values)
            {
                if (i > 0)
                    Console.Write(", ");

                Console.Write("("+ flow.From.Name +", "+ flow.To.Name +")");
                i++;
            }

            Console.WriteLine("}");
        }

        static void PrintRelationshipMatrix(RelationshipMatrix matrix, List<string> listActivities)
        {
            var sFollow = "";
            var sNotFollow = "";
            var sReciprocalFollow = "";
            var sCause = "";
            var sNone = "";            

            foreach (var actX in listActivities)
            {

                foreach (var actY in listActivities)
                {
                    var relType = matrix[Tuple.Create(actX, actY)];
                    var relText = $"({actX},{actY})";

                    if (relType == RelationshipType.Follow)
                        sFollow += (sFollow.Length > 0 ? ", " : "") + relText;
                    else if (relType == RelationshipType.Cause)
                        sCause += (sCause.Length > 0 ? ", " : "") + relText;
                    else if (relType == RelationshipType.NotFollow)
                        sNotFollow += (sNotFollow.Length > 0 ? ", " : "") + relText;
                    else if (relType == RelationshipType.ReciprocalFollow)
                        sReciprocalFollow += (sReciprocalFollow.Length > 0 ? ", " : "") + relText;
                    else
                        sNone += (sNone.Length > 0 ? ", " : "") + relText;

                }

            }

            //Console.WriteLine("Follow [> L1] = {" + sFollow + "}");
            Console.WriteLine("Cause [-> L1] = {" + sCause + "}");
            Console.WriteLine("NotFollow [# L1] = {" + sNotFollow + "}");
            Console.WriteLine("ReciprocalFollow [|| L1] = {" + sReciprocalFollow + "}");
            //Console.WriteLine("None = {" + sNone + "}");
        }

        static void PrintSimpleEventLog(SimpleEventLog simpleEventLog)
        {
            Console.WriteLine("L = [");

            var i = 0;

            foreach (var trace in simpleEventLog.Traces())
            {
                if (i > 0)
                    Console.WriteLine(",");

                Console.Write($"<{trace.Id}>{trace.Quantity}");

                i++;
            }

            Console.WriteLine();
            Console.WriteLine("]");
        }
        
    }

}
