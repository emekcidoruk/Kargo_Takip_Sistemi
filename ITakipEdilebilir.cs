using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kargo_Takip_Sistemi.Enums;

namespace Kargo_Takip_Sistemi
{
    public interface ITakipEdilebilir
    {
        string TakipNo { get; set; } // Takip numarası
        GonderiDurumu Durum { get; set; } // Gönderi durumu

        void DurumGuncelle(GonderiDurumu yeniDurum);
    }
}
