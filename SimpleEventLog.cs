using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AlphaMiner
{

    public class Trace
    {
        public Trace()
        {
            this.Quantity = 1;
            this.Activities = new List<string>();
        }

        public string Id { get => getId(); }
        public int Quantity { get; set; }
        public List<string> Activities { get; set; }

        private string getId()
        {
            string result = "";

            foreach (string activity in Activities)
            {
                result += (result.Length > 0 ? ", " : "") + activity;
            }

            return result;
        }

        public Trace AddActivity(string name)
        {
            this.Activities.Add(name);

            return this;
        }

    }

    public class SimpleEventLog
    {
        public SimpleEventLog(EventLog eventLog)
        {
            this._traces = new Dictionary<string, Trace>();

            foreach (var lCase in eventLog.Cases)
            {
                var lActivities = new List<string>();

                foreach (var lActivity in lCase.Events)
                {

                    if (lActivity.Type == EventType.Complete)
                        lActivities.Add(lActivity.Activity);

                }

                AddTrace(lActivities);
            }

        }

        private Dictionary<string, Trace> _traces { get; set; }

        public Trace AddTrace(List<string> activities)
        {
            var newTrace = new Trace();

            foreach (var activity in activities)
            {
                newTrace.AddActivity(activity);
            }

            var traceId = newTrace.Id;

            if (this._traces.TryGetValue(traceId, out Trace oldTrace))
            {
                oldTrace.Quantity++;
                return oldTrace;
            }
            else
            {
                this._traces.Add(traceId, newTrace);
                return newTrace;
            }
            
        }

        public List<Trace> Traces()
        {
            var result = this._traces.Values.ToList();
            result.Sort((x, y) => y.Quantity.CompareTo(x.Quantity));

            return result;
        }

        public List<string> Activities()
        {
            var result = new List<string>();

            foreach (var lTrace in this._traces.Values)
            {
                foreach (var lActivity in lTrace.Activities)
                {

                    if (!result.Contains(lActivity))
                        result.Add(lActivity);

                }
            }

            return result;
        }

        public Dictionary<string, ActivityAndRelationships> ActivitiesAndRelationships()
        {
            var result = new Dictionary<string, ActivityAndRelationships>();

            foreach (var lTrace in this._traces.Values)
            {
                var actCount = lTrace.Activities.Count - 1;

                for (var i = 0; i <= actCount; i++)  
                {
                    var actualActName = lTrace.Activities[i];

                    if (!result.TryGetValue(actualActName, out var actualActObj))
                    {
                        actualActObj = new ActivityAndRelationships(actualActName);
                        result.Add(actualActName, actualActObj);
                    }
                    

                    if (i > 0)
                    {
                        var priorActName = lTrace.Activities[i-1];
                        var priorActObj = result[priorActName];

                        if (!actualActObj.Prior.ContainsKey(priorActName))
                        {
                            actualActObj.Prior.Add(priorActName, priorActObj);

                            priorActObj.Next.Add(actualActName, actualActObj);
                        }

                    }
                    

                }
            }

            return result;
        }

        public RelationshipMatrix SearchRelationships()
        {
            var result = new RelationshipMatrix();
            var actRelationships = this.ActivitiesAndRelationships();
            var actList = actRelationships.Keys.ToList();

            foreach (var actX in actList)
            {

                foreach (var actY in actList)
                {
                    var relType = RelationshipType.None;

                    if (actRelationships[actX].Next.ContainsKey(actY))
                    {
                        relType = RelationshipType.Follow;

                        if (actRelationships[actY].Next.ContainsKey(actX))
                            relType = RelationshipType.ReciprocalFollow;
                        else
                            relType = RelationshipType.Cause;

                    }
                    else if (!actRelationships[actY].Next.ContainsKey(actX))
                        relType = RelationshipType.NotFollow;


                    result.Add(new Tuple<string, string>(actX, actY), relType);
                }

            }

            return result;
        }

    }

    public class ActivityAndRelationships
    {
        public ActivityAndRelationships(string name)
        {
            this.Name  = name;
            this.Prior = new Dictionary<string, ActivityAndRelationships>();
            this.Next  = new Dictionary<string, ActivityAndRelationships>();
        }

        public string Name { get; }
        public Dictionary<string, ActivityAndRelationships> Prior { get; set; }
        public Dictionary<string, ActivityAndRelationships> Next { get; set; }
    }

    public enum RelationshipType
    {
        Follow, // A > B = B segue diretamente A (se A > B)
        NotFollow, // A # B = A nunca segue diretamente B e B nunca segue diretamente A (se A !> B e B !> A)
        ReciprocalFollow, // A || B = A às vezes segue B e B às vezes segue A (se A > B e B > A)
        Cause, // A -> B = A causa B. Portanto B segue A e A nunca segue B (se A > B e B !> A) 
        None
    }

    public class RelationshipMatrix : Dictionary<Tuple<string, string>, RelationshipType> { }

}
