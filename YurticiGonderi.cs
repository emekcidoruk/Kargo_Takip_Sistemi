using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kargo_Takip_Sistemi
{
    public class YurticiGonderi : Gonderi, ITakipEdilebilir
    {
        public string Il { get; set; }
        public string Ilce { get; set; }
        public DateTime TahminiTeslimTarihi { get; set; }

        // Kurucu metod (opsiyonel)
        public YurticiGonderi()
        {
            TahminiTeslimTarihi = DateTime.Now.AddDays(2); // örnek: 2 gün sonra
        }

        public override string ToString() // ToString metodunu override ediyoruz
        {
            return $"{TakipNo} - {Gonderici} → {Alici} ({Il}/{Ilce}) [{Durum}] Tahmini Teslim: {TahminiTeslimTarihi.ToShortDateString()}";
        }

        public void DurumGuncelle(Enums.GonderiDurumu yeniDurum) // Durum güncelleme metodunu implement ediyoruz
        {
            this.Durum = yeniDurum; // Durumu güncelliyoruz
        }
    }
}
