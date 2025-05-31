using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kargo_Takip_Sistemi.Enums;

namespace Kargo_Takip_Sistemi
{
    public abstract class Gonderi
    {
        public string TakipNo { get; set; }
        public string Gonderici { get; set; }
        public string Alici { get; set; }
        public GonderiDurumu Durum { get; set; }
        public double GonderiUcreti { get; set; }

        public virtual string ToDataString()
        {
            return $"{TakipNo};{Gonderici};{Alici};{Durum};{GetType().Name}";
        }

        public void DurumGuncelle(GonderiDurumu yeniDurum)
        {
            Durum = yeniDurum;
        }
    }
}
