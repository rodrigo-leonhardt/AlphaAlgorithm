using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaMiner
{
    public class EventLog
    {
        public EventLog()
        {
            this.Cases = new List<Case>();
        }

        public Case AddCase()
        {
            var result = new Case();

            this.Cases.Add(result);

            return result;
        }

        public string Process { get; set; }
        public string Description { get; set; }
        public List<Case> Cases { get; }

    }

    public class Case
    {
        public Case()
        {            
            this.Attributes = new Dictionary<string, string>();
            this.Events = new List<Event>();
        }

        public Event AddEvent()
        {
            var result = new Event();

            this.Events.Add(result);

            return result;
        }

        public string Id { get; set; }
        public Dictionary<string, string> Attributes { get; }
        public List<Event> Events { get; }

    }

    public class Event
    {
        public Event()
        {            
            this.Attributes = new Dictionary<string, string>();
            this.Type = EventType.Complete;
        }

        public string Id { get; set; }
        public EventType Type { get; set; }
        public string Activity { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Attributes { get; }

    }

    public enum EventType
    {
        Schedule,
        Assign,
        Withdraw,
        Reassign,
        Start,
        Suspend,
        Resume,
        Pi_abort,
        Ate_abort,
        Complete,
        AutoSkip,
        ManualSkip,
        Unknown
    }

}
