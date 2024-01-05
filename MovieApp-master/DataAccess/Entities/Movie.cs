using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace DataAccess.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public short? Year { get; set; }

        public double Revenue { get; set; }

        public int? DirectorId { get; set; }

        public Director Director { get; set; }

        public List<MovieGenre> MovieGenres { get; set; }




    }
}
