using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace DataAccess.Entities
{
    public class Genre
    {
        public int Id { get; set; }

        [StringLength(75)]
        public string Name { get; set; }

        public List<MovieGenre> MovieGenres { get; set; }

    }
}
