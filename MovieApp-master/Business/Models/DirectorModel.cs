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
    public class DirectorModel
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool IsRetired { get; set; }

        [DisplayName("Birth Date")]
        public string DateOutput { get; set; }

        public string NameOutput { get; set; }

       

        
    }
}
