using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.model
{
    [Table("User")]
    public class User
    {
        public TableModel table { get; set; }
        [Key]
        [Column("Word")]
        [Display(Name = "Word")]
        public int Id { get; set; }

        public decimal? reportAmount { get; set; }

        public List<TableModel> tables { get; set; }

        public Nullable<decimal> LDBL { get; set; }
        public virtual ICollection<TableModel> DH_Logss { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        [StringLength(maximumLength:20,MinimumLength =10)]
        public string Name { get; set; }

        public string GetName() {
            return this.Name;
        }
        public void SetName(string name)
        {
            this.Name = name;
        }
        [DataType(DataType.Date)]
        public DateTime MyProperty { get; set; }
    }
}
