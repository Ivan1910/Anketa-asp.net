using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Anketa_vipdata
{
    public class Anketa
    {
        public readonly string naslovAnkete;
        public readonly int anketaID;
        public readonly DateTime aktivnaDo;
        public readonly DateTime aktivnaOd;
        public List<KeyValuePair<string, int>> listaOdgovora;

        public Anketa(string naslovAnkete, int id, DateTime aktivnaDo, DateTime aktivnaOd)
        {
            this.naslovAnkete = naslovAnkete;
            this.anketaID = id;
            this.aktivnaDo = aktivnaDo;
            this.aktivnaOd = aktivnaOd;
            listaOdgovora = new List<KeyValuePair<string, int>>();
        }
    }
}