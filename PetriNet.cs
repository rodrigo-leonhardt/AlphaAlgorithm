using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AlphaMiner
{
    public class PetriNet
    {
        private SimpleEventLog _simpleEventLog;

        public PetriNet(SimpleEventLog simpleEventLog)
        {
            this._simpleEventLog = simpleEventLog;
        }

        private PetriNetPlace AddPlaceBetweenTransitions(PetriNetDefinition definition, RelationshipMatrix matrix, PetriNetTransition from, PetriNetTransition to)
        {
            PetriNetPlace result = null;            

            foreach (var flowFrom in to.From)
            {                                        
                var placeFrom = flowFrom.From;
                var flowOrigin = placeFrom.From[0];
                var transOrigin = flowOrigin.From;

                    
                var relTypeFrom = matrix[Tuple.Create(from.Name, transOrigin.Name)];

                if (relTypeFrom == RelationshipType.NotFollow) //(c) XOR-Join pattern: a -> c, b -> c, and a#b
                {
                    result = placeFrom as PetriNetPlace;
                    definition.AddFlow(null, from, result);

                    return null;
                }            

            }

            var relType = matrix[Tuple.Create(from.Name, to.Name)];

            if (relType == RelationshipType.Cause) //(a) Sequence pattern: a -> b
            {
                result = definition.AddPlace(null, PetriNetPlaceType.Middle);
                definition.AddFlow(null, from, result);
                definition.AddFlow(null, result, to);                
            }

            
            return result;
        }

        public PetriNetDefinition Definition() 
        {
            var result = new PetriNetDefinition();

            //Start and End Places
            var startPlace = result.AddPlace(null, PetriNetPlaceType.Start);           
            var endPlace = result.AddPlace(null, PetriNetPlaceType.End);            

            //Transitions
            var activities = this._simpleEventLog.Activities();
            var actRelationships = this._simpleEventLog.ActivitiesAndRelationships();
            var matrixRelationships = this._simpleEventLog.SearchRelationships();

            foreach (var activity in activities)            
                result.AddTransition(activity);
            

            //Places and Flows
            foreach (var activity in activities)
            {
                var rel = actRelationships[activity];
                var trans = result.Transitions[activity];

                if (rel.Prior.Count == 0)                
                    result.AddFlow(null, startPlace, trans);                                

                if (rel.Next.Count == 0)
                    result.AddFlow(null, trans, endPlace);
                else 
                {
                    var listNext = rel.Next.Values.ToList();
                    var listPlaces = new List<PetriNetPlace>();

                    foreach (var nextTrans in listNext)
                    {                        

                        if (listPlaces.Count == 0) 
                        {
                            var newPlace = AddPlaceBetweenTransitions(result, matrixRelationships, trans, result.Transitions[nextTrans.Name]);
                            
                            if (newPlace != null)
                                listPlaces.Add(newPlace);

                        }
                        else
                        {

                            foreach (var place in listPlaces)
                            {
                                var placeTo = place.To[0].To.Name;

                                var relType = matrixRelationships[Tuple.Create(nextTrans.Name, placeTo)];

                                if (relType == RelationshipType.NotFollow) //(b) XOR-Split pattern: a -> b, b -> c, and b#c
                                {
                                    result.AddFlow(null, place, result.Transitions[nextTrans.Name]);

                                    break;
                                }
                                else if (relType == RelationshipType.ReciprocalFollow) //(d) AND-Split pattern: a -> b, b -> c, and b||c
                                {
                                    var newPlace = AddPlaceBetweenTransitions(result, matrixRelationships, trans, result.Transitions[nextTrans.Name]);                                    

                                    if (newPlace != null)
                                        listPlaces.Add(newPlace);

                                    break;
                                }

                            }

                        }

                    }                   
                    
                }
                
            }
           

            return result;
        }

    }

    public class PetriNetElement 
    {
        public PetriNetElement()
        {
            this.From = new List<PetriNetFlow>();
            this.To = new List<PetriNetFlow>();
        }

        public string Name { get; set; }
        public List<PetriNetFlow> From { get; set; }
        public List<PetriNetFlow> To { get; set; }
    }

    public enum PetriNetPlaceType
    {
        Start,
        End,
        Middle
    }

    public class PetriNetPlace : PetriNetElement
    {
        public PetriNetPlace(string name, PetriNetPlaceType type)
        {            
            this.Type = type;

            if (name == null)
            {
                this.Name = $"p{PetriNetPlace.Counter}";

                PetriNetPlace.Counter++;
            }
            else
                this.Name = name;

        }

        public PetriNetPlaceType Type { get; set; }

        private static int Counter = 1;
    }

    public class PetriNetTransition : PetriNetElement
    {        

        public PetriNetTransition(string name)
        {

            if (name == null)
            {
                this.Name = $"t{PetriNetTransition.Counter}";

                PetriNetTransition.Counter++;
            }
            else
                this.Name = name; 

        }

        private static int Counter = 1;

    }

    public class PetriNetFlow : PetriNetElement
    {

        public PetriNetFlow(string name, PetriNetElement from, PetriNetElement to)
        {            
            this.From = from;
            from.To.Add(this);

            this.To = to;
            to.From.Add(this);


            if (name == null)
            {
                this.Name = $"f{PetriNetFlow.Counter}";

                PetriNetFlow.Counter++;
            }
            else
                this.Name = name;

        }

        public PetriNetElement From { get; set; }
        public PetriNetElement To { get; set; }

        private static int Counter = 1;
    }


    public class PetriNetDefinition
    {
        public PetriNetDefinition()
        {
            this.Places = new Dictionary<string, PetriNetPlace>();
            this.Transitions = new Dictionary<string, PetriNetTransition>();
            this.Flows = new Dictionary<Tuple<string, string>, PetriNetFlow>();
        }

        public Dictionary<string, PetriNetPlace> Places { get; set; }
        public Dictionary<string, PetriNetTransition> Transitions { get; set; }
        public Dictionary<Tuple<string, string>, PetriNetFlow> Flows { get; set; }

        public PetriNetPlace AddPlace(string name, PetriNetPlaceType type)
        {
            var result = new PetriNetPlace(name, type);

            this.Places.Add(result.Name, result);

            return result;
        }

        public PetriNetTransition AddTransition(string name)
        {
            var result = new PetriNetTransition(name);

            this.Transitions.Add(result.Name, result);

            return result;
        }

        public PetriNetFlow AddFlow(string name, PetriNetElement from, PetriNetElement to)
        {
            var id = Tuple.Create(from.Name, to.Name);
            
            if (!this.Flows.TryGetValue(id, out var result))
            {
                result = new PetriNetFlow(name, from, to);
                this.Flows.Add(id, result);
            }

            return result;
        }

    }

}
