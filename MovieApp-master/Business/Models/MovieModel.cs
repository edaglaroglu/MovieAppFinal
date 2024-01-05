using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace Business.Models
{
    public class MovieModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public short? Year { get; set; }

        public double Revenue { get; set; }

		[DisplayName("Director")]
		public int? DirectorId { get; set; }

        [DisplayName("Revenue")]
        public string RevenueOutput { get; set; }
      

    }
}
