using System;
using System.Collections.Generic;

namespace Eventmanagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Event e = new Event("Full Force", DateTime.Parse("29.06.2022"), DateTime.Parse("02.07.2022"));
            Ticket t = new ZweiTageTicket(DateTime.Parse("29.06.2022"), DateTime.Parse("30.06.2022"), e);
            Besucher b = new Besucher(t);
            t.Besucher = b;
            t.Einzahlen(250);

            Verkaufsstand cocktailBar1 = new Verkaufsstand("Erdbeerbowle-Stand");
            cocktailBar1.Bezahlen(b.Armband, 10);
            cocktailBar1.Bezahlen(b.Armband, 17);

            Verkaufsstand imbiss1 = new Verkaufsstand("Veggie-Döner");
            imbiss1.Bezahlen(b.Armband, 15);

            b.ZeigeTransaktionen();

            Console.WriteLine("Kontostand: " + b.Konto.Kontostand);
        }
    }

    class Event
    {
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }

        public List<Ticket> tickets;

        public Event(string name, DateTime start, DateTime stop)
        {
            Name = name;
            Start = start;
            Stop = stop;
            tickets = new List<Ticket>();
        }
    }

    abstract class Ticket
    {
        public int Preis { get; set; }
        public Event Event { get; set; }
        public Besucher Besucher { get; set; }

        public Ticket(Event e)
        {
            Event = e;
        }

        public void Einzahlen(int betrag)
        {
            Besucher.Konto.AddTransaktion(new Einzahlung(DateTime.Now, betrag));
        }
    }

    class Tagesticket : Ticket
    {
        public DateTime Tag { get; set; }

        public Tagesticket(DateTime tag, Event e) : base(e)
        {
            Tag = tag;
            Preis = 60;
        }
    }

    class ZweiTageTicket : Ticket
    {
        public DateTime Tag1 { get; set; }
        public DateTime Tag2 { get; set; }

        public ZweiTageTicket(DateTime tag1, DateTime tag2, Event e) : base(e)
        {
            Tag1 = tag1;
            Tag2 = tag2;
            Preis = 100;
        }
    }

    class Normalticket : Ticket
    {
        public Normalticket(Event e) : base(e)
        {
            Preis = 180;
        }
    }

    class Besucher
    {
        private static int number = 1;

        public Ticket Ticket { get; set; }
        public Armband Armband { get; set; }
        public Konto Konto { get; set; }

        public Besucher(Ticket ticket)
        {
            Ticket = ticket;
            Armband = new Armband(Besucher.number++, ticket);
            Konto = new Konto(ticket);
        }

        public void ZeigeTransaktionen()
        {
            foreach(Transaktion t in Konto.GetTransaktionen())
            {
                Console.WriteLine(t);
            }
        }
    }

    class Armband
    {
        public int Nummer { get; set; }
        public Ticket Ticket { get; set; }

        public Armband(int nummer, Ticket ticket)
        {
            Nummer = nummer;
            Ticket = ticket;
        }
    }

    class Konto
    {
        public int Kontostand { get; set; }
        public Ticket Ticket { get; set; }
        
        public List<Transaktion> transaktionen;

        public Konto(Ticket t)
        {
            Ticket = t;
            transaktionen = new List<Transaktion>();
        }

        public void AddTransaktion(Transaktion t)
        {
            transaktionen.Add(t);
            if(t is Einzahlung)
            {
                Kontostand += t.Betrag;
            } else
            {
                Kontostand -= t.Betrag;
            }
        }

        public List<Transaktion> GetTransaktionen()
        {
            return transaktionen;
        }
    }

    abstract class Transaktion
    {
        public DateTime Datum { get; set; }
        public int Betrag { get; set; }

        public Transaktion(DateTime datum, int betrag)
        {
            Datum = datum;
            Betrag = betrag;
        }
    }

    class Einzahlung : Transaktion
    {
        public Einzahlung(DateTime datum, int betrag) : base(datum, betrag)
        {

        }

        public override string ToString()
        {
            return "(" + Datum + ") [Einzahlung] " + Betrag;
        }
    }

    class Zahlvorgang : Transaktion
    {
        public Verkaufsstand Wo { get; set; }

        public Zahlvorgang(DateTime datum, int betrag, Verkaufsstand wo) : base(datum, betrag)
        {
            Wo = wo;
        }

        public override string ToString()
        {
            return "(" + Datum + ") [Bezahlung] ("+Wo+") " + Betrag;
        }
    }

    class Verkaufsstand
    {
        public string Name { get; set; }

        public Verkaufsstand(string name)
        {
            Name = name;
        }

        public void Bezahlen(Armband a, int preis)
        {
            a.Ticket.Besucher.Konto.AddTransaktion(new Zahlvorgang(DateTime.Now, preis, this));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
