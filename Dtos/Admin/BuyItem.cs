using System;
using System.ComponentModel.DataAnnotations;

namespace ruhanBack.Dtos
{
    public class BuytItemDto
    {

        [Required]
        public string Name { get; set; }
        
        public string Skin { get; set; }
        
        public string Type { get; set; }
        
        public string Rarity { get; set; }
        
        public decimal CSFloatPrice { get; set; }

        public decimal CSMoneyPrice { get; set; }
        
        public decimal Revenue { get; set; } 
    
        public decimal? BoughtAtPrice { get; set; }
        
        

    }
}
