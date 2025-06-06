using System;
using System.ComponentModel.DataAnnotations;

namespace ruhanBack.Dtos
{
    public class SelltItemDto
    {

        [Required]
        public string Id { get; set; }
        
        public decimal Revenue { get; set; } 
    
        public decimal? SoldAtPrice { get; set; }
        
        

    }
}
