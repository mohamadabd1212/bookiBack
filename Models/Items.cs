using System;
using System.ComponentModel.DataAnnotations;

namespace ruhanBack.models
{
    public class items
    {
        [Key]
        public string Id { get; set; }= Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; }
        
        public string Skin { get; set; }
        
        public string Type { get; set; }
        
        public string Rarity { get; set; }
        
        [DataType(DataType.Currency)]
        public decimal CSFloatPrice { get; set; }
        
        [DataType(DataType.Currency)]
        public decimal CSMoneyPrice { get; set; }
        
        [DataType(DataType.Currency)]
        public decimal Revenue { get; set; }  // Usually calculated as CSMoneyPrice - CSFloatPrice
        
        [DataType(DataType.Currency)]
        public decimal? BoughtAtPrice { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? BoughtAtDate { get; set; }
        
        [DataType(DataType.Currency)]
        public decimal? SoldAtPrice { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? SoldAtDate { get; set; }
    }
}
