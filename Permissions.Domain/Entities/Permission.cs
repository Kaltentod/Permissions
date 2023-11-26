using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Permissions.Domain.Entities
{
    public class Permission : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string EmployeeForename { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string EmployeeSurname { get; set; }

        [Required]
        public int PermissionType { get; set; }

        [Required]
        public DateTime PermissionDate { get; set; }

        [ForeignKey("PermissionType")]
        public virtual PermissionType PermissionTypeRel { get; set; }
    }
}
