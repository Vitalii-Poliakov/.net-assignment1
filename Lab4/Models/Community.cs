using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Lab4.Models
{
    public class Community
    {


        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name ="Registration Number")]
        [Required]
        public string ID { get; set; }

        [StringLength(50, MinimumLength =3)]
        [Required]
        public string Title { get; set; }


       
       [DataType(DataType.Currency)]
       [Column(TypeName = "money")]

       public decimal Budget { get; set; }

       public ICollection<CommunityMembership> Memberships { get; set; }


    }
}
