using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kargo_Takip_Sistemi
{
    public class YurtdisiGonderi : Gonderi, ITakipEdilebilir
    {
        public string Ulke { get; set; } // Yurtdışı gönderiler için ülke bilgisi
        public double GonderiUcreti { get; set; } // Yurtdışı gönderiler için gönderi ücreti

        public void DurumGuncelle(Enums.GonderiDurumu yeniDurum) // Durum güncelleme metodunu implement ediyoruz
        {
            this.Durum = yeniDurum; // Durumu güncelliyoruz
        }

        public override string ToDataString() // ToDataString metodunu override ediyoruz
        {
            return base.ToDataString() + $";{Ulke}"; // Temel sınıfın metodunu çağırıp ülke bilgisini ekliyoruz
        }
    }
}
